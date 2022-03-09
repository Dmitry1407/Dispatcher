using System;
using System.Windows.Media;
using System.ComponentModel;

using Core.service.graphics;
using Core.model.core.channel;
using Core.service.propertygrid;

namespace Core.model.design.graphics.control
{
    public class Bar : Control
    {
        [Browsable(false)]
        public Int32 Value
        {
            get { return GetValueFromChannel(); }
            set { }
        }

        [SortedCategory("Object", 0, 10), PropertyOrder(3)]
        [DisplayName("Orientation")]
        [Description("Bar Orientation")]
        public BarOrientation Orientation { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(4)]
        [DisplayName("MinValue")]
        [Description("Min Value")]
        public Int32 MinValue { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(5)]
        [DisplayName("MaxValue")]
        [Description("Max Value")]
        public Int32 MaxValue { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(4)]
        [DisplayName("BarColor")]
        [Description("Bar Color")]
        public Color BarColor { get; set; }


        public Bar() : this(10, 10, 24, 64, BarOrientation.Vertical) { }

        public Bar(Int32 x, Int32 y, Int32 width, Int32 height, BarOrientation orientation)
        {
            CType = ControlType.Bar;

            X = x;
            Y = y;
            Width = width;
            Height = height;
            Orientation = orientation;

            MinValue = 0;
            MaxValue = 10;
        }

        private Int32 GetValueFromChannel()
        {
            Int32 retValue = 0;
            if (ChannelID > 0)
            {
                Channel channel = Model.GetInstance().GetChannelByID(ChannelID);
                retValue = channel != null ? channel.GetIntValue() : 0;
            }
            return retValue;
        }
    }
}
