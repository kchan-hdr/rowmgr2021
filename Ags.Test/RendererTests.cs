using geographia.ags;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Ags.Test
{
    public class RendererTests
    {
        readonly ITestOutputHelper _log;

        public RendererTests(ITestOutputHelper h) => _log = h;

        [Fact, Trait("Category", "AGS Renderer")]
        public async Task Simple_Renderer()
        {
            var s = new WhartonParcel("https://maps-stg.hdrgateway.com/arcgis/rest/services/Texas/CoW_ROW_MapService/MapServer");
            var r = await s.Describe(0);
            Assert.False(string.IsNullOrWhiteSpace(r));

            var r1 = await s.GetDomainValues(0);
            Assert.True(r1.Any());

            foreach (var v in r1)
            {
                _log.WriteLine(v.ToString());
            }
        }
    }
}
