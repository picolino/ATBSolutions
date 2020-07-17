using System;
using System.Collections.Generic;
using System.Linq;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Resources;
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
            
            logger.Info(Logging.ParsingDxfFiles);
            
            foreach (var dxfFileContentEntityId in dxfFileContentFilter)
            {
                ref var dxfFileDefinitionComponent = ref dxfFileContentFilter.Get1(dxfFileContentEntityId);
                ref var dxfFileContentComponent = ref dxfFileContentFilter.Get2(dxfFileContentEntityId);
                ref var dxfFileContentEntity = ref dxfFileContentFilter.GetEntity(dxfFileContentEntityId);
                
                logger.Debug(string.Format(Logging.ParsingDxfFile, dxfFileDefinitionComponent.path));
                
                var dxfDocument = dxfFileContentComponent.dfxDocument;

                var biggestCircle = dxfDocument.Circles.OrderByDescending(o => o.Radius).First();
                
                var biggestCircleRadius = biggestCircle.Radius;
                var biggestCircleCenter2d = new Vector2(biggestCircle.Center.X, biggestCircle.Center.Y);
                
                logger.Debug(string.Format(Logging.BiggestCircle, biggestCircle.Handle, biggestCircleCenter2d, biggestCircleRadius));

                ref var ncParametersComponent = ref dxfFileContentEntity.Get<NcParameters>();

                ncParametersComponent.startPointX = biggestCircleRadius + configurationService.StartPointXOffset;
                ncParametersComponent.startPointY = 0;
                
                logger.Debug(string.Format(Logging.StartPointX, ncParametersComponent.startPointX, ncParametersComponent.startPointY));
                
                ncParametersComponent.endPointX = configurationService.EndPoint.X;
                ncParametersComponent.endPointY = configurationService.EndPoint.Y;
                
                logger.Debug(string.Format(Logging.EndPointX, ncParametersComponent.endPointX, ncParametersComponent.endPointY));
                
                var drillParameters = new List<NcDrillVertexParameters>();
                var existingVertexPositions = new List<Vector2>();

                var previousXPosition = ncParametersComponent.startPointX;
                var previousVertexVector = Vector2.UnitY;
                
                logger.Debug(Logging.ParsingPolylines);

                var polylines = dxfDocument.LwPolylines.OrderByDescending(o => o.Vertexes.Count);

                var polylineCounter = 0;
                
                foreach (var polyline in polylines)
                {
                    logger.Debug(string.Format(Logging.ParsingPolyline, polyline.Handle, polyline.Vertexes.Count));
                    
                    polylineCounter++;
                    var reverseOrder = polylineCounter % 2 == 0;

                    var vertexes = reverseOrder
                                       ? polyline.Vertexes.AsEnumerable().Reverse().ToList()
                                       : polyline.Vertexes;
                    
                    foreach (var vertex in vertexes)
                    {
                        if (existingVertexPositions.Contains(vertex.Position))
                        {
                            logger.Debug(string.Format(Logging.VertexExists, vertex.Position));
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
                        
                        logger.Debug(string.Format(Logging.DrillVertex, drillVertex.offsetX, drillVertex.offsetY, drillVertex.drillTime));
                        
                        drillParameters.Add(drillVertex);
                        existingVertexPositions.Add(vertex.Position);
                    }
                }
                
                ncParametersComponent.drillParameters = drillParameters;
                
                logger.Debug(string.Format(Logging.ParsingDxfFileComplete, dxfFileDefinitionComponent.path));
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