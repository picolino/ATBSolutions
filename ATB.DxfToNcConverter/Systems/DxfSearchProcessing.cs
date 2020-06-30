using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfSearchProcessing : IEcsRunSystem
    {
        private readonly EcsWorld world = null;
        private readonly IFileSystemService fileSystemService = null;
        private readonly IConfigurationService configurationService = null;
        
        public void Run()
        {
            foreach (var dxfFullFilePath in fileSystemService.GetDxfFullFilePaths(configurationService.WorkingDirectory))
            {
                var dxfFullFilePathEntity = world.NewEntity();
                ref var dfxFullFilePathComponent = ref dxfFullFilePathEntity.Get<DxfFileDefinition>();
                dfxFullFilePathComponent.path = dxfFullFilePath;
            }
        }
    }
}