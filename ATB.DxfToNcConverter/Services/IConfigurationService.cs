namespace ATB.DxfToNcConverter.Services
{
    public interface IConfigurationService
    {
        bool IsInDebugMode { get; }
        bool IsInWhatIfMode { get; }
        netDxf.Vector2 EndPoint { get; }
        double HoleDrillTime { get; }
        string WorkingDirectory { get; }
        double StartPointXOffset { get; }
    }
}