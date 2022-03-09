using System;

namespace Core.model.design.graphics.control
{
    public class Journal : Control
    {
        public Journal(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            CType = ControlType.Journal;
            this.X = x;
            this.Y = y;
            Width = width;
            Height = height;
        }
    }
}
