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
                entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                                 .WithCircle(200)
                                                                 .WithPolylines(GiveMe.DxfPolyline
                                                                                      .WithVertex(0, 150)
                                                                                      .WithVertex(150, 0)
                                                                                      .WithVertex(0, -150)
                                                                                      .WithVertex(-150, 0)
                                                                                      .Please()).Please();
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
            entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                             .WithCircle(200)
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 150)
                                                                                  .WithVertex(150, 0)
                                                                                  .WithVertex(0, -150)
                                                                                  .WithVertex(-150, 0)
                                                                                  .Please()).Please();
            
            System.Run();
            
            Assert.That(ncParametersFilter.Get1(0).endPointX, Is.EqualTo(230));
            Assert.That(ncParametersFilter.Get1(0).endPointY, Is.EqualTo(123));
        }

        [Test]
        public void StartPointMustBeEqualToBiggestCircleRadiusMinusStartingOffsetOnXAxisAndZeroOnYAxis()
        {
            ConfigurationServiceStub.StartPointXOffset = -143;
            var document = GiveMe.DxfDocument
                                 .WithCircle(200)
                                 .WithPolylines(GiveMe.DxfPolyline
                                                      .WithVertex(0, 150)
                                                      .WithVertex(150, 0)
                                                      .WithVertex(0, -150)
                                                      .WithVertex(-150, 0)
                                                      .Please()).Please();
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
            entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                             .WithCircle(200)
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 150)
                                                                                  .WithVertex(150, 0)
                                                                                  .WithVertex(0, -150)
                                                                                  .WithVertex(-150, 0)
                                                                                  .Please()).Please();
            
            System.Run();
            
            Assert.That(ncParametersFilter.Get1(0).drillParameters.Count(), Is.EqualTo(4));
        }

        [Test]
        public void YOffsetMustApplyCorrectly()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                             .WithCircle(200)
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 150)
                                                                                  .WithVertex(150, 0)
                                                                                  .WithVertex(0, -150)
                                                                                  .WithVertex(-150, 0)
                                                                                  .Please()).Please();
            
            System.Run();

            var drillParameters = ncParametersFilter.Get1(0).drillParameters.ToArray();
            Assert.That(drillParameters[0].offsetY, Is.EqualTo(0));
            Assert.That(drillParameters[1].offsetY, Is.EqualTo(90));
            Assert.That(drillParameters[2].offsetY, Is.EqualTo(90));
            Assert.That(drillParameters[3].offsetY, Is.EqualTo(90));
        }

        [Test]
        public void XOffsetMustApplyCorrectly()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                             .WithCircle(200)
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 140)
                                                                                  .WithVertex(130, 0)
                                                                                  .WithVertex(0, -150)
                                                                                  .WithVertex(-160, 0)
                                                                                  .Please()).Please();
            
            System.Run();
            
            var drillParameters = ncParametersFilter.Get1(0).drillParameters.ToArray();
            Assert.That(drillParameters[0].offsetX, Is.EqualTo(-60));
            Assert.That(drillParameters[1].offsetX, Is.EqualTo(-10));
            Assert.That(drillParameters[2].offsetX, Is.EqualTo(20));
            Assert.That(drillParameters[3].offsetX, Is.EqualTo(10));
        }

        [Test]
        public void ManyPolylinesMustApplyCorrectly()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                             .WithCircle(200)
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 150)
                                                                                  .WithVertex(150, 0)
                                                                                  .WithVertex(0, -150)
                                                                                  .WithVertex(-150, 0)
                                                                                  .Please(),
                                                                            GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 100)
                                                                                  .WithVertex(100, 0)
                                                                                  .WithVertex(0, -100)
                                                                                  .WithVertex(-100, 0)
                                                                                  .Please())
                                                             .Please();
            
            System.Run();

            var drillParameters = ncParametersFilter.Get1(0).drillParameters;
            Assert.That(drillParameters.Count(), Is.EqualTo(8));
        }

        [Test]
        public void EvenPolylinesMustApplyWithReversedSign()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                             .WithCircle(200)
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 150)
                                                                                  .WithVertex(150, 0)
                                                                                  .WithVertex(0, -150)
                                                                                  .WithVertex(-150, 0)
                                                                                  .Please(),
                                                                            GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 100)
                                                                                  .WithVertex(100, 0)
                                                                                  .WithVertex(0, -100)
                                                                                  .WithVertex(-100, 0)
                                                                                  .Please())
                                                             .Please();
            
            System.Run();

            var drillParameters = ncParametersFilter.Get1(0).drillParameters.ToArray();
            Assert.That(drillParameters.Take(4).Select(o => o.offsetY), Is.All.GreaterThanOrEqualTo(0));
            Assert.That(drillParameters.Skip(4).Select(o => o.offsetY), Is.All.LessThanOrEqualTo(0));
        }

        [Test]
        public void ClosestAngleMustApplyWithDifferentSign()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                             .WithCircle(200)
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 150)
                                                                                  .WithVertex(150, 0)
                                                                                  .WithVertex(-75, 75)
                                                                                  .WithVertex(150, 0)
                                                                                  .Please()).Please();
            
            System.Run();
            
            var drillParameters = ncParametersFilter.Get1(0).drillParameters.ToArray();
            Assert.That(drillParameters[0].offsetY, Is.EqualTo(0));
            Assert.That(drillParameters[1].offsetY, Is.EqualTo(90));
            Assert.That(drillParameters[2].offsetY, Is.EqualTo(-135));
            Assert.That(drillParameters[3].offsetY, Is.EqualTo(135));
        }

        [Test]
        public void ClosestAngleMustApplyWithDifferentSignForEvenPolyline()
        {
            var entity = World.NewEntity();
            entity.Get<DxfFileDefinition>().path = "C:\\tmp\\dxf_file.dxf";
            entity.Get<DxfFileContent>().dfxDocument = GiveMe.DxfDocument
                                                             .WithCircle(200)
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 20)
                                                                                  .WithVertex(0, 20)
                                                                                  .WithVertex(0, 20)
                                                                                  .WithVertex(0, 20)
                                                                                  .WithVertex(0, 20)
                                                                                  .Please())
                                                             .WithPolylines(GiveMe.DxfPolyline
                                                                                  .WithVertex(0, 150)
                                                                                  .WithVertex(150, 0)
                                                                                  .WithVertex(-75, 75)
                                                                                  .WithVertex(150, 0)
                                                                                  .Please()).Please();
            
            System.Run();
            
            var drillParameters = ncParametersFilter.Get1(0).drillParameters.Skip(5).ToArray();
            Assert.That(drillParameters[0].offsetY, Is.EqualTo(90));
            Assert.That(drillParameters[1].offsetY, Is.EqualTo(-135));
            Assert.That(drillParameters[2].offsetY, Is.EqualTo(135));
            Assert.That(drillParameters[3].offsetY, Is.EqualTo(-90));
        }
    }
}