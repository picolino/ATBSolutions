using System;

namespace ATB.DxfToNcConverter.Exceptions
{
    public class DxfParseExceptionBase : Exception
    {
        protected DxfParseExceptionBase(string message) : base(message)
        {
        }
    }
}