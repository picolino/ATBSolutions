using System.Collections.Generic;
using ATB.DxfToNcConverter.Services;
using netDxf;

namespace ATB.DxfToNcConverter.Tests.Fakes
{
    public class DxfServiceStub : IDxfService
    {
        public Dictionary<string, DxfDocument> DxfDocuments { get; set; } = new Dictionary<string, DxfDocument>();
        
        public DxfDocument LoadDxfDocument(string filePath)
        {
            return DxfDocuments[filePath];
        }
    }
}