using System;
using ATB.DxfToNcConverter.Resources;

namespace ATB.DxfToNcConverter.Exceptions
{
    public class PolylineVertexIsOutsideOfTheBiggestCircleException : DxfParseExceptionBase
    {
        public PolylineVertexIsOutsideOfTheBiggestCircleException(string dxfFilePath) 
            : base(string.Format(Logging.PolylineVertexIsOutsideOfTheBiggestCircleErrorMessage, dxfFilePath))
        {
        }
    }
}