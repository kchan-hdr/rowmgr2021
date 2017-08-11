using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal.Test
{
    [TestClass]
    public class CRUDTests
    {
        [TestMethod]
        public async Task Simple_Owner_Add()
        {
            var repo = new OwnerRepository();
            var o1 = await repo.AddOwner("dev_user");
            Assert.IsNotNull(o1);
            Assert.AreEqual("dev_user", o1.PartyName);
            Assert.IsNotNull(o1.Contacts);
            Assert.AreNotEqual(0, o1.Contacts.Count());
        }

        [TestMethod]
        public async Task Simple_All_Parcel_IDs()
        {
            var repo = new OwnerRepository();
            var ps = repo.GetParcels();
            Assert.IsNotNull(ps);
            Assert.AreNotEqual(0, ps.Count());

            foreach( var p in ps )
            {
                Trace.WriteLine(p);
            }

            var testId = ps.Last();
            var parcel = await repo.GetParcel(testId);
            Assert.IsNotNull(parcel);
            Assert.AreEqual(testId, parcel.ParcelId);
            Assert.IsNotNull(parcel.Owners);

            Trace.WriteLine($"parcel {parcel.ParcelId}");
            foreach( var o in parcel.Owners)
            {
                Trace.WriteLine(o.Owner.PartyName);
            }
        }
    }
}
