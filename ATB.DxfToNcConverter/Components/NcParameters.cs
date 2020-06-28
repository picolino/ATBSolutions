using System.Collections.Generic;
using netDxf.Entities;

namespace ATB.DxfToNcConverter.Components
{
    public struct NcParameters
    {
        public double outerRadius;
        public double innerRadius;

        public IEnumerable<NcDrillParameters> drillParameters;
    }
}