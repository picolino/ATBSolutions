using netDxf;

namespace ATB.DxfToNcConverter.Services
{
    public class DxfService : IDxfService
    {
        public DxfDocument LoadDxfDocument(string filePath)
        {
            return DxfDocument.Load(filePath);
        }
    }
}