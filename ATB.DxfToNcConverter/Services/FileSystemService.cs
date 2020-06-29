using System.Collections.Generic;
using System.IO;

namespace ATB.DxfToNcConverter.Services
{
    public class FileSystemService : IFileSystemService
    {
        public IEnumerable<string> GetDxfFullFilePaths(string directoryToSearch)
        {
            return Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.dxf", SearchOption.TopDirectoryOnly);
        }

        public void SaveFileWithContent(string fullPath, string content)
        {
            File.WriteAllText(fullPath, content);
        }
    }
}