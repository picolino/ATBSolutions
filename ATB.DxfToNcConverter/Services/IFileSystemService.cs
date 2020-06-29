using System.Collections.Generic;

namespace ATB.DxfToNcConverter.Services
{
    public interface IFileSystemService
    {
        IEnumerable<string> GetDxfFullFilePaths();
        void SaveFileWithContent(string fullPath, string content);
    }
}