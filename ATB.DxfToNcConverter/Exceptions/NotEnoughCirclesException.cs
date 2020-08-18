using System;
using ATB.DxfToNcConverter.Resources;

namespace ATB.DxfToNcConverter.Exceptions
{
    public class NotEnoughCirclesException : DxfParseExceptionBase
    {
        public NotEnoughCirclesException(string dxfFilePath) 
            : base(string.Format(Logging.NotEnoughCirclesErrorMessage, dxfFilePath))
        {
        }
    }
}