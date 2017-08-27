﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public class FeatureService_Base
    {
        protected string _URL;
        protected int _LAYERID;

        public virtual async Task<int> Find(int layerId, string query)
        {
            if (layerId < 0)
                throw new ArgumentNullException(nameof(layerId));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            using (var client = new HttpClient())
            {
                var q = $"{_URL}/{layerId}/query?returnGeometry=fale&returnIdsOnly=true&f=json&where={query}";
                var response = await client.GetStringAsync(q);
                var r = JObject.Parse(response);

                if ( ! r.TryGetValue("objectIdFieldName", out var fieldname))
                {
                    Trace.TraceWarning("missing OID field name");
                }

                var idx = r["objectIds"];
                if ( idx.Type == JTokenType.Array)
                {
                    var ids = (JArray)idx;
                    if ( ids != null && ids.Count() > 0)
                    {
                        if (ids.Count() > 1)
                            Trace.TraceWarning($"'{query}' returned more than 1 record");

                        return (int)ids.First();
                    }
                }

                throw new KeyNotFoundException(query);
            }
        }

        public virtual async Task<IEnumerable<T>> GetAll<T>(string query, Func<JArray, IEnumerable<T>> parser)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query));

            using (var client = new HttpClient())
            {
                var responseText = await client.GetStringAsync(query);
                var obj = JObject.Parse(responseText);
                var ff = obj["features"];
                if ( ff.Type==JTokenType.Array)
                {
                    return parser((JArray)ff);
                }
                else
                {
                    return new List<T>();
                }
            }
        }

        public virtual async Task<bool> Update(int layerId, HttpContent reqContent)
        {
            if (layerId < 0)
                throw new ArgumentNullException(nameof(layerId));

            if (reqContent == null)
                throw new ArgumentNullException(nameof(reqContent));

            using (var client = new HttpClient())
            {
                var q = $"{_URL}/{layerId}/updateFeatures";
                var response = await client.PostAsync(q, reqContent);
                response.EnsureSuccessStatusCode();
                var responseText = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(responseText);

                var rst = obj["updateResults"];
                if ( rst.Type == JTokenType.Array)
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
}
