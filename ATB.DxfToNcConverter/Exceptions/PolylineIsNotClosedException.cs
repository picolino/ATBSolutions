using System;

namespace ATB.DxfToNcConverter.Exceptions
{
    public class PolylineIsNotClosedException : DxfParseExceptionBase
    {
        public PolylineIsNotClosedException(string dxfFilePath) : base($"All polylines in DXF file '{dxfFilePath}' must be closed.")
        {
        }
    }
}