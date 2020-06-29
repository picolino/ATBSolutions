using System.IO;
using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;

namespace ATB.DxfToNcConverter.Systems
{
    public class NcSaveProcessing : IEcsRunSystem
    {
        private readonly IFileSystemService fileSystemService = null;
        private readonly EcsFilter<DfxFileDefinition, NcProgram> filter = null;
        
        public void Run()
        {
            foreach (var idx in filter)
            {
                ref var dfxFileDefinitionComponent = ref filter.Get1(idx);
                ref var ncProgramComponent = ref filter.Get2(idx);

                var fileName = Path.GetFileName(dfxFileDefinitionComponent.path) + ".nc";
                
                fileSystemService.SaveFileWithContent(Path.Combine(Directory.GetCurrentDirectory(), fileName), ncProgramComponent.programText);
            }
        }
    }
}