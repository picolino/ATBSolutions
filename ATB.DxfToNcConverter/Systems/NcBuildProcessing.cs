using System;
using System.Globalization;
using System.Text;
using System.Threading;
using ATB.DxfToNcConverter.Components;
using Leopotam.Ecs;
using NLog;

namespace ATB.DxfToNcConverter.Systems
{
    public class NcBuildProcessing : IEcsRunSystem
    {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly EcsFilter<NcParameters> ncParametersFilter = null;
        
        public void Run()
        {
            logger.Info("Building NC programs...");
            
            foreach (var ncParametersEntityId in ncParametersFilter)
            {
                ref var ncParametersEntity = ref ncParametersFilter.GetEntity(ncParametersEntityId);
                ref var ncParametersComponent = ref ncParametersFilter.Get1(ncParametersEntityId);

                var currentThreadCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; // For correction floating point separator
                
                var ncStringBuilder = new StringBuilder();

                ncStringBuilder.AppendLine("G0G90");
                ncStringBuilder.AppendLine();
                ncStringBuilder.AppendLine($"X{ncParametersComponent.startPointX:0.####}Y{ncParametersComponent.startPointY:0.####}");
                ncStringBuilder.AppendLine();
                ncStringBuilder.AppendLine("G1G91F3000");
                ncStringBuilder.AppendLine("G4P1");
                ncStringBuilder.AppendLine();

                foreach (var vertexDrillParameter in ncParametersComponent.drillParameters)
                {
                    if (vertexDrillParameter.offsetX != 0)
                    {
                        var sign = Math.Sign(vertexDrillParameter.offsetX) > 0 ? "+" : string.Empty;
                        ncStringBuilder.AppendLine($"X{sign}{vertexDrillParameter.offsetX:0.####}");
                    }

                    if (vertexDrillParameter.offsetY != 0)
                    {
                        var sign = Math.Sign(vertexDrillParameter.offsetY) > 0 ? "+" : string.Empty;
                        ncStringBuilder.AppendLine($"Y{sign}{vertexDrillParameter.offsetY:0.####}");
                    }
                    
                    ncStringBuilder.AppendLine($"M3G4P{vertexDrillParameter.drillTime:0.##}");
                    ncStringBuilder.AppendLine("M5");
                    
                    ncStringBuilder.AppendLine();
                }
                
                ncStringBuilder.AppendLine("G90");
                ncStringBuilder.AppendLine($"G0X{ncParametersComponent.endPointX:0.####}Y{ncParametersComponent.endPointY:0.####}");
                ncStringBuilder.AppendLine("M30");

                ref var ncProgramComponent = ref ncParametersEntity.Get<NcProgram>();
                ncProgramComponent.programText = ncStringBuilder.ToString();
                
                Thread.CurrentThread.CurrentCulture = currentThreadCulture; // Reset culture
            }
        }
    }
}