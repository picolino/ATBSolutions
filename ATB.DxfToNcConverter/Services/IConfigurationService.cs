using System.Numerics;

namespace ATB.DxfToNcConverter.Services
{
    public interface IConfigurationService
    {
        bool IsInDebugMode { get; }
        bool IsInWhatIfMode { get; }
        Vector2 EndPoint { get; }
        double HoleDrillTime { get; }
    }
}