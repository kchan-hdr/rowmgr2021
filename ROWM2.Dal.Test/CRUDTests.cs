using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal.Test
{
    [TestClass]
    public class CRUDTests
    {
        ROWM_Context myCtx;

        [TestInitialize]
        public void Init()
        {
            myCtx = new ROWM_Context(DbConnection.GetConnectionString());
        }

        [TestMethod, TestCategory("DAL")]
        public async Task Simple_Owner_Add()
        {
            var repo = new OwnerRepository(myCtx);
            var o1 = await repo.AddOwner("dev_user");
            Assert.IsNotNull(o1);
            Assert.AreEqual("dev_user", o1.PartyName);
            Assert.IsNotNull(o1.ContactInfo); //.Contacts);
            Assert.AreNotEqual(0, o1.ContactInfo.Count());
        }

        [TestMethod, TestCategory("DAL")]
        public async Task Simple_All_Parcel_IDs()
        {
            var repo = new OwnerRepository(myCtx);
            var ps = repo.GetParcels();
            Assert.IsNotNull(ps);
            Assert.AreNotEqual(0, ps.Count());

            foreach (var p in ps)
            {
                Trace.WriteLine(p);
            }

            var testId = ps.Last();
            var parcel = await repo.GetParcel(testId);
            Assert.IsNotNull(parcel);
            Assert.AreEqual(testId, parcel.Assessor_Parcel_Number);
            Assert.IsNotNull(parcel.Ownership); // .Owners);

            Trace.WriteLine($"parcel {parcel.ParcelId}");
            foreach (var o in parcel.Ownership)
            {
                Trace.WriteLine(o.Owner.PartyName);
            }
        }

        [TestMethod, TestCategory("DAL")]
        public async Task Simple_Parcel_Speed()
        {
            var repo = new OwnerRepository(myCtx);
            var repo2 = new ParcelRepository(myCtx);
            var ps = repo.GetParcels();
            Assert.IsNotNull(ps);
            Assert.AreNotEqual(0, ps.Count());

            var watch = new Stopwatch();
            watch.Start();

            foreach( var p in ps)
            {
                var parcel = repo2.GetParcel(p);
                Assert.IsNotNull(parcel);
                Assert.AreEqual(p, parcel.ParcelId);
                Assert.IsNotNull(parcel.Owners);
            }

            watch.Stop();
            Trace.TraceInformation($"elapse {watch.ElapsedMilliseconds}");

            watch.Restart();
            foreach (var p in ps)
            {
                var parcel = await repo.GetParcel(p);
                Assert.IsNotNull(parcel);
                Assert.AreEqual(p, parcel.ParcelId);
                Assert.IsNotNull(parcel.Ownership);
            }

            watch.Stop();
            Trace.TraceInformation($"elapse {watch.ElapsedMilliseconds}");
        }
    }
}
