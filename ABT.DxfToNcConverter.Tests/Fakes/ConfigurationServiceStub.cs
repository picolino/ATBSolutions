using System.Numerics;
using ATB.DxfToNcConverter.Services;

namespace ABT.DxfToNcConverter.Tests.Fakes
{
    public class ConfigurationServiceStub : IConfigurationService
    {
        public bool IsInDebugMode { get; set; }
        public bool IsInWhatIfMode { get; set; }
        public Vector2 EndPoint { get; set; }
        public double HoleDrillTime { get; set; }
        public string WorkingDirectory { get; set; }
    }
}