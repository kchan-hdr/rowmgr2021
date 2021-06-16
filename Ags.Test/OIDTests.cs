using geographia.ags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ags.Test
{
    public class OIDTests
    {
        readonly ITestOutputHelper _log;
        public OIDTests(ITestOutputHelper h) => _log = h;

        readonly static List<IFeatureUpdate> parcels = new()
        {
            // 6934
            new AtcParcel("https://gis05s.hdrgateway.com/arcgis/rest/services/California/ATC_Line6943_Parcel_FS/FeatureServer"),
            new AtcParcel("https://maps.hdrgateway.com/arcgis/rest/services/California/ATC_Line6943_Parcel_FS/FeatureServer"),

            // 862
            new AtcParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/California/ATC_Line862_Parcel_FS/FeatureServer"),
            new AtcParcel("https://maps.hdrgateway.com/arcgis/rest/services/California/ATC_Line862_Parcel_FS/FeatureServer"),

            // chc
            new AtcParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/California/ATC_CHC_Parcel_FS/FeatureServer"),
            new AtcParcel("https://maps.hdrgateway.com/arcgis/rest/services/California/ATC_CHC_Parcel_FS/FeatureServer"),

            new AtpParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/Texas/ATP_Parcel_FS/FeatureServer"),
            new AtpParcel("https://maps.hdrgateway.com/arcgis/rest/services/Texas/ATP_Parcel_FS/FeatureServer")
        };

        public static IEnumerable<object[]> GetProviders()
        {
            foreach (var p in parcels)
                yield return new object[] { p };
        }

        [Theory]
        [Trait ("Category","AGS")]
        [MemberData(nameof(GetProviders))]
        public async Task Simple_OBJECT_ID(FeatureService_Base f)
        {
            var sch = new AgsSchema(f);
            Assert.NotNull(sch);

            var desc = await f.Describe(0);
            _log.WriteLine(desc);

            //
            switch (f)
            {
                case AtcParcel atc:
                    {
                        var p = await atc.GetAllParcels();
                        Assert.NotNull(p);
                        Assert.True(p.Any());

                        _log.WriteLine($"{p.Count()}");
                        foreach (var px in p)
                            _log.WriteLine($"{px.Attributes.OBJECTID} {px.Attributes.ParcelId}");

                        break;
                    }
                case AtpParcel atp:
                    {
                        var p = await atp.GetAllParcels();
                        Assert.NotNull(p);
                        Assert.True(p.Any());

                        _log.WriteLine($"{p.Count()}");
                        foreach (var px in p)
                            _log.WriteLine($"{px.Attributes.OBJECTID} {px.Attributes.ParcelId}");

                        break;
                    }
            }
        }
    }
}
