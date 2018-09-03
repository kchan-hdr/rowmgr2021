using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using geographia.ags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ags.Test
{
    [TestClass]
    public class FeatureTests
    {
        [TestMethod, TestCategory("AGS")]
        public async Task Simple_Blackhill()
        {
            var h = new BlackhillParcel();
            var parcels = await h.GetAllParcels();

            Assert.IsNotNull(parcels);
            Assert.AreEqual(67, parcels.Count());

            foreach (var p in parcels)
            {
                Trace.WriteLine($"{p.OBJECTID}, {p.ParcelId}");
                Assert.IsFalse(string.IsNullOrWhiteSpace(p.ParcelId));
            }
        }
    }
}
