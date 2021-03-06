﻿using System.Numerics;
using ATB.DxfToNcConverter.Services;

namespace ATB.DxfToNcConverter.Tests.Fakes
{
    public class ConfigurationServiceStub : IConfigurationService
    {
        public bool IsInDebugMode { get; set; }
        public bool IsInWhatIfMode { get; set; }
        public netDxf.Vector2 EndPoint { get; set; }
        public double HoleDrillTime { get; set; }
        public double FastenersDrillTime { get; set; }
        public string WorkingDirectory { get; set; }
        public double StartPointXOffset { get; set; }
    }
}