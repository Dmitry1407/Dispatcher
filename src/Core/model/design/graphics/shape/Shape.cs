using System;
using System.Windows.Media;
using System.ComponentModel;

using Core.model.core.channel;
using Core.service.propertygrid;

namespace Core.model.design.graphics.shape
{
    public abstract class Shape : GElement
    {
        [ReadOnly(true)]
        [SortedCategory("Object", 0, 10), PropertyOrder(2)]
        [DisplayName("SType")]
        [Description("Shape Type")]
        public ShapeType SType { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(2)]
        [DisplayName("ThicknessFromChannel")]
        [Description("Thickness From Channel")]
        public Boolean ThicknessFromChannel { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(6)]
        [DisplayName("BackColorFromChannel")]
        [Description("BackColor From Channel")]
        public Boolean BackColorFromChannel { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(10)]
        [DisplayName("BorderColorFromChannel")]
        [Description("BorderColor From Channel")]
        public Boolean BorderColorFromChannel { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(3)]
        [DisplayName("ThicknessChannel")]
        [Description("Thickness Channel")]
        public Int32 ThicknessChannelID { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(7)]
        [DisplayName("BackColorChannel")]
        [Description("BackColor Channel")]
        public Int32 BackColorChannelID { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(11)]
        [DisplayName("BorderColorChannel")]
        [Description("BorderColor Channel")]
        public Int32 BorderColorChannelID { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(4)]
        [DisplayName("ThicknessOFF")]
        [Description("ThicknessOFF")]
        public Int32 ThicknessOFF { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(8)]
        [DisplayName("BackColorOFF")]
        [Description("BackColorOFF")]
        public Color BackColorOFF { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(12)]
        [DisplayName("BorderColorOFF")]
        [Description("BorderColorOFF")]
        public Color BorderColorOFF { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(5)]
        [DisplayName("ThicknessON")]
        [Description("ThicknessON")]
        public Int32 ThicknessON { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(9)]
        [DisplayName("BackColorON")]
        [Description("BackColorON")]
        public Color BackColorON { get; set; }

        [SortedCategory("Binding", 3, 10), PropertyOrder(13)]
        [DisplayName("BorderColorON")]
        [Description("BorderColorON")]
        public Color BorderColorON { get; set; }

        public Color BackColorWarning { get; set; }
        public Color BackColorAlarm { get; set; }

        protected Shape()
        {
            EType = GElementType.Shape;
            IsVisible = true;
            Thickness = 1;
            BackColor = Colors.LightGray;
            BorderColor = Colors.Black;

            ThicknessFromChannel = true;
            BackColorFromChannel = true;
            BorderColorFromChannel = true;
        }


        public Int32 GetThickness()
        {
            Int32 thickness = Thickness;
            if (ThicknessFromChannel)
            {
                thickness = GetValueFromChannel(ThicknessChannelID) ?
                    ThicknessON :
                    ThicknessOFF;
            }
            return thickness;
        }

        public Color GetBackColor()
        {
            Color backColor = BackColor;
            if (BackColorFromChannel)
            {
                backColor = GetValueFromChannel(BackColorChannelID) ?
                    BackColorON :
                    BackColorOFF;
            }
            return backColor;
        }

        public Color GetBorderColor()
        {
            Color borderColor = BorderColor;
            if (BorderColorFromChannel)
            {
                borderColor = GetValueFromChannel(BorderColorChannelID) ?
                    BorderColorON :
                    BorderColorOFF;
            }
            return borderColor;
        }

        private Boolean GetValueFromChannel(Int32 channelID)
        {
            Boolean retValue = false;
            if (channelID > 0)
            {
                Channel channel = Model.GetInstance().GetChannelByID(channelID);
                retValue = channel != null ? channel.GetBoolValue() : false;
            }
            return retValue;
        }
    }
}
