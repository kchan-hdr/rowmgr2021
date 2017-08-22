using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal.Test
{
    [TestClass]
    public class DocumentTests
    {
        [TestMethod()]
        public async Task Test_Simple_File_Upload()
        {
            var repo = new OwnerRepository();

            var a = await repo.GetAgent("Agent 99");
            var d = await repo.Store("test upload", "test", "application/test", "test.txt", a.AgentId, null);
            Assert.IsNotNull(d);
        }
    }
}
