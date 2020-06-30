using System.Collections.Generic;
using System.Linq;
using ATB.DxfToNcConverter.Services;

namespace ABT.DxfToNcConverter.Tests.Fakes
{
    public class FileSystemServiceStub : IFileSystemService
    {
        public List<string> FilesInWorkingDirectory { get; set; } = new List<string>();
        
        public IEnumerable<string> GetDxfFullFilePaths(string directoryToSearch)
        {
            return FilesInWorkingDirectory;
        }
        
        public Dictionary<string, string> SavedFiles { get; set; } = new Dictionary<string, string>();

        public void SaveFileWithContent(string fullPath, string content)
        {
            SavedFiles.Add(fullPath, content);
        }
    }
}