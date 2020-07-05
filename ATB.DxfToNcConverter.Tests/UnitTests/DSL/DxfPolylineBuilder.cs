using System.Collections.Generic;
using netDxf.Entities;

namespace ATB.DxfToNcConverter.Tests.UnitTests.DSL
{
    public class DxfPolylineBuilder
    {
        private readonly List<LwPolylineVertex> vertexes = new List<LwPolylineVertex>();
        
        public DxfPolylineBuilder WithVertex(double x, double y)
        {
            vertexes.Add(new LwPolylineVertex(x, y));
            return this;
        }

        public LwPolyline Please()
        {
            return new LwPolyline(vertexes, true);
        }
    }
}