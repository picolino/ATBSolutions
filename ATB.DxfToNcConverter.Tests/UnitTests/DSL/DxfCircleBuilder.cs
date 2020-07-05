using netDxf;
using netDxf.Entities;

namespace ATB.DxfToNcConverter.Tests.UnitTests.DSL
{
    public class DxfCircleBuilder
    {
        private double radius = 100;
        
        public DxfCircleBuilder WithRadius(double radius)
        {
            this.radius = radius;
            return this;
        }

        public Circle Please()
        {
            return new Circle(Vector2.Zero, radius);
        }
    }
}