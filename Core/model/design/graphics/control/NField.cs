using System;
using System.Windows.Media;
using System.ComponentModel;

using Core.service.graphics;
using Core.model.core.channel;
using Core.service.propertygrid;

namespace Core.model.design.graphics.control
{
    public class NField : Control
    {
        [Browsable(false)]
        public String Value
        {
            get
            {
                return Model.GetInstance().WorkMode == WorkMode.Run ?
                    GetValueFromChannel() :
                    "####";
            }
            protected set { }
        }

        [SortedCategory("Object", 0, 10), PropertyOrder(3)]
        [DisplayName("FontName")]
        [Description("Font Name")]
        public String FontName { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(4)]
        [DisplayName("FontSize")]
        [Description("Font Size")]
        public Int32 FontSize { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(5)]
        [DisplayName("HAlignment")]
        [Description("Horizontal Alignment")]
        public HAlignment HAlignment { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(6)]
        [DisplayName("VAlignment")]
        [Description("Vertical Alignment")]
        public VAlignment VAlignment { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(4)]
        [DisplayName("TextColor")]
        [Description("Text Color")]
        public Color TextColor { get; set; }

        public NField() : this(10, 10, 64, 32) { }

        public NField(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            CType = ControlType.NField;
            FontName = "Arial";
            FontSize = 24;
            HAlignment = HAlignment.Center;
            VAlignment = VAlignment.Center;

            this.X = x;
            this.Y = y;
            Width = width;
            Height = height;
        }

        private String GetValueFromChannel()
        {
            string retValue = "####";
            if (ChannelID > 0)
            {
                Channel channel = Model.GetInstance().GetChannelByID(ChannelID);
                retValue = channel != null ? channel.GetStringValue() : "";
            }
            return retValue;
        }
    }
}
