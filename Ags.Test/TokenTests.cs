
using geographia.ags;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Ags.Test
{
    public class TokenTests
    {
        [Fact, Trait("Category","AGS")]
        public async Task Normal_Token_Gen()
        {
            var server = "http://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS_stg/FeatureServer";
            var h = new TokenHelper(server);

            // var good = await h.Validate();
            // Assert.IsTrue(good);

            var token = await h.GetToken();
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact, Trait("Category", "AGS")]
        public async Task Normal_Prod_Token_Gen()
        {
            var server = "http://gis05.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS/FeatureServer";
            var h = new TokenHelper(server);
            var token = await h.GetToken();
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact, Trait("Category", "AGS")]
        public async Task Bad_Service_Url()
        {
            var server = "http://gis05s.hdrgateway.com/arcgis/rest";
            var h = new TokenHelper(server);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => h.GetToken());
        }

        [Fact, Trait("Category", "AGS")]
        public async Task Bad_Service()
        {
            var server = "http://gis05s.hdrgateway.com/arcgis/rest/services/California/B2H_ROW_Parcels_FS_stgg/FeatureServer";
            var h = new TokenHelper(server);

            await Assert.ThrowsAsync<InvalidOperationException>(() => h.Validate());
        }
    }
}
