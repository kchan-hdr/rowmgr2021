using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal.Test
{
    [TestClass]
    public class ContactLogTests
    {
        [TestMethod]
        public async Task Test_Add_Log()
        {
            var repo = new OwnerRepository();

            var pid = repo.GetParcels().Last();
            var p = await repo.GetParcel(pid);
            var n = p.ContactLogs.Count();

            var a = await repo.GetAgent("Agent 99");

            var px = await repo.RecordContact(p, a, "whatever", DateTimeOffset.Now, "dev");

            Assert.IsNotNull(px);
            Assert.AreEqual(pid, px.ParcelId);
            Assert.AreEqual(n + 1, px.ContactLogs.Count());
        }
    }
}
