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
        private readonly EcsFilter<DxfFileContent> dxfFileContentFilter = null;
        
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
                var previousVertexVector = Vector2.UnitY;

                foreach (var polyline in polylines)
                {
                    foreach (var vertex in polyline.Vertexes)
                    {
                        var circleCenterToVertexPositionVector = vertex.Position - biggestCircleCenter2d;
                        var circleCenterToVertexPositionDistance = circleCenterToVertexPositionVector.Modulus();
                        
                        var angle = Rad2Deg(SignedAngleBetween(previousVertexVector, circleCenterToVertexPositionVector));
                        previousVertexVector = circleCenterToVertexPositionVector;
                        
                        var offsetX = RoundDefault(ncParametersComponent.startPointX - circleCenterToVertexPositionDistance - offsetXAccumulator);
                        var offsetY = RoundDefault(angle);
                        
                        offsetXAccumulator += offsetX;
                        
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

        private static double SignedAngleBetween(Vector2 u, Vector2 v)
        {
            var angle = Math.Atan2(u.Y, u.X) - Math.Atan2(v.Y, v.X);
            
            if (angle > Math.PI)
            {
                angle -= 2 * Math.PI;
            }
            else if (angle <= -Math.PI)
            {
                angle += 2 * Math.PI;
            }

            return angle;
        }
    }
}