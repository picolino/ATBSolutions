using ATB.DxfToNcConverter.Components;
using ATB.DxfToNcConverter.Services;
using Leopotam.Ecs;

namespace ATB.DxfToNcConverter.Systems
{
    public class DxfLoadProcessing : IEcsRunSystem
    {
        private readonly EcsFilter<DfxFileDefinition> filter = null;

        private readonly IDxfService dxfService = null;
        
        public void Run()
        {
            foreach (var dxfFullFilePathEntityId in filter)
            {
                ref var dxfFullFilePathComponent = ref filter.Get1(dxfFullFilePathEntityId);
                ref var dxfEntity = ref filter.GetEntity(dxfFullFilePathEntityId);

                ref var dfxFileContent = ref dxfEntity.Get<DfxFileContent>();
                dfxFileContent.dfxDocument = dxfService.LoadDxfDocument(dxfFullFilePathComponent.path);
            }
        }
    }
}