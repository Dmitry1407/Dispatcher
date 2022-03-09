using System;
using System.Text;
using System.ComponentModel;

using Core.service.graphics;
using Core.service.propertygrid;

namespace Core.model.design.graphics.shape
{
    public class Text : Shape
    {
        [SortedCategory("Object", 0, 10), PropertyOrder(2)]
        [DisplayName("Text")]
        [Description("Text")]
        public String Value { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(3)]
        [DisplayName("FontName")]
        [Description("Font Name")]
        public String FontName { get; set; }
        [SortedCategory("Object", 0, 10), PropertyOrder(4)]
        [DisplayName("FontSize")]
        [Description("FontSize")]
        public Int32 FontSize { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(5)]
        [DisplayName("Text Color")]
        [Description("Text Color")]
        public virtual System.Windows.Media.Color TextColor { get; set; }

        public Text(Int32 x, Int32 y, String str)
        {
            SType = ShapeType.Text;
            FontName = "Arial";
            FontSize = 24;
            Thickness = 0;
            BackColor = System.Windows.Media.Colors.Transparent;
            BorderColor = System.Windows.Media.Colors.Transparent;
            TextColor = System.Windows.Media.Colors.Black;

            X = x;
            Y = y;
            Value = str;
        }
    }
}
