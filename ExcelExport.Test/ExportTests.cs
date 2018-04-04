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
    }
}
