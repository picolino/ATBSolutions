namespace ATB.DxfToNcConverter.Services
{
    public interface IConfigurationService
    {
        bool IsInDebugMode { get; }
        bool IsInWhatIfMode { get; }
    }
}