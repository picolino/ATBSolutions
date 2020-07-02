using System.IO;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;
using NLog;

namespace ATB.DxfToNcConverter.Systems
{
    public class NcSaveProcessing : IEcsRunSystem
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly IFileSystemService fileSystemService = null;
        private readonly IConfigurationService configurationService = null;
        private readonly EcsFilter<DxfFileDefinition, NcProgram> filter = null;
        
        public void Run()
        {
            foreach (var idx in filter)
            {
                ref var dfxFileDefinitionComponent = ref filter.Get1(idx);
                ref var ncProgramComponent = ref filter.Get2(idx);

                var fileName = Path.GetFileNameWithoutExtension(dfxFileDefinitionComponent.path) + ".nc";
                
                fileSystemService.SaveFileWithContent(Path.Combine(configurationService.WorkingDirectory, fileName), ncProgramComponent.programText);
            }
        }
    }
}