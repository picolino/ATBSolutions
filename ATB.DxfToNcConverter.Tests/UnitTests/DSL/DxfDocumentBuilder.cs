using System.Collections.Generic;
using netDxf;
using netDxf.Entities;

namespace ATB.DxfToNcConverter.Tests.UnitTests.DSL
{
    public class DxfDocumentBuilder
    {
        private readonly List<Circle> circles = new List<Circle>();
        private readonly List<LwPolyline> polylines = new List<LwPolyline>();
        
        public DxfDocumentBuilder WithCircle(double radius)
        {
            circles.Add(new Circle(Vector2.Zero, radius));
            return this;
        }

        public DxfDocumentBuilder WithPolylines(params LwPolyline[] polyline)
        {
            polylines.AddRange(polyline);
            return this;
        }

        public DxfDocument Please()
        {
            var document = new DxfDocument();

            document.AddEntity(circles);
            document.AddEntity(polylines);
            
            return document;
        }
    }
}