﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public class FeatureService_Base
    {
        static HttpClient _Client;

        protected string _URL;
        protected int _LAYERID;

        static FeatureService_Base()
        {
            _Client = new HttpClient();
        }

        public virtual async Task<IEnumerable<int>> Find(int layerId, string query)
        {
            if (layerId < 0)
                throw new ArgumentNullException(nameof(layerId));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var q = $"{_URL}/{layerId}/query?returnGeometry=fale&returnIdsOnly=true&f=json&where={query}";
            var response = await _Client.GetStringAsync(q);
            var r = JObject.Parse(response);

            if (!r.TryGetValue("objectIdFieldName", out var fieldname))
            {
                Trace.TraceWarning("missing OID field name");
            }

            var idx = r["objectIds"];
            if (idx.Type == JTokenType.Array)
            {
                var ids = (JArray)idx;
                return ids.Select<JToken,int>(id => id.Value<int>());
            }

            throw new KeyNotFoundException(query);
        }

        /// <summary>
        /// gis05s seems broken
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="parser"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> _GetAll<T>(string query, Func<JArray, IEnumerable<T>> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var results = new List<T>();
            var hasMore = true;
            var offset = 0;
            var step = 100;

            while (hasMore)
            {
                var myQuery = $"{query}&supportsPagination=true&resultOffset={offset}&resultRecordCount={step}";
                var response = await _Client.GetAsync(query);
                response.EnsureSuccessStatusCode();

                var responseText = await response.Content.ReadAsStringAsync();
                var obj = JObject.Parse(responseText);

                var ff = obj["features"];
                if ( ff.Type == JTokenType.Array)
                {
                    results.AddRange(parser((JArray)ff));
                }

                hasMore = HasMore(obj);
                if (hasMore)
                    offset += step;
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// ArcGIS REST Query response
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool HasMore(JObject obj)
        {
            var good = obj["exceededTransferLimit"]?.Value<bool>() ?? false;
            return good;
        }


        public virtual async Task<IEnumerable<T>> GetAll<T>(string query, Func<JArray, IEnumerable<T>> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            var responseText = await _Client.GetStringAsync(query);
            var obj = JObject.Parse(responseText);
            var ff = obj["features"];
            if (ff.Type == JTokenType.Array)
            {
                return parser((JArray)ff);
            }
            else
            {
                return new List<T>();
            }
        }

        public virtual async Task<bool> Update(int layerId, HttpContent reqContent)
        {
            if (layerId < 0)
                throw new ArgumentNullException(nameof(layerId));

            if (reqContent == null)
                throw new ArgumentNullException(nameof(reqContent));

            var q = $"{_URL}/{layerId}/updateFeatures";
            var response = await _Client.PostAsync(q, reqContent);
            response.EnsureSuccessStatusCode();
            var responseText = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(responseText);

            var rst = obj["updateResults"];
            if (rst.Type == JTokenType.Array)
            {
                var results = (JArray)rst;
                return results.All(rx => rx["success"].Value<bool>());
            }
            else
            {
                return false;
            }
        }
    }
}