using System;
using System.ComponentModel;

using Core.model.core.channel;
using Core.service.propertygrid;

namespace Core.model.design.graphics.shape
{
    public class Line : Shape
    {
        [SortedCategory("Geometry", 1, 10), PropertyOrder(2)]
        [DisplayName("DX")]
        [Description("Delta X")]
        public Int32 DX { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(3)]
        [DisplayName("DY")]
        [Description("Delta Y")]
        public Int32 DY { get; set; }

        public Line() : this(10, 10, 50, 0) { }

        public Line(Int32 x, Int32 y, Int32 dX, Int32 dY)
        {
            SType = ShapeType.Line;
            X = x;
            Y = y;
            DX = dX;
            DY = dY;
        }
    }
}
