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
    public interface ISharePointCRUD
    {
        string GetSiteTitle();
        Folder GetOrCreateFolder(string folderName, string baseFolderName, string folderTemplate);
        ListCollection ListAllLists();
        bool UploadParcelDoc(string pid, string docType, string docName, byte[] docBytes, string baseFolderName);
        System.IO.Stream GetParcelDoc(string pid, string docType, string docName, string baseFolderName);
        string GetParcelFolderName(string pid);
        List<string> GetDocTargetPath(string baseFolderName, string parcelFolderName, string docType);
        bool DocExists(Folder folder, string docName);
        bool InsertDoc(Folder folder, string docName, byte[] docBytes);
        bool CopyPasteFolder(string source, string sourceListName, string target, string targetListName, string targetFolderName);
    }

    public class SharePointCRUD : ISharePointCRUD
    {
        private ClientContext _ctx;
        private string _parcelsFolderName;
        private string _parcelsFolderTemplate;
        private string _siteUrl;
        private Dictionary<string, string> _docTypes;

        public SharePointCRUD (string _appId = null, string _appSecret = null, Dictionary<string,string> docTypes = null)
        {
            _parcelsFolderName = "4.0 ROW/4.3 Parcels";
            _siteUrl = "https://hdroneview.sharepoint.com/SF-CH-TS";
            _parcelsFolderTemplate = "Documents/4.0 ROW/4.3 Parcels/_Parcel No_LO Name";
            if (docTypes == null)
            {
                _docTypes = new Dictionary<string, string>();
                _docTypes.Add("0", "4.3.1 ROE/1 ROE Working");
                _docTypes.Add("1", "4.3.1 ROE/2 ROE QC");
                _docTypes.Add("2", "4.3.1 ROE/3 Final Sent to LO");
                _docTypes.Add("3", "4.3.1 ROE/4 Signed");

            } else
            {
                _docTypes = docTypes;
            }

            if (_appId == null || _appSecret == null )
            {
                _appId = "a6fad0e8-3e1f-42eb-89f2-6cb8e1dcb329";
                _appSecret = "FMMJTzMMkP8CZOsL1IP3JvSoVWAOrF90zGxKVmUc2tc=";

            }

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
        public Folder GetOrCreateFolder(string folderName, string baseFolderName = "", string folderTemplate = "")
        {
            if (String.IsNullOrWhiteSpace(baseFolderName))
            {
                baseFolderName = "Documents/" + _parcelsFolderName;
            }
            if (String.IsNullOrWhiteSpace(folderTemplate))
            {
                folderTemplate = _parcelsFolderTemplate;
            }

            Web web = _ctx.Web;
            List list = web.Lists.GetByTitle("Documents");

            string targetFolderPath = String.Format("{0}/{1}", baseFolderName, folderName);
            //List <string> pathList = new List<string> { "4.0 ROW", "4.3 Parcels", folderName };
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
        public bool UploadParcelDoc(string pid, string docType, string docName, byte[] docBytes, string baseFolderName = "")
        {

            if (String.IsNullOrWhiteSpace(baseFolderName))
            {
                baseFolderName = _parcelsFolderName;
            }

            // Get Parcel folder list
            Web web = _ctx.Web;
            List parcelFolders = web.Lists.GetByTitle("Documents");

            // Get Parcel Folder Name
            string parcelFolderName = GetParcelFolderName(pid);

            // Ensure parcel folder structure exists
            Folder parcelFolder = GetOrCreateFolder(parcelFolderName);


            // Check if Parcel & Doc Type Folder Exists
            List<string> targetPath = GetDocTargetPath(baseFolderName, parcelFolderName, docType);
            Folder docFolder = EnsureAndGetTargetFolder(_ctx, parcelFolders, targetPath);
            
            // Check if Doc exists
            bool docExists = DocExists(docFolder, docName);
            if (!docExists)
            {
                docExists = InsertDoc(docFolder, docName, docBytes);
            }
            return docExists;
        }

        public System.IO.Stream GetParcelDoc(string pid, string docType, string docName, string baseFolderName = "")
        {
            if (String.IsNullOrWhiteSpace(baseFolderName))
            {
                baseFolderName = _parcelsFolderName;
            }

            // Get Parcel folder list
            Web web = _ctx.Web;
            List parcelFolders = web.Lists.GetByTitle("Documents");

            // Get Parcel Folder Name
            string parcelFolderName = GetParcelFolderName(pid);

            // Ensure parcel folder structure exists
            Folder parcelFolder = GetOrCreateFolder(parcelFolderName);

            // Check if Parcel & Doc Type Folder Exists
            List<string> targetPath = GetDocTargetPath(baseFolderName, parcelFolderName, docType);
            Folder docFolder = EnsureAndGetTargetFolder(_ctx, parcelFolders, targetPath);

            File doc = docFolder.GetFile(docName);
            _ctx.Load(doc);
            ClientResult<System.IO.Stream> fileStream = doc.OpenBinaryStream();
            _ctx.ExecuteQuery();

            return fileStream.Value;
        }

        public string GetParcelFolderName(string pid)
        {
            // Change to lookup if necessary

            // remove sharepoint reserved chars
            // https://support.microsoft.com/en-us/help/2933738/restrictions-and-limitations-when-you-sync-sharepoint-libraries-to-you
            // ~, \, /, :, *, ?, ", <, >, | , # , %
            // 400 Char length limit
            return pid;
        }

        public List<string> GetDocTargetPath(string baseFolderName, string parcelFolderName, string docType)
        {
            string doctypePath = docType;

            if (string.IsNullOrWhiteSpace(docType))
            {
                docType = _docTypes.Keys.First<string>();
            }
            // Lookup doctype path
            if (_docTypes.TryGetValue(docType, out string val))
            {
                doctypePath = val;
            }

            doctypePath = String.Format("{0}/{1}/{2}", baseFolderName, parcelFolderName, doctypePath);
            return doctypePath.Split('/').ToList();
        }

        // Doc Exists
        public bool DocExists(Folder folder, string docName)
        {
            bool _docExists = false;
            // Check if Doc exists
            File doc = folder.GetFile(docName);
            _docExists = (doc != null);

            return _docExists;
        }

        // Insert Doc
        public bool InsertDoc(Folder folder, string docName, byte[] docBytes)
        {
            // Check if Doc exists
            bool _docExists = false;

            try
            {
                var info = new FileCreationInformation
                {
                    Content = docBytes,
                    Overwrite = false,
                    Url = String.Format("{0}/{1}", folder.ServerRelativeUrl, docName),
                };

                File file = folder.Files.Add(info);
                folder.Update();
                _ctx.Load(file, f => f.ListItemAllFields);
                _ctx.ExecuteQuery();
                _ctx.Load(file);

                ListItem item = file.ListItemAllFields;
                item["Title"] = "Title";
                item.Update();
                _ctx.ExecuteQuery();

                _docExists = true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Uploading Doc Failed: {0}", e.Message);
            }
    
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
