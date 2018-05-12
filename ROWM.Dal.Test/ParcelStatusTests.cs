using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Controllers;

namespace ROWM.Dal.Test
{
    [TestClass]
    public class ParcelStatusTests
    {
        static readonly string _TEST_PARCEL = "1941700000002000";
        static readonly ParcelStatusHelper _helper = new ParcelStatusHelper(new ROWM_Context(DbConnection.GetConnectionString()));

        [TestMethod]
        public async Task Simple_Write_Status()
        {
            var repo = new OwnerRepository(new ROWM_Context(DbConnection.GetConnectionString()));

            var p = await repo.GetParcel(_TEST_PARCEL);

            foreach( var status in new int[] { 99,1,3,4,5,6,7 })
            {
                var c = _helper.ParseDomainValue(status);
                p.ParcelStatusCode = c;
                var touched = await repo.UpdateParcel(p);
                Assert.AreEqual(c, touched.ParcelStatusCode);

                Trace.WriteLine($"{p.ParcelStatusCode} {p.RoeStatusCode}");
            }
        }
    }
}
