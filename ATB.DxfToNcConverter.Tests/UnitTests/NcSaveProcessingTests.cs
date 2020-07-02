using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using NUnit.Framework;

namespace ATB.DxfToNcConverter.Tests.UnitTests
{
    public class NcSaveProcessingTests : SystemTestBase<NcSaveProcessing>
    {
        protected override void BeforeInitializeSystems(EcsSystems systems)
        {
            ConfigurationServiceStub.WorkingDirectory = "C:\\tmp\\";
        }

        [Test]
        public void SavedFileNameEqualToSourceDxfFileNameAndExtensionIsNc()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<NcProgram>().programText = "NCPROGRAMTEXT";
            
            System.Run();
            
            Assert.DoesNotThrow(() => { var _ = FileSystemServiceStub.SavedFiles["C:\\tmp\\dxf_file.nc"];});
        }

        [Test]
        public void SavedFileConsistsOfNcProgramText()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<NcProgram>().programText = "NCPROGRAMTEXT";
            
            System.Run();
            
            Assert.That(FileSystemServiceStub.SavedFiles["C:\\tmp\\dxf_file.nc"], Is.EqualTo("NCPROGRAMTEXT"));
        }
    }
}