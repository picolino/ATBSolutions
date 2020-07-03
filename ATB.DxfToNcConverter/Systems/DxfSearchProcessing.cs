using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;
using NLog;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfSearchProcessing : IEcsRunSystem
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly EcsWorld world = null;
        private readonly EcsFilter<DxfFileDefinition> dxfFileDefinitionFilter = null;
        private readonly IFileSystemService fileSystemService = null;
        private readonly IConfigurationService configurationService = null;
        
        public void Run()
        {
            logger.Info($"Searching DXF files...");

            var directoryToSearchDxfFiles = configurationService.WorkingDirectory;
            
            logger.Debug($"Search directory: '{directoryToSearchDxfFiles}.");
            
            foreach (var dxfFullFilePath in fileSystemService.GetDxfFullFilePaths(directoryToSearchDxfFiles))
            {
                var dxfFullFilePathEntity = world.NewEntity();
                ref var dfxFullFilePathComponent = ref dxfFullFilePathEntity.Get<DxfFileDefinition>();
                dfxFullFilePathComponent.path = dxfFullFilePath;
                
                logger.Debug($"Found DXF file: '{dxfFullFilePath}'.");
            }

            if (dxfFileDefinitionFilter.IsEmpty())
            {
                logger.Warn($"There are no *.dxf files in working directory.");
            }
        }
    }
}