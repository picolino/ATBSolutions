using System.Numerics;

namespace ATB.DxfToNcConverter.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(bool debug, bool whatIf, Vector2 endPoint)
        {
            IsInDebugMode = debug;
            IsInWhatIfMode = whatIf;
            EndPoint = endPoint;
        }
        
        public bool IsInDebugMode { get; }
        public bool IsInWhatIfMode { get; }
        public Vector2 EndPoint { get; }
    }
}