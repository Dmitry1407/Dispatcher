using System;
using System.Windows.Media;
using System.ComponentModel;

using Core.model.core.channel;
using Core.service.propertygrid;

namespace Core.model.design.graphics.control
{
    public abstract class Control : GElement
    {
        [ReadOnly(true)]
        [SortedCategory("Object", 0, 10), PropertyOrder(2)]
        [DisplayName("CType")]
        [Description("Control Type")]
        public ControlType CType { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(2)]
        [DisplayName("Width")]
        [Description("Width")]
        public Int32 Width { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(3)]
        [DisplayName("Height")]
        [Description("Height")]
        public Int32 Height { get; set; }

        public Boolean IsActive { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(0)]
        [DisplayName("Channel")]
        [Description("Channel")]
        public Int32 ChannelID { get; set; }

        protected Control()
        {
            EType = GElementType.Control;
            IsActive = true;
            IsVisible = true;
            Thickness = 1;

            BackColor = Colors.LightGray;
            BorderColor = Colors.Black;
        }
    }
}
