using System.Collections.Generic;

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
        string GetParcelFolderURL(string pid, string baseFolderName);
    }

    #region placeholder classes
    public interface Folder { }
    public interface ListCollection { }
    #endregion
}
