using System;

namespace ATB.DxfToNcConverter.Exceptions
{
    public class NotEnoughCirclesException : DxfParseExceptionBase
    {
        public NotEnoughCirclesException(string dxfFilePath) : base($"DXF file '{dxfFilePath}' must contain at least one circle element.")
        {
        }
    }
}