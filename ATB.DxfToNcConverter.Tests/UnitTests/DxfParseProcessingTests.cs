using System.Linq;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using NUnit.Framework;
using Vector2 = System.Numerics.Vector2;

namespace ATB.DxfToNcConverter.Tests.UnitTests
{
    public class DxfParseProcessingTests : SystemTestBase<DxfParseProcessing>
    {
        private EcsFilter<NcParameters> ncParametersFilter;

        protected override void SetupFilters(EcsWorld world)
        {
            ncParametersFilter = world.GetFilter<EcsFilter<NcParameters>>();
        }

        [Test]
        [TestCase(1, ExpectedResult = 1)]
        [TestCase(4, ExpectedResult = 4)]
        public int ForEachEntityNcParametersComponentMustBeAdded(int entitiesCount)
        {
            for (var i = 0; i < entitiesCount; i++)
            {
                var entity = World.NewEntity();
                entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
                entity.Get<DxfFileContent>().dfxDocument = CreateCorrectPlainDxfDocument();
            }
            
            System.Run();

            return ncParametersFilter.GetEntitiesCount();
        }

        [Test]
        public void EndPointMustBeEqualToEndPointFromConfigurationService()
        {
            ConfigurationServiceStub.EndPoint = new netDxf.Vector2(230, 123);
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = CreateCorrectPlainDxfDocument();
            
            System.Run();
            
            Assert.That(ncParametersFilter.Get1(0).endPointX, Is.EqualTo(230));
            Assert.That(ncParametersFilter.Get1(0).endPointY, Is.EqualTo(123));
        }

        [Test]
        public void StartPointMustBeEqualToBiggestCircleRadiusMinusStartingOffsetOnXAxisAndZeroOnYAxis()
        {
            ConfigurationServiceStub.StartPointXOffset = -143;
            var document = CreateCorrectPlainDxfDocument();
            document.Circles.First().Radius = 412;
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = document;
            
            System.Run();
            
            Assert.That(ncParametersFilter.Get1(0).startPointX, Is.EqualTo(412-143));
            Assert.That(ncParametersFilter.Get1(0).startPointY, Is.EqualTo(0));
        }

        [Test]
        public void CountOfDrillVertexesMustBeEqualToPolylineVertexes()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = CreateCorrectPlainDxfDocument();
            
            System.Run();
            
            Assert.That(ncParametersFilter.Get1(0).drillParameters.Count(), Is.EqualTo(4));
        }

        [Test]
        public void AngleBetweenVertexesMustBeCorrect()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = CreateCorrectPlainDxfDocument();
            
            System.Run();

            var drillParameters = ncParametersFilter.Get1(0).drillParameters.ToArray();
            Assert.That(drillParameters[0].offsetY, Is.EqualTo(0));
            Assert.That(drillParameters[1].offsetY, Is.EqualTo(90));
            Assert.That(drillParameters[2].offsetY, Is.EqualTo(90));
            Assert.That(drillParameters[3].offsetY, Is.EqualTo(90));
        }
    }
}