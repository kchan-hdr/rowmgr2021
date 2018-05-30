using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExcelExport.Test
{
    [TestClass]
    public class ExportTests
    {
        [TestMethod, TestCategory("Export")]
        public void Normal_Export()
        {
            var e = new Exporter();
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }

        [TestMethod, TestCategory("Export")]
        public void Agent_Log_Export()
        {
            var e = new AgentLogExport();
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }

        [TestMethod, TestCategory("Export")]
        public void Contact_List_Export()
        {
            var e = new ContactListExport();
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }

        [TestMethod, TestCategory("Export")]
        public void Doc_List_Export()
        {
            var e = new DocListExport();
            var bytes = e.Export();
            Assert.IsNotNull(bytes);
            Assert.AreNotEqual(0, bytes.Length);
        }
    }
}
