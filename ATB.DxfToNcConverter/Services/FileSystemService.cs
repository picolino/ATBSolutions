using System.Collections.Generic;
using System.IO;

namespace ATB.DxfToNcConverter.Services
{
    public class FileSystemService : IFileSystemService
    {
        public IEnumerable<string> GetDxfFullFilePaths()
        {
            return Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.dxf", SearchOption.TopDirectoryOnly);
        }
    }
}