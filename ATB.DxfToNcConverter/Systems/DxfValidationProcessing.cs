using System;
using System.Linq;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Exceptions;
using Leopotam.Ecs;

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
                    
                    if (circles.Length < 2)
                    {
                        throw new NotEnoughCirclesException();
                    }

                    var centerPoint = circles[0].Center;
                    if (circles.Any(o => o.Center != centerPoint))
                    {
                        throw new CenterOfCirclesIsNotTheSameException();
                    }

                    if (polylines.Any(o => !o.IsClosed))
                    {
                        throw new PolylineIsNotClosedException();
                    }
                }
                catch (Exception e)
                {
                    switch (e)
                    {
                        case NotEnoughCirclesException _:
                            // TODO: Write in log
                            break;
                        case CenterOfCirclesIsNotTheSameException _:
                            // TODO: Write in log
                            break;
                        case PolylineIsNotClosedException _:
                            // TODO: Write in log
                            break;
                    }
                    
                    dxfFileContentEntity.Destroy();
                }
            }
        }
    }
}