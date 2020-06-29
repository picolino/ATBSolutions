using System.Collections.Generic;
using System.Linq;
using ATB.DxfToNcConverter.Components;
using Leopotam.Ecs;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfParseProcessing : IEcsRunSystem
    {
        private readonly EcsFilter<DfxFileContent> dxfFileContentFilter = null;
        
        public void Run()
        {
            foreach (var dxfFileContentEntityId in dxfFileContentFilter)
            {
                ref var dxfFileContentComponent = ref dxfFileContentFilter.Get1(dxfFileContentEntityId);
                ref var dxfFileContentEntity = ref dxfFileContentFilter.GetEntity(dxfFileContentEntityId);
                var dxfDocument = dxfFileContentComponent.dfxDocument;
                
                var orderedCircles = dxfDocument.Circles.OrderByDescending(o => o.Radius).ToList();
                var orderPolylines = dxfDocument.LwPolylines.ToList();

                ref var ncParametersComponent = ref dxfFileContentEntity.Get<NcParameters>();
                ncParametersComponent.outerRadius = orderedCircles[0].Radius;
                ncParametersComponent.innerRadius = orderedCircles[^1].Radius;
                
                var drillParameters = new NcDrillParameters[orderPolylines.Count];

                for (var i = 0; i < drillParameters.Length; i++)
                {
                    drillParameters[i] = new NcDrillParameters
                                         {
                                             radius = 0d, // TODO: Add correct radius from orderPolylines
                                             stepAngle = 0d // TODO: Add correct step angle from orderPolylines
                                         };
                }

                ncParametersComponent.drillParameters = drillParameters;
            }
        }
    }
}