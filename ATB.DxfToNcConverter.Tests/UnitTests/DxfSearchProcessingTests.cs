using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using NUnit.Framework;

namespace ATB.DxfToNcConverter.Tests.UnitTests
{
    public class DxfSearchProcessingTests : SystemTestBase<DxfSearchProcessing>
    {
        protected override void SetupWorld(EcsWorld world)
        {
            dxfFileDefinitionFilter = world.GetFilter<EcsFilter<DxfFileDefinition>>();
        }

        private EcsFilter<DxfFileDefinition> dxfFileDefinitionFilter;

        [Test]
        public void DxfFileDefinitionComponentsAreCreatedForEachFile()
        {
            FileSystemServiceStub.FilesInWorkingDirectory.Add("C:\\tmp\\file1.dxf");
            FileSystemServiceStub.FilesInWorkingDirectory.Add("C:\\tmp\\file2.dxf");
            
            System.Run();

            Assert.That(dxfFileDefinitionFilter.GetEntitiesCount(), Is.EqualTo(2));
            foreach (var idx in dxfFileDefinitionFilter)
            {
                ref var dxfFileDefinition = ref dxfFileDefinitionFilter.Get1(idx);
                Assert.That(dxfFileDefinition.path, Is.EqualTo(FileSystemServiceStub.FilesInWorkingDirectory[idx]));
            }
        }
    }
}