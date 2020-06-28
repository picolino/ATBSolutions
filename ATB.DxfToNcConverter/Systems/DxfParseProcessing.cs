using System.Collections.Generic;
using System.Linq;
using ATB.DxfToNcConverter.Components;
using Leopotam.Ecs;
using netDxf;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfParseProcessing : IEcsRunSystem
    {
        private readonly EcsWorld world = null;
        private readonly EcsFilter<DfxFullFilePath> filter = null;
        
        public void Run()
        {
            foreach (var dxfFullFilePathEntityId in filter)
            {
                ref var dxfFullFilePathComponent = ref filter.Get1(dxfFullFilePathEntityId);

                var document = DxfDocument.Load(dxfFullFilePathComponent.path);

                var orderedCircles = document.Circles.OrderByDescending(o => o.Radius).ToList();

                var ncParametersEntity = world.NewEntity();
                ref var ncParametersComponent = ref ncParametersEntity.Get<NcParameters>();
                ncParametersComponent.outerRadius = orderedCircles[0].Radius;
                ncParametersComponent.innerRadius = orderedCircles[^1].Radius;
                
                var drillParameters = new List<NcDrillParameters>();

                for (var i = 1; i < orderedCircles.Count - 1; i++)
                {
                    var circle = orderedCircles[i];
                    drillParameters.Add(new NcDrillParameters
                                        {
                                            radius = circle.Radius,
                                            stepAngle = 0d // TODO: Add correct step angle
                                        });
                }

                ncParametersComponent.drillParameters = drillParameters;
            }
        }
    }
}