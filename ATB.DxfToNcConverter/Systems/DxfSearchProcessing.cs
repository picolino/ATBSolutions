using System;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Resources;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;
using NLog;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfSearchProcessing : IEcsRunSystem
    {
        private readonly Action noDxfFilesInWorkingDirectoryAction;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly EcsWorld world = null;
        private readonly EcsFilter<DxfFileDefinition> dxfFileDefinitionFilter = null;
        private readonly IFileSystemService fileSystemService = null;
        private readonly IConfigurationService configurationService = null;

        public DxfSearchProcessing() : this(null)
        {
        }
        
        public DxfSearchProcessing(Action noDxfFilesInWorkingDirectoryAction)
        {
            this.noDxfFilesInWorkingDirectoryAction = noDxfFilesInWorkingDirectoryAction;
        }
        
        public void Run()
        {
            logger.Info(Logging.SearchingDxfFiles);

            var directoryToSearchDxfFiles = configurationService.WorkingDirectory;
            
            logger.Debug(string.Format(Logging.SearchDirectory, directoryToSearchDxfFiles));
            
            foreach (var dxfFullFilePath in fileSystemService.GetDxfFullFilePaths(directoryToSearchDxfFiles))
            {
                var dxfFullFilePathEntity = world.NewEntity();
                ref var dfxFullFilePathComponent = ref dxfFullFilePathEntity.Get<DxfFileDefinition>();
                dfxFullFilePathComponent.path = dxfFullFilePath;
                
                logger.Debug(string.Format(Logging.FoundDxfFile, dxfFullFilePath));
            }

            if (dxfFileDefinitionFilter.IsEmpty())
            {
                logger.Warn(Logging.NoDxfFilesFound);
                noDxfFilesInWorkingDirectoryAction?.Invoke();
            }
        }
    }
}