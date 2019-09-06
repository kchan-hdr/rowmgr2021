using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public class ReservoirParcel : FeatureService_Base, IFeatureUpdate
    {
        static readonly string _PARCEL_KEY = "Assessor_Parcel_Number";

        static readonly int _ContactInfoTable = 8;

        public ReservoirParcel(string url = "")
        {
            _URL = string.IsNullOrWhiteSpace(url) ?
                "https://maps-stg.hdrgateway.com/arcgis/rest/services/California/Sites_Parcel_FS/FeatureServer"
                : url;

            _LAYERID = 0;

            SetSecured();
        }

        public async Task<IEnumerable<Status_dto>> GetAllParcels()
        {
            var req = $"{_URL}/{_LAYERID}/query?f=json&where=OBJECTID is not null&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,PARCEL_ID,ParcelStatus,ROE_Status,Documents";
            var r = await GetAll<Status_dto>(req, (arr) =>
            {
                var list = new List<Status_dto>();

                foreach (var f in arr)
                {
                    var s = new Status_dto();
                    s.OBJECTID = f["attributes"].Value<int>("OBJECTID");
                    s.ParcelId = f["attributes"].Value<string>(_PARCEL_KEY);
                    s.ParcelStatus = f["attributes"].Value<string>("ParcelStatus");
                    s.RoeStatus = f["attributes"].Value<string>("ROE_Status");
                    s.Documents = f["attributes"].Value<string>("Documents");
                    s.Landowner_Score = f["attributes"].Value<int>("Likelihood");
                    list.Add(s);
                }

                return list;
            });

            return r;
        }

        public async Task<IEnumerable<Status_dto>> GetParcels(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
                throw new ArgumentNullException("parcel apn");

            var req = $"{_URL}/{_LAYERID}/query?f=json&where={_PARCEL_KEY}%3D'{pid}'&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,{_PARCEL_KEY},ParcelStatus,ROE_Status,Documents";
            var r = await GetAll<Status_dto>(req, (arr) =>
            {
                var list = new List<Status_dto>();

                foreach (var f in arr)
                {
                    var s = new Status_dto();
                    s.OBJECTID = f["attributes"].Value<int>("OBJECTID");
                    s.ParcelId = f["attributes"].Value<string>(_PARCEL_KEY);
                    s.ParcelStatus = f["attributes"].Value<string>("ParcelStatus");
                    s.RoeStatus = f["attributes"].Value<string>("ROE_Status");
                    s.Documents = f["attributes"].Value<string>("Documents");
                    s.Landowner_Score = f["attributes"].Value<int>("Likelihood");
                    list.Add(s);
                }

                return list;
            });

            return r;
        }
        public async Task<bool> Update(IEnumerable<UpdateFeature> u)
        {
            var req = JsonConvert.SerializeObject(u);
            req = $"features={req}&f=json&gdbVersion=&rollbackOnFailure=true";
            var reqContent = new StringContent(req);
            reqContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return await base.Update(_LAYERID, reqContent);
        }

        async Task<bool> IFeatureUpdate.UpdateFeatureDocuments(string parcelId, string documentURL)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"{_PARCEL_KEY}='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    Documents = documentURL
                }
            });
            return await this.Update(u);
        }

        async Task<bool> IFeatureUpdate.UpdateFeature(string parcelId, int status)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"{_PARCEL_KEY}='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    ParcelStatus = status
                }
            });
            return await this.Update(u);
        }

        async Task<bool> IFeatureUpdate.UpdateFeatureRoe(string parcelId, int status) => await UpdateFeatureRoe_Impl(parcelId, status, "");

        async Task<bool> IFeatureUpdate.UpdateFeatureRoe_Ex(string parcelId, int status, string condition) => await UpdateFeatureRoe_Impl(parcelId, status, condition);

        async Task<bool> UpdateFeatureRoe_Impl(string parcelId, int status, string condition)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(0, $"{_PARCEL_KEY}='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    ROE_Status = status,
                    ROE_Condition = condition
                }
            });
            return await this.Update(u);
        }

        async Task<bool> IFeatureUpdate.UpdateRating(string parcelId, int rating)
        {
            if (string.IsNullOrWhiteSpace(parcelId))
                throw new ArgumentNullException(nameof(parcelId));

            var oid = await Find(_LAYERID, $"{_PARCEL_KEY}='{parcelId}'");
            var u = oid.Select(i => new UpdateFeature
            {
                attributes = new Status_Req
                {
                    OBJECTID = i,
                    Likelihood = rating
                }
            });
            return await this.Update(u);
        }


        #region off-label
        public async Task<bool> Update(ContactInfo_dto dto)
        {
            if (dto == null)
                return false;

            var oid = await Find(_ContactInfoTable, $"ContactId='{dto.ContactId}'");

            string action = string.Empty;
            ContactAttribute[] edits;
            if (oid != null && oid.Any())
            {
                dto.ESRI_OID = oid.First();
                edits = new ContactAttribute[] { new ContactAttribute() { attributes = dto } };
                action = "updates";
            }
            else
            {
                edits = new ContactAttribute[] { new ContactAttribute { attributes = dto } };
                action = "adds";
            }

            var req = JsonConvert.SerializeObject(edits);
            req = $"{action}={req}&f=json&gdbVersion=&rollbackOnFailure=true";
            var reqContent = new StringContent(req);
            reqContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return await base.Edit(_ContactInfoTable, reqContent);
        }

        public class Edits
        {
            public string f { get; set; } = "json";
            public ContactAttribute[] adds { get; set; }
            public ContactAttribute[] updates { get; set; }
        }

        public class ContactAttribute
        { 
            public ContactInfo_dto attributes { get; set; }
        }

        public class ContactInfo_dto
        {
            public int? ESRI_OID { get; set; } = null;
            public string ContactId { get; set; }
            public bool IsPrimaryContact { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZIP { get; set; }
            public string Email { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string WorkPhone { get; set; }
            public string ContactOwnerId { get; set; }
            public DateTimeOffset Created { get; set; }
            public DateTimeOffset? LastModified { get; set; }
            public string ModifiedBy { get; set; }
            public string Representation { get; set; }
        }
        #endregion

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
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string ROE_Condition { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int Likelihood { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Documents { get; set; }
        }
        #endregion
        #region dto
        public class Status_dto
        {
            public int OBJECTID { get; set; }
            public string ParcelId { get; set; }
            public string ParcelStatus { get; set; }
            public string RoeStatus { get; set; }
            public int Landowner_Score { get; set; }
            public string Documents { get; set; }

            internal enum Likelihood_Domain { Unknown = 0 , Likely, Unlikely };
            static int FromLikelihood(string code)
            {
                if ( Enum.TryParse<Likelihood_Domain>(code, out var value))
                    return (int)value;

                Trace.TraceWarning($"Unk Likelihood value {code}");
                return 0;
            }
        }
        #endregion
    }
}
