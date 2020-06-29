using System.Collections.Generic;

namespace ATB.DxfToNcConverter.Services
{
    public interface IFileSystemService
    {
        IEnumerable<string> GetDxfFullFilePaths(string directoryToSearch);
        void SaveFileWithContent(string fullPath, string content);
    }
}