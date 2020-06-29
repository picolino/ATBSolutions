using System;
using System.Collections.Generic;
using System.Linq;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;
using netDxf;
using netDxf.Entities;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfParseProcessing : IEcsRunSystem
    {
        private readonly IConfigurationService configurationService = null;
        private readonly EcsFilter<DfxFileContent> dxfFileContentFilter = null;
        
        public void Run()
        {
            foreach (var dxfFileContentEntityId in dxfFileContentFilter)
            {
                ref var dxfFileContentComponent = ref dxfFileContentFilter.Get1(dxfFileContentEntityId);
                ref var dxfFileContentEntity = ref dxfFileContentFilter.GetEntity(dxfFileContentEntityId);
                var dxfDocument = dxfFileContentComponent.dfxDocument;

                var polylines = dxfDocument.LwPolylines.ToArray();
                var biggestCircle = dxfDocument.Circles.OrderByDescending(o => o.Radius).First();

                var biggestCircleRadius = biggestCircle.Radius;
                var biggestCircleCenter2d = new Vector2(biggestCircle.Center.X, biggestCircle.Center.Y);
                var biggestCircleZeroXAxis2d = new Vector2(0, biggestCircleRadius);

                ref var ncParametersComponent = ref dxfFileContentEntity.Get<NcParameters>();
                ncParametersComponent.endPointX = configurationService.EndPoint.X;
                ncParametersComponent.endPointY = configurationService.EndPoint.Y;

                ncParametersComponent.startPointX = biggestCircleRadius;
                ncParametersComponent.startPointY = 0;
                
                var drillParameters = new List<NcDrillVertexParameters>();

                var offsetXAccumulator = 0d;
                var offsetYAccumulator = 0d;
                
                foreach (var polyline in polylines)
                {
                    foreach (var vertex in polyline.Vertexes)
                    {
                        var circleCenterToVertexPositionVector = vertex.Position - biggestCircleCenter2d;
                        var circleCenterToVertexPositionDistance = circleCenterToVertexPositionVector.Modulus();
                        var signMinus = circleCenterToVertexPositionVector.X > biggestCircleZeroXAxis2d.X;
                        
                        var angle = Rad2Deg(Vector2.AngleBetween(biggestCircleZeroXAxis2d, circleCenterToVertexPositionVector));

                        if (signMinus)
                        {
                            angle = 360 - angle;
                        }

                        if (angle < offsetYAccumulator)
                        {
                            offsetYAccumulator = 360 - offsetYAccumulator;
                        }
                        
                        var offsetX = RoundDefault(ncParametersComponent.startPointX - circleCenterToVertexPositionDistance - offsetXAccumulator);
                        var offsetY = RoundDefault(angle - offsetYAccumulator);
                        
                        offsetXAccumulator += offsetX;
                        offsetYAccumulator += offsetY;
                        
                        drillParameters.Add(new NcDrillVertexParameters
                                            {
                                                offsetX = offsetX,
                                                offsetY = offsetY, 
                                                drillTime = configurationService.HoleDrillTime
                                            });
                    }
                }
                
                ncParametersComponent.drillParameters = drillParameters;
            }
        }

        private static double RoundDefault(double value)
        {
            return Math.Round(value, 4, MidpointRounding.AwayFromZero);
        }

        private static double Rad2Deg(double rad)
        {
            return rad * 180 / Math.PI;
        }
    }
}