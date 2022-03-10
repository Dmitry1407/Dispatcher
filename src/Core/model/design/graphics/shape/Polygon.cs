using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Core.model.design.graphics.shape
{
    public class Polygon : Shape
    {
        [Browsable(false)]
        public IList<Point> Points { get; set; }

        public Polygon() : this(0, 0) { }

        public Polygon(Int32 x, Int32 y)
        {
            SType = ShapeType.Polygon;
            Points = new List<Point>();
            X = x;
            Y = y;
        }

        public void AddPoint(Int32 x, Int32 y)
        {
            Points.Add(new Point(x, y));
        }
    }
}
