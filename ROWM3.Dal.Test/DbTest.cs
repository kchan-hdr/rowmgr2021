using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;

namespace ROWM3.Dal.Test
{
    [TestClass]
    public class DbTest
    {
        [TestMethod, TestCategory("DAL3")]
        public void Simple_Connection_Test()
        {
            var c = new ROWM_Context();
            var n = c.Owner.Count();
            Assert.IsTrue(n >= 0);

            Trace.WriteLine(n);
        }

        [TestMethod, TestCategory("DAL3")]
        public void Simple_Database_First_Insert()
        {
            var c = new ROWM_Context();


            var l = c.ContactLog.Create();
            l.Agent = c.Agent.First();
            l.Created = DateTime.UtcNow;
            c.ContactLog.Add(l);
            try
            {
                var touched = c.SaveChanges();
            }
            catch ( DbUpdateException e)
            {
                Trace.TraceError(e.Message);
            }

            Assert.AreNotEqual(Guid.Empty, l.ContactLogId);

            // clean up
            c.ContactLog.Remove(l);
            var touched2 = c.SaveChanges();
        }
    }
}
