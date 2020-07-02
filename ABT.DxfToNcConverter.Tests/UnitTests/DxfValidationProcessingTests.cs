using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using netDxf;
using netDxf.Entities;
using NUnit.Framework;

namespace ABT.DxfToNcConverter.Tests.UnitTests
{
    public class DxfValidationProcessingTests : SystemTestBase<DxfValidationProcessing>
    {
        protected override void SetupWorld(EcsWorld world)
        {
            this.world = world;

            dxfFileContentFilter = world.GetFilter<EcsFilter<DxfFileContent>>();
        }
        
        private EcsWorld world;
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
            var entity = world.NewEntity();
            entity.Get<DxfFileContent>().dfxDocument = document;
            
            System.Run();

            return dxfFileContentFilter.GetEntitiesCount();
        }

        [Test]
        [TestCase(2, false, ExpectedResult = 0)]
        [TestCase(2, true, ExpectedResult = 1)]
        [TestCase(3, false, ExpectedResult = 0)]
        [TestCase(3, true, ExpectedResult = 1)]
        public int IfCenterOfAllCirclesIsNotTheSameThenEntityMustBeDeleted(int circlesCount, bool centerIsTheSame)
        {
            var document = new DxfDocument();
            for (var i = 0; i < circlesCount; i++)
            {
                var coords = centerIsTheSame ? 1 : i;
                document.AddEntity(new Circle(new Vector2(coords, coords), 10));
            }
            var entity = world.NewEntity();
            entity.Get<DxfFileContent>().dfxDocument = document;
            
            System.Run();

            return dxfFileContentFilter.GetEntitiesCount();
        }

        [Test]
        [TestCase(true, ExpectedResult = 1)]
        [TestCase(false, ExpectedResult = 0)]
        public int IfAnyPolylineIsNotClosedThenEntityMustBeDeleted(bool isClosed)
        {
            var document = new DxfDocument();
            document.AddEntity(new Circle());
            document.AddEntity(new LwPolyline(new []{new LwPolylineVertex(), new LwPolylineVertex()}, isClosed));
            var entity = world.NewEntity();
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
            var entity = world.NewEntity();
            entity.Get<DxfFileContent>().dfxDocument = document;
            
            System.Run();

            return dxfFileContentFilter.GetEntitiesCount();
        }
    }
}