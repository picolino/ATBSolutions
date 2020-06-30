using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using netDxf;
using netDxf.Entities;
using NUnit.Framework;

namespace ABT.DxfToNcConverter.Tests.IntegrationTests
{
    public class AllSystemsIntegrationTests : TestBase
    {
        private EcsSystems systems;

        protected override void SetupSystems(EcsSystems systems)
        {
            systems.Add(new DxfSearchProcessing())
                   .Add(new DxfLoadProcessing())
                   .Add(new DxfValidationProcessing())
                   .Add(new DxfParseProcessing())
                   .Add(new NcBuildProcessing())
                   .Add(new NcSaveProcessing());
            
            this.systems = systems;
        }
        
        [Test]
        public void PlainDocumentConvertsCorrectly()
        {
            ConfigurationServiceStub.WorkingDirectory = "C:\\tmp\\";
            var dxfDocument = CreatePlainDxfDocument();
            FileSystemServiceStub.FilesInWorkingDirectory.Add("C:\\tmp\\dxf_file.dxf");
            DxfServiceStub.DxfDocuments.Add("C:\\tmp\\dxf_file.dxf", dxfDocument);
            
            systems.Run();

            Assert.That(FileSystemServiceStub.SavedFiles["C:\\tmp\\dxf_file.nc"], Is.EqualTo("")); // TODO: Fix test.
        }

        private DxfDocument CreatePlainDxfDocument()
        {
            var dxfDocument = new DxfDocument();
            
            dxfDocument.AddEntity(new Circle(Vector2.Zero, 200));
            dxfDocument.AddEntity(new Circle(Vector2.Zero, 100));
            
            var vertexes = new []
                           {
                               new LwPolylineVertex(0, 150), 
                               new LwPolylineVertex(150, 0), 
                               new LwPolylineVertex(0, -150), 
                               new LwPolylineVertex(-150, 0)
                           };
            
            var polyline = new LwPolyline(vertexes, true);

            dxfDocument.AddEntity(polyline);

            return dxfDocument;
        }
    }
}