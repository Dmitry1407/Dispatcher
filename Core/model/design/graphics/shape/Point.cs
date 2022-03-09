using System;

namespace Core.model.design.graphics.shape
{
    public class Point
    {
        public Int32 X { get; set; }
        public Int32 Y { get; set; }

        public Point() : this(0, 0) { }

        public Point(Int32 x, Int32 y)
        {
            X = x;
            Y = y;
        }
    }
}
