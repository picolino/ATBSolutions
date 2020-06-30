using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using NUnit.Framework;

namespace ABT.DxfToNcConverter.Tests.UnitTests
{
    public class DxfSearchProcessingTests : SystemTestBase<DxfSearchProcessing>
    {
        protected override void SetupWorld(EcsWorld world)
        {
            dxfFileDefinitionFilter = world.GetFilter<EcsFilter<DfxFileDefinition>>();
        }

        private EcsFilter<DfxFileDefinition> dxfFileDefinitionFilter;

        [Test]
        public void DxfFileDefinitionComponentsAreCreatedForEachFile()
        {
            FileSystemServiceStub.FilesInWorkingDirectory.Add("C:\\tmp\\file1.dxf");
            FileSystemServiceStub.FilesInWorkingDirectory.Add("C:\\tmp\\file2.dxf");
            
            System.Run();

            Assert.That(dxfFileDefinitionFilter.GetEntitiesCount(), Is.EqualTo(2));
        }
    }
}