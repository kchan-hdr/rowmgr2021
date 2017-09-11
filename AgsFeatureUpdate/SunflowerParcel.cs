using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public class SunflowerParcel : FeatureService_Base, IFeatureUpdate
    {
        public SunflowerParcel(string url = "")
        {
            //_URL = "https://gis05s.hdrgateway.com/arcgis/rest/services/California/SunFlower_Parcels_FS/FeatureServer";
            _URL = string.IsNullOrWhiteSpace(url) ?
                "https://gis05s.hdrgateway.com/arcgis/rest/services/California/Sunflower_Parcel_Stg_FS/FeatureServer"
                : url;

            _LAYERID = 0;
        }

        public async Task<IEnumerable<Status_dto>> GetAllParcels()
        {
            var req = $"{_URL}/{_LAYERID}/query?f=json&where=OBJECTID is not null&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,PID,ParcelStatus,ROE_Status";
            var r = await GetAll<Status_dto>(req, (arr) =>
            {
                var list = new List<Status_dto>();

                foreach( var f in arr)
                {
                    var s = new Status_dto();
                    s.OBJECTID = f["attributes"].Value<int>("OBJECTID");
                    s.ParcelId = f["attributes"].Value<string>("PID");
                    s.ParcelStatus = f["attributes"].Value<string>("ParcelStatus");
                    s.RoeStatus = f["attributes"].Value<string>("ROE_Status");
                    list.Add(s);
                }

                return list;
            });

            return r;
        }

        public async Task<bool> Update(string parcelId, int status)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            if (status < 0)
                throw new ArgumentNullException(nameof(status));

            var oid = await Find(0, $"PID='{parcelId}'");
            var u = new UpdateFeature[]
            {
                new UpdateFeature
                {
                    attributes = new Status_Req
                    {
                        OBJECTID = oid,
                        ParcelStatus = status
                    }
                }
            };

            var req = JsonConvert.SerializeObject(u);
            // req = Uri.EscapeDataString(req);
            req = $"features={req}&f=json&gdbVersion=&rollbackOnFailure=true";
            var reqContent = new StringContent(req);
            reqContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return await base.Update(_LAYERID, reqContent);
        }

        public async Task<bool> Update2(string parcelId, Status_Req d)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var u = new UpdateFeature[]
            {
                new UpdateFeature
                {
                    attributes = d
                }
            };

            var req = JsonConvert.SerializeObject(u);
            req = Uri.EscapeDataString(req);
            req = $"features={req}&f=json&gdbVersion=&rollbackOnFailure=true";
            var reqContent = new StringContent(req);
            reqContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return await base.Update(_LAYERID, reqContent);
        }

        async Task<bool> IFeatureUpdate.UpdateFeature(string parcelId, int status) => await this.Update(parcelId, status);
        async Task<bool> IFeatureUpdate.UpdateFeatureRoe(string parcelId, int status)
        {
            var oid = await Find(0, $"PID='{parcelId}'");
            return await this.Update2(parcelId, new Status_Req { OBJECTID = oid, ROE_Status = status });
        }

        #region request
        public class UpdateRequest
        {
            public string f { get; set; } = "json";
            public UpdateFeature[] features { get; set; }
        }

        public class UpdateFeature
        {
            public Status_Req attributes { get; set; }
        }

        public class Status_Req
        {
            public int OBJECTID { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? ParcelStatus { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? ROE_Status { get; set; }
        }
        #endregion

        #region dto
        public class Status_dto
        {
            public int OBJECTID { get; set; }
            public string ParcelId { get; set; }
            public string ParcelStatus { get; set; }
            public string RoeStatus { get; set; }
        }
        #endregion
    }
}
