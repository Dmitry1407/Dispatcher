using System;

namespace Core.model.design.graphics.control
{
    public class Trend : Control
    {
        public Trend(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            CType = ControlType.Trend;
            this.X = x;
            this.Y = y;
            Width = width;
            Height = height;
        }
    }
}
