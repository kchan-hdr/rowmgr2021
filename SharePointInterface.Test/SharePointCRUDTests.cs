using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;

namespace SharePointInterface.Test
{
    [TestClass]
    public class SharePointCRUDTests
    {
        //private string _appId = "AppID";
        //private string _appSecret = "AppSecret";
        private string _appId = "a6fad0e8-3e1f-42eb-89f2-6cb8e1dcb329";
        private string _appSecret = "FMMJTzMMkP8CZOsL1IP3JvSoVWAOrF90zGxKVmUc2tc=";

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetTitle()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(_appId, _appSecret);
            string title = spCRUD.GetSiteTitle();
            Assert.IsNotNull(title);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetLists()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(_appId, _appSecret);
            ListCollection lists = spCRUD.ListAllLists();
            Assert.AreNotEqual(0, lists.Count);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetFolder()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(_appId, _appSecret);
            Folder folder = spCRUD.GetOrCreateFolder("Test");
            Assert.AreEqual("Test", folder.Name);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void DocExists()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(_appId, _appSecret);
            Folder folder = spCRUD.GetOrCreateFolder("Test");
            bool docExists = spCRUD.DocExists(folder, "test.txt");
            Assert.IsTrue(docExists);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void InsertDoc()
        {
            string fileName = @"C:\Users\cpyle\Documents\Sunflower\test2.txt";

            SharePointCRUD spCRUD = new SharePointCRUD(_appId, _appSecret);
            Folder folder = spCRUD.GetOrCreateFolder("Test");
            bool docExists = spCRUD.DocExists(folder, "test2.txt");
            if (!docExists)
            {
                byte[] bytes = System.IO.File.ReadAllBytes(fileName);
                docExists = spCRUD.InsertDoc(folder, "test2.txt", bytes);
            }
            Assert.IsTrue(docExists);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetDocTargetPath()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(_appId, _appSecret);
            var docPath = spCRUD.GetDocTargetPath("", "Test", "0");
            Console.WriteLine(docPath.ToString());
            Assert.AreEqual(4, docPath.Count);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void UploadParcelDoc()
        {
            string fileName = @"C:\Users\cpyle\Documents\Sunflower\test2.txt";

            SharePointCRUD spCRUD = new SharePointCRUD(_appId, _appSecret);
            byte[] bytes = System.IO.File.ReadAllBytes(fileName);
            bool docExists = spCRUD.UploadParcelDoc("Test", "0", "test2.txt", bytes);
            Assert.IsTrue(docExists);
        }
    }
}
