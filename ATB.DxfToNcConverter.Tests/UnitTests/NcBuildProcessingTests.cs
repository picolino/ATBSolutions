using System.Linq;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Systems;
using Leopotam.Ecs;
using NUnit.Framework;

namespace ATB.DxfToNcConverter.Tests.UnitTests
{
    public class NcBuildProcessingTests : SystemTestBase<NcBuildProcessing>
    {
        private EcsFilter<NcProgram> ncProgramFilter;
        
        protected override void SetupFilters(EcsWorld world)
        {
            ncProgramFilter = world.GetFilter<EcsFilter<NcProgram>>();
        }

        [Test]
        public void ThirdRowIsStartPoint()
        {
            World.NewEntity().Get<NcParameters>() = BuildNcParameters();
            
            System.Run();
            
            Assert.That(ncProgramFilter.Get1(0).programText.Split("\r\n")[2], Is.EqualTo("X10Y10"));
        }

        [Test]
        public void ThirdRowFromTheEndIsEndPoint()
        {
            World.NewEntity().Get<NcParameters>() = BuildNcParameters();
            
            System.Run();
            
            Assert.That(ncProgramFilter.Get1(0).programText.Split("\r\n").Reverse().ToArray()[2], Is.EqualTo("G0X100Y100"));
        }

        private NcParameters BuildNcParameters()
        {
            return new NcParameters
                   {
                       startPointX = 10,
                       startPointY = 10,
                       endPointX = 100,
                       endPointY = 100,
                       drillParameters = new []
                                         {
                                             new NcDrillVertexParameters
                                             {
                                                 offsetX = -10,
                                                 offsetY = 5.1231,
                                                 drillTime = 1.5
                                             }, 
                                             new NcDrillVertexParameters
                                             {
                                                 offsetX = 0,
                                                 offsetY = 4.2315,
                                                 drillTime = 1.5
                                             }, 
                                             new NcDrillVertexParameters
                                             {
                                                 offsetX = -40,
                                                 offsetY = 10.2314,
                                                 drillTime = 1.5
                                             }, 
                                             new NcDrillVertexParameters
                                             {
                                                 offsetX = 0,
                                                 offsetY = 7.4184,
                                                 drillTime = 1.5
                                             }, 
                                         }
                   };
        }
    }
}