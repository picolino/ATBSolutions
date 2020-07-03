using System;
using System.Linq;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Exceptions;
using Leopotam.Ecs;
using netDxf;
using NLog;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfValidationProcessing : IEcsRunSystem
    {
        private readonly Action<Exception> onFileNotValidAction;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly EcsFilter<DxfFileDefinition, DxfFileContent> dxfFileContentFilter = null;

        public DxfValidationProcessing() : this(null)
        {
        }
        
        public DxfValidationProcessing(Action<Exception> onFileNotValidAction)
        {
            this.onFileNotValidAction = onFileNotValidAction;
        }
        
        public void Run()
        {
            if (dxfFileContentFilter.IsEmpty())
            {
                return;
            }
            
            logger.Info($"Validating DXF files...");
            
            foreach (var dxfFileContentEntityId in dxfFileContentFilter)
            {
                ref var dxfFileContentEntity = ref dxfFileContentFilter.GetEntity(dxfFileContentEntityId);
                ref var dxfFileDefinitionComponent = ref dxfFileContentFilter.Get1(dxfFileContentEntityId);
                
                try
                {
                    ref var dxfFileContentComponent = ref dxfFileContentFilter.Get2(dxfFileContentEntityId);
                    
                    var dxfDocument = dxfFileContentComponent.dfxDocument;

                    var circles = dxfDocument.Circles.ToArray();
                    var polylines = dxfDocument.LwPolylines.ToArray();
                    
                    if (circles.Length < 1)
                    {
                        throw new NotEnoughCirclesException(dxfFileDefinitionComponent.path);
                    }

                    var biggestCircle = circles.OrderByDescending(o => o.Radius).First();

                    if (polylines.Any(o => !o.IsClosed))
                    {
                        throw new PolylineIsNotClosedException(dxfFileDefinitionComponent.path);
                    }

                    if (polylines.Any(o => o.Vertexes.Any(o1 => 
                                                              !IsPointInsideTheCircle(biggestCircle.Center.X, 
                                                                                       biggestCircle.Center.Y, 
                                                                                       biggestCircle.Radius, 
                                                                                       o1.Position))))
                    {
                        throw new PolylineVertexIsOutsideOfTheBiggestCircleException(dxfFileDefinitionComponent.path);
                    }
                    
                    logger.Debug($"DXF file {dxfFileDefinitionComponent.path} is valid.");
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    onFileNotValidAction?.Invoke(e);
                    dxfFileContentEntity.Destroy();
                }
            }
        }

        private bool IsPointInsideTheCircle(double circleX, double circleY, double circleRadius, Vector2 point)
        {
            return (point.X - circleX) * (point.X - circleX) + 
                   (point.Y - circleY) * (point.Y - circleY)    <= circleRadius * circleRadius;
        }
    }
}