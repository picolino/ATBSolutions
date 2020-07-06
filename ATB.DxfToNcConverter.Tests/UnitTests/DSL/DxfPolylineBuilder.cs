using System;
using System.Collections.Generic;
using netDxf.Entities;

namespace ATB.DxfToNcConverter.Tests.UnitTests.DSL
{
    public class DxfPolylineBuilder
    {
        private readonly List<LwPolylineVertex> vertexes = new List<LwPolylineVertex>();

        public DxfPolylineBuilder AutoBuildByAngleAndRadius(double angle, double radius)
        {
            var fullAngle = 0d;

            while (fullAngle - 360 < -0.001)
            {
                var rad = fullAngle * Math.PI / 180;
                WithVertex(radius * Math.Sin(rad), radius * Math.Cos(rad));
                fullAngle += angle;
            }

            return this;
        }
        
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