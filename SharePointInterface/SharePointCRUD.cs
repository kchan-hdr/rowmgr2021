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
    public class SharePointCRUD
    {
        private ClientContext _ctx;
        private string _parcelsFolderName;
        private string _siteUrl;

        public SharePointCRUD (string _appId, string _appSecret)
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
            //string _appId = "APPID";
            //string _appSecret = "SECRET";

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

        // List Parcel Folders
        public Folder GetFolder(string folderName)
        {
            Web web = _ctx.Web;
            List list = web.Lists.GetByTitle("Documents");
            string baseFolderName = "Documents/4.0 ROW/4.3 Parcels";
            string targetFolderPath = "Documents/4.0 ROW/4.3 Parcels/" + folderName;
            string folderTemplate = "Documents/4.0 ROW/4.3 Parcels/_Parcel No_LO Name";
            List <string> pathList = new List<string> { "4.0 ROW", "4.3 Parcels", folderName };
            Folder baseFolder = web.GetFolderByServerRelativeUrl(baseFolderName);
            _ctx.Load(web);
            _ctx.Load(list);
            _ctx.Load(baseFolder);
            _ctx.ExecuteQuery();

            if (baseFolder.FolderExists(folderName))
            {
                Console.WriteLine("Folder {0} exists in {1}", folderName, baseFolderName);
            } else {
                //EnsureAndGetTargetFolder(_ctx, list, pathList);
                CopyPasteFolder(folderTemplate, "Documents", baseFolderName, "Documents", folderName);
            }

            Folder newFolder = web.GetFolderByServerRelativeUrl(targetFolderPath);
            _ctx.Load(newFolder);
            _ctx.ExecuteQuery();

            return newFolder;
        }


        // List All Lists
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

        /// <summary>
        /// Recursively copy and paste nested folders creation if folders in target folderPath don't exist.
        /// </summary>
        /// <param name="source">Source Folder Path</param>
        /// <param name="sourceListName">Source Document Library SharePoint List Object</param>
        /// <param name="target">Target Folder Path</param>
        /// <param name="targetListName">Targe Document Libarary SharePoint List Object</param>
        /// <param name="targetFolderName">Target Base Folder Name</param>
        /// <returns>bool success</returns>
        public bool CopyPasteFolder(string source, string sourceListName, string target, string targetListName, string targetFolderName)
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
                        CopyPasteFolder(cFolder.ServerRelativeUrl, sourceListName, currentFolder.ServerRelativeUrl, targetListName, cFolder.Name);
                    }
                    isFolderPasted = true;
                }
            }
            catch (Exception ex)
            {
            }
            return isFolderPasted;
        }
    }
}
