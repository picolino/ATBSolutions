﻿namespace ATB.DxfToNcConverter.Services
{
    public interface IConfigurationService
    {
        bool IsInDebugMode { get; }
        bool IsInWhatIfMode { get; }
        netDxf.Vector2 EndPoint { get; }
        double HoleDrillTime { get; }
        double FastenersDrillTime { get; }
        string WorkingDirectory { get; }
        double StartPointXOffset { get; }
    }
}