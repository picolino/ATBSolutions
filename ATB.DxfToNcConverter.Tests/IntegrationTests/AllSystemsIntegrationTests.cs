using System.IO;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using netDxf;
using netDxf.Entities;
using NUnit.Framework;

namespace ATB.DxfToNcConverter.Tests.IntegrationTests
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
            var dxfDocument = GiveMe.DxfDocument
                                    .WithCircle(200)
                                    .WithPolylines(GiveMe.DxfPolyline
                                                         .WithVertex(0, 150)
                                                         .WithVertex(150, 0)
                                                         .WithVertex(0, -150)
                                                         .WithVertex(-150, 0)
                                                         .Please()).Please();
            FileSystemServiceStub.FilesInWorkingDirectory.Add("C:\\tmp\\dxf_file.dxf");
            DxfServiceStub.DxfDocuments.Add("C:\\tmp\\dxf_file.dxf", dxfDocument);
            
            systems.Run();

            var ncExpected = GetTestFileContent("\\IntegrationTests\\TestData\\pdcc.nc");
            Assert.That(FileSystemServiceStub.SavedFiles["C:\\tmp\\dxf_file.nc"], Is.EqualTo(ncExpected));
        }
    }
}