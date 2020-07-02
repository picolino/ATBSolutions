using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using netDxf;
using NUnit.Framework;

namespace ATB.DxfToNcConverter.Tests.UnitTests
{
    public class DxfLoadProcessingTests : SystemTestBase<DxfLoadProcessing>
    {
        protected override void SetupWorld(EcsWorld world)
        {
            this.world = world;
            dxfFileContentFilter = world.GetFilter<EcsFilter<DxfFileContent>>();
        }

        private EcsWorld world;
        private EcsFilter<DxfFileContent> dxfFileContentFilter;

        [Test]
        public void DxfFileContentComponentAreCreated()
        {
            var file1Entity = world.NewEntity();
            file1Entity.Get<DxfFileDefinition>().path = "C:\\tmp\\file1.dxf";
            var file2Entity = world.NewEntity();
            file2Entity.Get<DxfFileDefinition>().path = "C:\\tmp\\file2.dxf";
            var documents = new[] {new DxfDocument(), new DxfDocument()};
            DxfServiceStub.DxfDocuments.Add("C:\\tmp\\file1.dxf", documents[0]);
            DxfServiceStub.DxfDocuments.Add("C:\\tmp\\file2.dxf", documents[1]);
            
            System.Run();

            foreach (var idx in dxfFileContentFilter)
            {
                ref var dxfFileContentComponent = ref dxfFileContentFilter.Get1(idx);
                Assert.That(dxfFileContentComponent.dfxDocument, Is.EqualTo(documents[idx]));
            }
        }
    }
}