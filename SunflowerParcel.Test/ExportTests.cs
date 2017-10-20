using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROWM.Controllers;

namespace SunflowerParcel.Test
{
    [TestClass]
    public class ExportTests
    {
        [TestMethod]
        public void Test_Export_Contacts()
        {
            var rep = new ROWM.Dal.OwnerRepository();
            var ctrl = new ExportController(rep);

            dynamic file = ctrl.ExportContact("excel");
            Assert.IsNotNull(file);

            System.IO.File.WriteAllBytes(@"D:/junk/contacts.csv", file.FileContents);
        }
    }
}
