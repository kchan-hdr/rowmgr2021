using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Dal;

namespace ROWM2.Dal.Test
{
    [TestClass]
    public class DbTests
    {
        [TestMethod, TestCategory("DAL")]
        public void Simple_Connection_Test()
        {
            var c = new ROWM_Context(DbConnection.GetConnectionString());
            var n = c.Owner.Count();
            Assert.IsTrue(n >= 0);

            Trace.WriteLine(n);
        }
    }
}
