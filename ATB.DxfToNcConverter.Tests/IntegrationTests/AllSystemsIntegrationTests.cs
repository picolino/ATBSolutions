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
            var dxfDocument = CreateCorrectPlainDxfDocument();
            FileSystemServiceStub.FilesInWorkingDirectory.Add("C:\\tmp\\dxf_file.dxf");
            DxfServiceStub.DxfDocuments.Add("C:\\tmp\\dxf_file.dxf", dxfDocument);
            
            systems.Run();

            var ncExpected = GetTestFileContent("\\IntegrationTests\\TestData\\pdcc.nc");
            Assert.That(FileSystemServiceStub.SavedFiles["C:\\tmp\\dxf_file.nc"], Is.EqualTo(ncExpected));
        }
    }
}