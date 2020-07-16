using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using netDxf;
using netDxf.Entities;
using NUnit.Framework;

namespace ATB.DxfToNcConverter.Tests.UnitTests
{
    public class DxfValidationProcessingTests : SystemTestBase<DxfValidationProcessing>
    {
        protected override void SetupFilters(EcsWorld world)
        {
            dxfFileContentFilter = world.GetFilter<EcsFilter<DxfFileContent>>();
        }
        
        private EcsFilter<DxfFileContent> dxfFileContentFilter;

        [Test]
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(1, ExpectedResult = 1)]
        [TestCase(2, ExpectedResult = 1)]
        public int IfCirclesCountLessOneThenEntityMustBeDeleted(int circlesCount)
        {
            var document = new DxfDocument();
            for (var i = 0; i < circlesCount; i++)
            {
                document.AddEntity(new Circle());
            }
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = document;
            
            System.Run();

            return dxfFileContentFilter.GetEntitiesCount();
        }

        [Test]
        [TestCase(true, ExpectedResult = 0)]
        [TestCase(false, ExpectedResult = 1)]
        public int IfAnyPolylineVertexIsOutsideTheBiggestCircleThenEntityMustBeDeleted(bool hasPolylineVertexOutsideTheBiggestCircle)
        {
            var document = new DxfDocument();
            document.AddEntity(new Circle(new Vector2(0, 0), 100));
            var polyline = new LwPolyline(new[] {new LwPolylineVertex(10, 10), new LwPolylineVertex(20, 20)}, true);
            if (hasPolylineVertexOutsideTheBiggestCircle)
            {
                polyline.Vertexes.Add(new LwPolylineVertex(200, 300));
            }
            document.AddEntity(polyline);
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = document;
            
            System.Run();

            return dxfFileContentFilter.GetEntitiesCount();
        }
    }
}