﻿using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Resources;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;
using NLog;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfLoadProcessing : IEcsRunSystem
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly EcsFilter<DxfFileDefinition> filter = null;

        private readonly IDxfService dxfService = null;
        
        public void Run()
        {
            if (filter.IsEmpty())
            {
                return;
            }
            
            logger.Info(Logging.LoadingDxfFiles);
            
            foreach (var dxfFullFilePathEntityId in filter)
            {
                ref var dxfFullFilePathComponent = ref filter.Get1(dxfFullFilePathEntityId);
                ref var dxfEntity = ref filter.GetEntity(dxfFullFilePathEntityId);

                ref var dfxFileContent = ref dxfEntity.Get<DxfFileContent>();
                dfxFileContent.dfxDocument = dxfService.LoadDxfDocument(dxfFullFilePathComponent.path);
                
                logger.Debug(string.Format(Logging.DxfFileLoaded, dxfFullFilePathComponent.path));
            }
        }
    }
}