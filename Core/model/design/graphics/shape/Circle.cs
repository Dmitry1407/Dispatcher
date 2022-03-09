using System;
using System.ComponentModel;

using Core.service.propertygrid;

namespace Core.model.design.graphics.shape
{
    public class Circle : Shape
    {
        [SortedCategory("Geometry", 1, 10), PropertyOrder(2)]
        [DisplayName("Radius")]
        [Description("Radius")]
        public Int32 Radius { get; set; }

        public Circle(Int32 x, Int32 y, Int32 radius)
        {
            SType = ShapeType.Circle;
            X = x;
            Y = y;
            Radius = radius;
        }
    }
}
