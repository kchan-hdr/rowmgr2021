using geographia.ags;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static geographia.ags.B2hParcel;

namespace Backfill
{
    internal class B2hFeature
    {
        B2hParcel helper = new B2hParcel();

        readonly string _URL = "https://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS/FeatureServer";
        readonly int _LAYERID = 0;

        internal async Task<int> GetAllFeatures()
        {
            FileStream fu = File.OpenWrite(@"D:\fu.csv");
            var wrt = new StreamWriter(fu);

            var req = $"{_URL}/{_LAYERID}/query?f=json&where=OBJECTID is not null&returnGeometry=false&returnIdsOnly=true";

            var client = new HttpClient();
            var response = await client.GetAsync(req);
            response.EnsureSuccessStatusCode();

            var responseText = await response.Content.ReadAsStringAsync();

            var obj = JObject.Parse(responseText);
            var ids = obj["objectIds"] ?? throw new IndexOutOfRangeException();

            foreach (var id in ids)
            {
                var polys = await this.GetParcels(id.Value<string>());

                foreach( var poly in polys )
                {
                    Console.WriteLine($"{poly.OBJECTID} {poly.ParcelId}");
                    await wrt.WriteLineAsync($"{poly.OBJECTID},{poly.ParcelId},{poly.ParcelStatus}");
                }
            }

            wrt.Close();
            fu.Close();

            return 0;
        }

        public async Task<IEnumerable<Status_dto>> GetParcels(string pid)
        {
            if (string.IsNullOrWhiteSpace(pid))
                throw new ArgumentNullException("parcel apn");

            var req = $"{_URL}/{_LAYERID}/query?f=json&where=OBJECTID%3D'{pid}'&returnGeometry=false&returnIdsOnly=false&outFields=OBJECTID,PARCEL_ID,ParcelStatus,ROE_Status,Documents";
            var r = await helper.GetAll<Status_dto>(req, (arr) =>
            {
                var list = new List<Status_dto>();

                foreach (var f in arr)
                {
                    var s = new Status_dto();
                    s.OBJECTID = f["attributes"].Value<int>("OBJECTID");
                    s.ParcelId = f["attributes"].Value<string>("PARCEL_ID");
                    s.ParcelStatus = f["attributes"].Value<string>("ParcelStatus");
                    s.RoeStatus = f["attributes"].Value<string>("ROE_Status");
                    s.Documents = f["attributes"].Value<string>("Documents");
                    list.Add(s);
                }

                return list;
            });

            return r;
        }
    }
}
