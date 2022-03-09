using System;
using System.Windows.Media;
using System.ComponentModel;

using Core.model.core.channel;
using Core.service.propertygrid;

namespace Core.model.design.graphics
{
    [TypeConverter(typeof(PropertySorter))]
    [DefaultProperty("ID")]
    public abstract class GElement
    {
        [ReadOnly(true)]
        [SortedCategory("Object", 0, 10), PropertyOrder(0)]
        [DisplayName("ID")]
        [Description("Object ID")]
        public Int32 ID { get; set; }

        [ReadOnly(true)]
        [SortedCategory("Object", 0, 10), PropertyOrder(1)]
        [DisplayName("EType")]
        [Description("Element Type")]
        public GElementType EType { get; protected set; }

        [Browsable(false)]
        public GElement Parent { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(0)]
        [DisplayName("X")]
        [Description("X Coordinate")]
        public virtual Int32 X { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(1)]
        [DisplayName("Y")]
        [Description("Y Coordinate")]
        public virtual Int32 Y { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(0)]
        [DisplayName("Is Visible")]
        [Description("Is Visible")]
        public Boolean IsVisible { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(1)]
        [DisplayName("Thickness")]
        [Description("Thickness")]
        public Int32 Thickness { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(2)]
        [DisplayName("Back Color")]
        [Description("Back Color")]
        public virtual Color BackColor { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(3)]
        [DisplayName("Border Color")]
        [Description("Border Color")]
        public Color BorderColor { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(0)]
        [DisplayName("IsVisibleFromChannel")]
        [Description("IsVisible From Channel")]
        public Boolean IsVisibleFromChannel { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(1)]
        [DisplayName("IsVisibleChannel")]
        [Description("IsVisible Channel")]
        public Int32 IsVisibleChannelID { get; set; }


        public Boolean GetVisible()
        {
            Boolean isVisible = false;
            if (IsVisibleFromChannel && IsVisibleChannelID > 0)
            {
                Channel channel = Model.GetInstance().GetChannelByID(IsVisibleChannelID);
                isVisible = channel != null ? channel.GetBoolValue() : false;
            }
            return isVisible;
        }
    }
}
