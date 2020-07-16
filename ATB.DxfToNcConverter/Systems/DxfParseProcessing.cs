using System;
using System.Collections.Generic;
using System.Linq;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;
using netDxf;
using NLog;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfParseProcessing : IEcsRunSystem
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfigurationService configurationService = null;
        private readonly EcsFilter<DxfFileDefinition, DxfFileContent> dxfFileContentFilter = null;
        
        public void Run()
        {
            if (dxfFileContentFilter.IsEmpty())
            {
                return;
            }
            
            logger.Info("Parsing DXF files...");
            
            foreach (var dxfFileContentEntityId in dxfFileContentFilter)
            {
                ref var dxfFileDefinitionComponent = ref dxfFileContentFilter.Get1(dxfFileContentEntityId);
                ref var dxfFileContentComponent = ref dxfFileContentFilter.Get2(dxfFileContentEntityId);
                ref var dxfFileContentEntity = ref dxfFileContentFilter.GetEntity(dxfFileContentEntityId);
                
                logger.Debug($"Parsing DXF file: '{dxfFileDefinitionComponent.path}'...");
                
                var dxfDocument = dxfFileContentComponent.dfxDocument;

                var biggestCircle = dxfDocument.Circles.OrderByDescending(o => o.Radius).First();
                
                var biggestCircleRadius = biggestCircle.Radius;
                var biggestCircleCenter2d = new Vector2(biggestCircle.Center.X, biggestCircle.Center.Y);
                
                logger.Debug($"Biggest circle: Handle: '{biggestCircle.Handle}' Center: '{biggestCircleCenter2d}', Radius: '{biggestCircleRadius}'.");

                ref var ncParametersComponent = ref dxfFileContentEntity.Get<NcParameters>();

                ncParametersComponent.startPointX = biggestCircleRadius + configurationService.StartPointXOffset;
                ncParametersComponent.startPointY = 0;
                
                logger.Debug($"Start point: X: '{ncParametersComponent.startPointX}', Y: '{ncParametersComponent.startPointY}'.");
                
                ncParametersComponent.endPointX = configurationService.EndPoint.X;
                ncParametersComponent.endPointY = configurationService.EndPoint.Y;
                
                logger.Debug($"End point: X: '{ncParametersComponent.endPointX}', Y: '{ncParametersComponent.endPointY}'.");
                
                var drillParameters = new List<NcDrillVertexParameters>();
                var existingVertexPositions = new List<Vector2>();

                var previousXPosition = ncParametersComponent.startPointX;
                var previousVertexVector = Vector2.UnitY;
                
                logger.Debug($"Parsing polylines...");

                var polylines = dxfDocument.LwPolylines.OrderByDescending(o => o.Vertexes.Count);

                var polylineCounter = 0;
                
                foreach (var polyline in polylines)
                {
                    logger.Debug($"Parsing polyline: Handle: '{polyline.Handle}'. Vertexes count: {polyline.Vertexes.Count}");
                    
                    polylineCounter++;
                    var reverseOrder = polylineCounter % 2 == 0;

                    var vertexes = reverseOrder
                                       ? polyline.Vertexes.AsEnumerable().Reverse().ToList()
                                       : polyline.Vertexes;
                    
                    foreach (var vertex in vertexes)
                    {
                        if (existingVertexPositions.Contains(vertex.Position))
                        {
                            logger.Debug($"Vertex at position {vertex.Position} already exists. Passing...");
                            continue;
                        }
                        
                        var circleCenterToVertexPositionVector = vertex.Position - biggestCircleCenter2d;
                        var circleCenterToVertexPositionDistance = circleCenterToVertexPositionVector.Modulus() + configurationService.StartPointXOffset;

                        var angle = Rad2Deg(SignedAngleBetween(previousVertexVector, circleCenterToVertexPositionVector));
                        
                        var offsetX = RoundDefault(circleCenterToVertexPositionDistance - previousXPosition);
                        var offsetY = RoundDefault(angle);
                        
                        previousVertexVector = circleCenterToVertexPositionVector;
                        previousXPosition += offsetX;

                        var drillTime = polyline.Color.IsByLayer
                                            ? configurationService.HoleDrillTime
                                            : configurationService.FastenersDrillTime;

                        var drillVertex = new NcDrillVertexParameters
                                          {
                                              offsetX = offsetX,
                                              offsetY = offsetY,
                                              drillTime = drillTime
                                          };
                        
                        logger.Debug($"Drill vertex: X offset: '{drillVertex.offsetX}', Y offset: '{drillVertex.offsetY}', Drill time: '{drillVertex.drillTime}'.");
                        
                        drillParameters.Add(drillVertex);
                        existingVertexPositions.Add(vertex.Position);
                    }
                }
                
                ncParametersComponent.drillParameters = drillParameters;
                
                logger.Debug($"Parsing DXF file '{dxfFileDefinitionComponent.path}' complete.");
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