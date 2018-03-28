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
        private string _appId = "26589ee5-16ef-4444-9143-cfea08cba1cc"; // for ../row_dev/...
        private string _appSecret = "7T2/B1ZNHMw2EE90USqdVf8mMTfAze0LAjkT7ni7V7w=";

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetTitle()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
            string title = spCRUD.GetSiteTitle();
            Assert.IsNotNull(title);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetLists()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(); //  _appId, _appSecret);
            ListCollection lists = spCRUD.ListAllLists();
            Assert.AreNotEqual(0, lists.Count);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetFolder()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(); //  _appId, _appSecret);
            Folder folder = spCRUD.GetOrCreateFolder("Test");
            Assert.AreEqual("Test", folder.Name);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void DocExists()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(); //  _appId, _appSecret);
            Folder folder = spCRUD.GetOrCreateFolder("Test");
            bool docExists = spCRUD.DocExists(folder, "test.txt");
            Assert.IsTrue(docExists);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void InsertDoc()
        {
            string fileName = @"C:\Users\cpyle\Documents\Sunflower\test2.txt";

            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
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
            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
            var docPath = spCRUD.GetDocTargetPath("", "Test", "0");
            Console.WriteLine(docPath.ToString());
            Assert.AreEqual(4, docPath.Count);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void UploadParcelDoc()
        {
            string fileName = @"C:\Users\cpyle\Documents\Sunflower\test2.txt";

            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
            byte[] bytes = System.IO.File.ReadAllBytes(fileName);
            bool docExists = spCRUD.UploadParcelDoc("Test", "0", "test2.txt", bytes);
            Assert.IsTrue(docExists);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetParcelDoc()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
            System.IO.Stream docStream = spCRUD.GetParcelDoc("Test", "0", "test2.txt");
            Assert.IsNotNull(docStream);
        }

        [TestMethod, TestCategory("SharePointCRUD")]
        public void GetParcelFolderURL()
        {
            SharePointCRUD spCRUD = new SharePointCRUD(); // _appId, _appSecret);
            string expected = "https://hdroneview.sharepoint.com/SF-CH-TS/Documents/4.0%20ROW/4.3%20Parcels/055150000000100C%20MOONEY,%20PATRICIA%20D%20TRUST";
            // Ending illegal sharepoint Characters removed
            string actual = spCRUD.GetParcelFolderURL("055150000000100C MOONEY, PATRICIA D TRUST/&?#<>+"); 
            Assert.AreEqual(expected, actual);
            actual = spCRUD.GetParcelFolderURL("Folder that doesn't exist returns null");
            Assert.AreEqual(null, actual);
        }
    }
}
