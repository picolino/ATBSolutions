namespace ATB.DxfToNcConverter.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(bool debug, bool whatIf)
        {
            IsInDebugMode = debug;
            IsInWhatIfMode = whatIf;
        }
        
        public bool IsInDebugMode { get; }
        public bool IsInWhatIfMode { get; }
    }
}