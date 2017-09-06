using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;

namespace SharePointInterface
{
    class SharePointCRUD
    {
        private ClientContext _ctx;
        private string _parcelsFolderName;
        private string _siteUrl;

        public SharePointCRUD ()
        {
            _parcelsFolderName = "4.3 Parcels";
            _siteUrl = "https://hdroneview.sharepoint.com/SF-CH-TS";

            // Method using Sharepoint Credentials
            //_ctx = new ClientContext(_siteUrl);
            //var passWord = new SecureString();
            //foreach (char c in "pwd".ToCharArray()) passWord.AppendChar(c);
            //_ctx.Credentials = new SharePointOnlineCredentials("<sharepoint_user>@hdrinc.com", passWord);

            // Method using AppID
            // Using OfficeDevPnp.Core
            // https://github.com/SharePoint/PnP-Sites-Core/blob/master/Core/README.md
            string _appId = "APPID";
            string _appSecret = "SECRET";
            AuthenticationManager authManager = new AuthenticationManager();
            _ctx = authManager.GetAppOnlyAuthenticatedContext(_siteUrl, _appId, _appSecret);
        }

        public string GetSiteTitle()
        {
            string title = "NA";
            Web web = _ctx.Web;

            _ctx.Load(web);

            _ctx.ExecuteQuery();

            Console.WriteLine(web.Title);

            if (!String.IsNullOrWhiteSpace(web.Title)) {
                title = web.Title;
            }

            return title;
        }

        // List Parcels
        public void GetFolders(string folderName)
        {
            Web web = _ctx.Web;
            List list = web.Lists.GetByTitle("Documents");
            string parcelsFolderName = "Documents/4.0 ROW/4.3 Parcels";
            string targetFolderPath = "Documents/4.0 ROW/4.3 Parcels/" + folderName;
            string folderTemplate = "Documents/4.0 ROW/4.3 Parcels/_Parcel No_LO Name";
            List <string> pathList = new List<string> { "4.0 ROW", "4.3 Parcels", folderName };
            Folder folder = web.GetFolderByServerRelativeUrl(parcelsFolderName);
            _ctx.Load(web);
            _ctx.Load(list);
            _ctx.Load(folder);
            _ctx.ExecuteQuery();

            Console.WriteLine("Folder contents: {0}", folder.ItemCount);

            if (folder.FolderExists(folderName))
            {
                Console.WriteLine("whoo-hoo");
            } else
            {
                //EnsureAndGetTargetFolder(_ctx, list, pathList);
                PasteFolder(folderTemplate, "Documents", parcelsFolderName, "Documents", folderName);
            }

            //_ctx.Load(list);
            //_ctx.Load(list.RootFolder);
            //_ctx.Load(list.RootFolder.Folders);
            //_ctx.Load(list.RootFolder.Files);
            //_ctx.ExecuteQuery();
            //FolderCollection fcol = list.RootFolder.Folders;
            //List<string> lstFile = new List<string>();
            //foreach (Folder f in fcol)
            //{
            //    if (f.Name == folderName)
            //    {
            //        //_ctx.Load(f.Files);
            //        //_ctx.ExecuteQuery();
            //        //FileCollection fileCol = f.Files;
            //        //foreach (File file in fileCol)
            //        //{
            //        //    lstFile.Add(file.Name);
            //        //}

            //        Console.WriteLine("Found {1}", folderName);
            //    }
            //}
        }

        public bool PasteFolder(string source, string sourceListName, string target, string targetListName, string targetFolderName)
        {
            bool isFolderPasted = false;
            try
            {
                using (var context = _ctx)
                {
                    //context.Credentials = credentials;     
                    var currentWeb = context.Web;
                    var addFoldername = targetFolderName;
                    var sourceFolder = currentWeb.GetFolderByServerRelativeUrl(source);
                    context.Load(sourceFolder.Folders);
                    context.Load(sourceFolder.Files);
                    context.ExecuteQuery();
                    Folder newfolder;

                    try
                    {
                        newfolder = currentWeb.GetFolderByServerRelativeUrl(target);
                        context.Load(newfolder);
                        context.ExecuteQuery();
                    }
                    catch
                    {
                        target = target.Replace("/", string.Empty).Trim();
                        var list = currentWeb.Lists.GetByTitle(target);
                        newfolder = list.RootFolder;
                        context.Load(newfolder);
                        context.ExecuteQuery();
                    }

                    var currentFolder = newfolder.Folders.Add(addFoldername);
                    context.Load(currentFolder);
                    context.Load(currentFolder.Folders);
                    context.Load(currentFolder.Files);
                    context.ExecuteQuery();

                    foreach (var file in sourceFolder.Files)
                    {
                        file.CopyTo(currentFolder.ServerRelativeUrl + "/" + file.Name, true);
                        context.ExecuteQuery();
                    }
                    foreach (var cFolder in sourceFolder.Folders)
                    {
                        PasteFolder(cFolder.ServerRelativeUrl, sourceListName, currentFolder.ServerRelativeUrl, targetListName, cFolder.Name);
                    }
                    isFolderPasted = true;
                }
            }
            catch (Exception ex)
            {
            }
            return isFolderPasted;
        }
        // List Parcels
        public ListCollection ListAllLists()
        {
            // Get folder list
            Web web = _ctx.Web;
            _ctx.Load(web.Lists,
                lists => lists.Include(list => list.Title,
                    list => list.Id));
            _ctx.ExecuteQuery();

            return web.Lists;
        }

        // Upload Parcel Doc
        public bool UploadParcelDoc(string pid, string docType, string docName, byte[] docBytes)
        {
            // Get Parcel folder list
            Web web = _ctx.Web;
            List parcelFolders = web.Lists.GetByTitle(_parcelsFolderName);

            // Check if Parcel & Doc Type Folder Exists
            List<string> targetPath = new List<string>() { pid, docType };
            Folder docFolder = EnsureAndGetTargetFolder(_ctx, parcelFolders, targetPath);
            
            // Check if Doc exists
            bool _docExists = DocExists(docFolder, docName);
            if (!_docExists)
            {
                _docExists = InsertDoc(docFolder, docName, docBytes);
            }
            return _docExists;
        }

        // Doc Exists
        public bool DocExists(Folder folder, string docName)
        {
            // Check if Doc exists
            bool _docExists = false;
            return _docExists;
        }

        // Insert Doc
        public bool InsertDoc(Folder folder, string docName, byte[] docBytes)
        {
            // Check if Doc exists
            bool _docExists = false;
            return _docExists;
        }

        /// <summary>
        /// Will ensure nested folder creation if folders in folderPath don't exist.
        /// </summary>
        /// <param name="ctx">Loaded SharePoint Client Context</param>
        /// <param name="list">Document Library SharePoint List Object</param>
        /// <param name="folderPath">List of strings ParentFolder, ChildFolder, ...</param>
        /// <returns>Last ChildFolder as target</returns>
        private static Folder EnsureAndGetTargetFolder(ClientContext ctx, List list, List<string> folderPath)
        {
            Folder returnFolder = list.RootFolder;
            if (folderPath != null && folderPath.Count > 0)
            {
                Web web = ctx.Web;
                Folder currentFolder = list.RootFolder;
                ctx.Load(web, t => t.Url);
                ctx.Load(currentFolder);
                ctx.ExecuteQuery();
                foreach (string folderPointer in folderPath)
                {
                    FolderCollection folders = currentFolder.Folders;
                    ctx.Load(folders);
                    ctx.ExecuteQuery();

                    bool folderFound = false;
                    foreach (Folder existingFolder in folders)
                    {
                        if (existingFolder.Name.Equals(folderPointer, StringComparison.InvariantCultureIgnoreCase))
                        {
                            folderFound = true;
                            currentFolder = existingFolder;
                            break;
                        }
                    }

                    if (!folderFound)
                    {
                        ListItemCreationInformation itemCreationInfo = new ListItemCreationInformation();
                        itemCreationInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
                        itemCreationInfo.LeafName = folderPointer;
                        itemCreationInfo.FolderUrl = currentFolder.ServerRelativeUrl;
                        ListItem folderItemCreated = list.AddItem(itemCreationInfo);
                        folderItemCreated.Update();
                        ctx.Load(folderItemCreated, f => f.Folder);
                        ctx.ExecuteQuery();
                        currentFolder = folderItemCreated.Folder;
                    }
                }
                returnFolder = currentFolder;
            }
            return returnFolder;
        }
    }
}
