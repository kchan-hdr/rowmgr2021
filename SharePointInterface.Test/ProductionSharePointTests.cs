using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharePointInterface.Test
{
    [TestClass]
    public class ProductionSharePointTests
    {
        [TestMethod, TestCategory("SharePointCRUD")]
        public void TestConnection()
        {
            ISharePointCRUD sp = new SharePointCRUD();
            var t = sp.GetSiteTitle();
            Assert.AreNotEqual("NA", t);
            Trace.WriteLine(t);


            var lists = sp.ListAllLists();
            Assert.AreNotEqual(0, lists.Count);

            foreach( var l in lists)
            {
                Trace.WriteLine(l.Title);
            }
        }

        [TestMethod, TestCategory("SharePointCRUD - Destructive")]
        public void InsertDoc()
        {
            string fileName = @"testdata\logs.csv";

            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
            Microsoft.SharePoint.Client.Folder folder = spCRUD.GetOrCreateFolder("Test");
            bool docExists = spCRUD.DocExists(folder, "test2.txt");
            if (!docExists)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(fileName);
                docExists = spCRUD.InsertDoc(folder, "test2.txt", bytes);
            }
            Assert.IsTrue(docExists);
        }
    }
}
