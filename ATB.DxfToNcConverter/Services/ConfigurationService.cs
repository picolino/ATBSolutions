namespace ATB.DxfToNcConverter.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(bool debug, bool whatIf, string workingDirectory, netDxf.Vector2 endPoint, double holeDrillTime, double startPointXOffset)
        {
            IsInDebugMode = debug;
            IsInWhatIfMode = whatIf;
            WorkingDirectory = workingDirectory;
            EndPoint = endPoint;
            HoleDrillTime = holeDrillTime;
            StartPointXOffset = startPointXOffset;
        }
        
        public bool IsInDebugMode { get; }
        public bool IsInWhatIfMode { get; }
        public netDxf.Vector2 EndPoint { get; }
        public double HoleDrillTime { get; }
        public double StartPointXOffset { get; }
        public string WorkingDirectory { get; }
    }
}