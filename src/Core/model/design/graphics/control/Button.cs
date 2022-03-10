using System;
using System.ComponentModel;
using System.Windows.Media;

using Core.service.propertygrid;
using Core.model.core.channel;

namespace Core.model.design.graphics.control
{
    public class Button : Control
    {
        public static readonly String TEXT_OFF = "OFF";
        public static readonly String TEXT_ON = "ON";

        [SortedCategory("Object", 0, 10), PropertyOrder(3)]
        [DisplayName("ButtonType")]
        [Description("Button Type")]
        public ButtonType ButtonType { get; set; }

        [Browsable(false)]
        public Boolean State { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(4)]
        [DisplayName("FontName")]
        [Description("Font Name")]
        public String FontName { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(5)]
        [DisplayName("FontSize")]
        [Description("Font Size")]
        public Int32 FontSize { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(6)]
        [DisplayName("TextOFF")]
        [Description("Text OFF")]
        public String TextOFF { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(7)]
        [DisplayName("TextON")]
        [Description("Text ON")]
        public String TextON { get; set; }

        [Browsable(false)]
        public override Color BackColor { get; set; }
        [SortedCategory("Object", 0, 10), PropertyOrder(7)]
        [DisplayName("BackColorOFF")]
        [Description("BackColor OFF")]
        public Color BackColorOFF { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(8)]
        [DisplayName("BackColorON")]
        [Description("BackColor ON")]
        public Color BackColorON { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(4)]
        [DisplayName("Round")]
        [Description("Round")]
        public Int32 Round { get; set; }

        public Int32 ChangedWindowID { get; set; }

        public Button() : this(10, 10, 64, 32) { }

        public Button(Int32 x, Int32 y, Int32 width, Int32 height) : this(x, y, width, height, 5) { }

        public Button(Int32 x, Int32 y, Int32 width, Int32 height, Int32 round)
        {
            CType = ControlType.Button;
            ButtonType = ButtonType.Moment;
            FontName = "Arial";
            FontSize = 24;

            TextOFF = TEXT_OFF;
            TextON = TEXT_ON;
            BackColorOFF = Colors.LightGray;
            BackColorON = Colors.Gray;

            X = x;
            Y = y;
            Width = width;
            Height = height;
            Round = round;
        }

        public String GetText()
        {
            return State ? TextON : TextOFF;
        }

        public Color GetBackColor()
        {
            return State ? BackColorON : BackColorOFF;
        }

        public void ButtonPress()
        {
            Channel channel = Model.GetInstance().GetChannelByID(ChannelID);
            switch (ButtonType)
            {
                case ButtonType.Set:
                    if (channel != null && channel.Type == ChannelType.Bit) { ((BitChannel)channel).Value = true; }
                    break;
                case ButtonType.Reset:
                    if (channel != null && channel.Type == ChannelType.Bit) { ((BitChannel)channel).Value = false; }
                    break;
                case ButtonType.Moment:
                    State = true;
                    if (channel != null && channel.Type == ChannelType.Bit) { ((BitChannel)channel).Value = State; }
                    break;
                case ButtonType.Toggle:
                    State = !State;
                    if (channel != null && channel.Type == ChannelType.Bit) { ((BitChannel)channel).Value = State; }
                    break;
                default:
                    return;
            }
        }

        public void ButtonRelease()
        {
            Channel channel = Model.GetInstance().GetChannelByID(ChannelID);
            switch (ButtonType)
            {
                case ButtonType.Set:
                    break;
                case ButtonType.Reset:
                    break;
                case ButtonType.Moment:
                    State = false;
                    if (channel != null && channel.Type == ChannelType.Bit) { ((BitChannel)channel).Value = State; }
                    break;
                case ButtonType.Toggle:
                    break;
                default:
                    return;
            }
        }
    }
}
