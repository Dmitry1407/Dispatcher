using System;
using System.ComponentModel;

using Core.service.propertygrid;

namespace Core.model.design.graphics.shape
{
    public class Rectangle : Shape
    {
        [SortedCategory("Geometry", 1, 10), PropertyOrder(2)]
        [DisplayName("Width")]
        [Description("Width")]
        public Int32 Width { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(3)]
        [DisplayName("Height")]
        [Description("Height")]
        public Int32 Height { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(4)]
        [DisplayName("Round")]
        [Description("Round")]
        public Int32 Round { get; set; }

        public Rectangle(Int32 x, Int32 y, Int32 width, Int32 height)
            : this(x, y, width, height, 0) { }

        public Rectangle(Int32 x, Int32 y, Int32 width, Int32 height, Int32 round)
        {
            SType = ShapeType.Rectangle;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Round = round;
        }
    }
}
