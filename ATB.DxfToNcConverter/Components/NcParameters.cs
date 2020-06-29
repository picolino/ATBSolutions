using System.Collections.Generic;
using netDxf.Entities;

namespace ATB.DxfToNcConverter.Components
{
    public struct NcParameters
    {
        public double startPointX;
        public double startPointY;

        public double endPointX;
        public double endPointY;

        public IEnumerable<NcDrillVertexParameters> drillParameters;
    }
}