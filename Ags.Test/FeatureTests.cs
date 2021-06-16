using geographia.ags;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ags.Test
{
    public class FeatureTests
    {
        readonly ITestOutputHelper _log;
        public FeatureTests(ITestOutputHelper h) => _log = h;

        [Fact, Trait("Category", "AGS")]
        public async Task Simple_Blackhill()
        {
            var h = new BlackhillParcel();
            var parcels = await h.GetAllParcels();

            Assert.NotNull(parcels);
            Assert.Equal(79, parcels.Count());

            foreach (var p in parcels)
            {
                _log.WriteLine($"{p.OBJECTID}, {p.ParcelId}");
                Assert.False(string.IsNullOrWhiteSpace(p.ParcelId));
            }
        }

        [Fact, Trait("Category", "AGS")]
        public async Task Simple_Denver()
        {
            var h = new DenverParcel();
            var parcels = await h.GetAllParcels();

            Assert.NotNull(parcels);
            Assert.Equal(207, parcels.Count());

            // duplicated parcels
            var q = from p in parcels
                    group p by p.ParcelId into par
                    select par;

            foreach (var p in q)
                if (p.Count() > 1)
                    _log.WriteLine($"duplicated {p.Key} {p.Count()}");

            foreach (var p in parcels)
            {
                _log.WriteLine($"{p.OBJECTID}, {p.ParcelId}");
                Assert.False(string.IsNullOrWhiteSpace(p.ParcelId), $"bad parcel {p.OBJECTID}");
            }
        }

        [Fact, Trait("Category", "AGS")]
        public async Task Locked_down_sunflower()
        {
            var h = new SunflowerParcel("https://gis05s.hdrgateway.com/arcgis/rest/services/California/Sunflower_Parcel_Stg_FS/FeatureServer");
            var parcels = await h.GetAllParcels();

            var (t, d) = await h.Token();
            _log.WriteLine($"token {t} expiration {d}");

            Assert.NotNull(parcels);
            Assert.Equal(64, parcels.Count());

            foreach (var p in parcels)
            {
                _log.WriteLine($"{p.OBJECTID}, {p.ParcelId}");
                Assert.False(string.IsNullOrWhiteSpace(p.ParcelId));
            }
        }
    }
}
