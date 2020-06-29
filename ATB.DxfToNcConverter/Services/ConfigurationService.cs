using System.Numerics;

namespace ATB.DxfToNcConverter.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(bool debug, bool whatIf, string workingDirectory, Vector2 endPoint, double holeDrillTime)
        {
            IsInDebugMode = debug;
            IsInWhatIfMode = whatIf;
            WorkingDirectory = workingDirectory;
            EndPoint = endPoint;
            HoleDrillTime = holeDrillTime;
        }
        
        public bool IsInDebugMode { get; }
        public bool IsInWhatIfMode { get; }
        public Vector2 EndPoint { get; }
        public double HoleDrillTime { get; }
        public string WorkingDirectory { get; }
    }
}