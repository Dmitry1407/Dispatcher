using System;
using System.ComponentModel;

using Core.service.propertygrid;

namespace Core.model.design.graphics.shape
{
    public class Picture : Shape
    {
        [SortedCategory("Object", 0, 10), PropertyOrder(3)]
        [DisplayName("FileName")]
        [Description("Picture File Name")]
        public String FileName { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(2)]
        [DisplayName("Width")]
        [Description("Picture Width")]
        public Int32 Width { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(3)]
        [DisplayName("Height")]
        [Description("Picture Height")]
        public Int32 Height { get; set; }

        public Picture()
        {
            SType = ShapeType.Picture;
        }
    }
}
