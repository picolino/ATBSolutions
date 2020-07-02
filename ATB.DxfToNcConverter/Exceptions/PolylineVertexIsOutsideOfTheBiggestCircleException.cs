using System;

namespace ATB.DxfToNcConverter.Exceptions
{
    public class PolylineVertexIsOutsideOfTheBiggestCircleException : DxfParseExceptionBase
    {
        public PolylineVertexIsOutsideOfTheBiggestCircleException(string dxfFilePath) : base($"All polylines in DXF file '{dxfFilePath}' must be placed inside the biggest circle.")
        {
        }
    }
}