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
            ConfigurationServiceStub.EndPoint = new Vector2(230, 123);
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = CreateCorrectPlainDxfDocument();
            
            System.Run();
            
            Assert.That(ncParametersFilter.Get1(0).endPointX, Is.EqualTo(230));
            Assert.That(ncParametersFilter.Get1(0).endPointY, Is.EqualTo(123));
        }

        [Test]
        public void StartPointMustBeEqualToBiggestCircleRadiusOnXAxisAndZeroOnYAxis()
        {
            var document = CreateCorrectPlainDxfDocument();
            document.Circles.First().Radius = 412;
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = document;
            
            System.Run();
            
            Assert.That(ncParametersFilter.Get1(0).startPointX, Is.EqualTo(412));
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
    }
}