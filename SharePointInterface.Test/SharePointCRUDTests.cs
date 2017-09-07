using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;

namespace SharePointInterface.Test
{
    [TestClass]
    public class SharePointCRUDTests
    {
        private string _appId = "AppID";
        private string _appSecret = "AppSecret";

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
            Folder folder = spCRUD.GetFolder("Test");
            Assert.AreEqual("Test", folder.Name);
        }
    }
}
