using netDxf;

namespace ATB.DxfToNcConverter.Services
{
    public interface IDxfService
    {
        DxfDocument LoadDxfDocument(string filePath);
    }
}