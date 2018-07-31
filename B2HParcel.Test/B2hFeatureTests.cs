using geographia.ags;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace B2HParcel.Test
{
    public class B2hFeatureTests
    {
        ITestOutputHelper _log;

        public B2hFeatureTests(Xunit.Abstractions.ITestOutputHelper h) => _log = h;

        [Fact, Trait("Category", "AGS")]
        public async Task Simple_Feature_Connection()
        {
            var h = new B2hParcel();
            var dtos = await h.GetAllParcels();
            Assert.NotEmpty(dtos);

            foreach( var p in dtos.Take(20))
                _log.WriteLine($"checking {p.ParcelId} doc => {p.Documents}");
        }

        [Theory, Trait("Category", "AGS")]
        [InlineData("03S36E00500", "doc")]
        public async Task Simple_Document_Update(string pid, string url)
        {
            var h = new B2hParcel();
            var good = await ((IFeatureUpdate)h).UpdateFeatureDocuments(pid, url);
            Assert.True(good);

            var dtos = await h.GetAllParcels();
            var p = dtos.First(px => px.ParcelId.Equals(pid));
            Assert.Equal(url, p.Documents);

        }
    }
}
