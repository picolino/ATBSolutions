using System;
using System.Linq;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Exceptions;
using Leopotam.Ecs;
using netDxf;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfValidationProcessing : IEcsRunSystem
    {
        private readonly EcsFilter<DxfFileContent> dxfFileContentFilter = null;
        
        public void Run()
        {
            foreach (var dxfFileContentEntityId in dxfFileContentFilter)
            {
                ref var dxfFileContentEntity = ref dxfFileContentFilter.GetEntity(dxfFileContentEntityId);
                
                try
                {
                    ref var dxfFileContentComponent = ref dxfFileContentFilter.Get1(dxfFileContentEntityId);
                    var dxfDocument = dxfFileContentComponent.dfxDocument;

                    var circles = dxfDocument.Circles.ToArray();
                    var polylines = dxfDocument.LwPolylines.ToArray();
                    
                    if (circles.Length < 1)
                    {
                        throw new NotEnoughCirclesException();
                    }

                    var biggestCircle = circles.OrderByDescending(o => o.Radius).First();

                    if (polylines.Any(o => !o.IsClosed))
                    {
                        throw new PolylineIsNotClosedException();
                    }

                    if (polylines.Any(o => o.Vertexes.Any(o1 => 
                                                              !IsPointInsideTheCircle(biggestCircle.Center.X, 
                                                                                       biggestCircle.Center.Y, 
                                                                                       biggestCircle.Radius, 
                                                                                       o1.Position))))
                    {
                        throw new PolylineVertexIsOutsideOfTheBiggestCircleException();
                    }
                }
                catch (Exception e)
                {
                    switch (e)
                    {
                        case NotEnoughCirclesException _:
                            // TODO: Write in log
                            break;
                        case PolylineIsNotClosedException _:
                            // TODO: Write in log
                            break;
                        case PolylineVertexIsOutsideOfTheBiggestCircleException _:
                            // TODO: Write in log
                            break;
                    }
                    
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