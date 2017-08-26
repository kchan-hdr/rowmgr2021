using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace geographia.ags.Test
{
    [TestClass]
    public class ParcelStatusTests
    {
        [TestMethod, TestCategory("AGS")]
        public async Task Simple_Get_Status()
        {
            var fs = new SunflowerParcel();
            var parcels = await fs.GetAllParcels();

            Assert.IsNotNull(parcels);
            Assert.AreNotEqual(0, parcels.Count());

            foreach( var p in parcels)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(p.ParcelId));
                Trace.WriteLine($"{p.ParcelId} - {p.ParcelStatus}");
            }
        }

        [TestMethod, TestCategory("AGS - destructive")]
        public async Task Simple_Update()
        {
            IFeatureUpdate fs = new SunflowerParcel();
            var good = await fs.UpdateFeature("1941800000002000", "Denied");

            Assert.IsTrue(good);
        }
    }
}
