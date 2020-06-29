using System.Text;
using ATB.DxfToNcConverter.Components;
using Leopotam.Ecs;

namespace ATB.DxfToNcConverter.Systems
{
    public class NcBuildProcessing : IEcsRunSystem
    {
        private readonly EcsFilter<NcParameters> ncParametersFilter = null;
        
        public void Run()
        {
            foreach (var ncParametersEntityId in ncParametersFilter)
            {
                ref var ncParametersEntity = ref ncParametersFilter.GetEntity(ncParametersEntityId);
                ref var ncParametersComponent = ref ncParametersFilter.Get1(ncParametersEntityId);
                
                var ncStringBuilder = new StringBuilder();
                
                // TODO: Build NC programs using NC parameters

                ref var ncProgramComponent = ref ncParametersEntity.Get<NcProgram>();
                ncProgramComponent.programText = ncStringBuilder.ToString();
            }
        }
    }
}