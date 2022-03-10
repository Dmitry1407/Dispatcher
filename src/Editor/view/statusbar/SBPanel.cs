using System;

using Windows = System.Windows;
using Media = System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace Editor.view.statusbar
{
    public class SBPanel : StatusBar
    {
        Label windowSizeLabel;
        Label mousePositionLabel;
        Label elementPositionLabel;

        private StatusBarItem windowSize;
        private StatusBarItem mousePosition;
        private StatusBarItem elementPosition;

        public SBPanel()
        {
            windowSizeLabel = new Label();
            windowSizeLabel.FontSize = 12;
            windowSizeLabel.Padding = new Windows.Thickness(0);
            SetWindowSize(0, 0);

            windowSize = new StatusBarItem();
            windowSize.Padding = new Windows.Thickness(2);
            windowSize.Content = windowSizeLabel;

            mousePositionLabel = new Label();
            mousePositionLabel.FontSize = 12;
            mousePositionLabel.Padding = new Windows.Thickness(0);
            SetMousePosition(0, 0);

            mousePosition = new StatusBarItem();
            mousePosition.Padding = new Windows.Thickness(2);
            mousePosition.Content = mousePositionLabel;

            elementPositionLabel = new Label();
            elementPositionLabel.FontSize = 12;
            elementPositionLabel.Padding = new Windows.Thickness(0);
            SetElementPosition(0, 0);

            elementPosition = new StatusBarItem();
            elementPosition.Padding = new Windows.Thickness(2);
            elementPosition.Content = elementPositionLabel;
            
            this.Items.Add(windowSize);
            this.Items.Add(mousePosition);
            this.Items.Add(elementPosition);
        }

        public void SetWindowSize(Int32 height, Int32 width)
        {
            windowSizeLabel.Content = "[WINDOW H:" + height + " W:" + width + "]";
        }

        public void SetMousePosition(Int32 x, Int32 y)
        {
            mousePositionLabel.Content = "[MOUSE X:" + x + " Y:" + y + "]";
        }

        public void SetElementPosition(Int32 x, Int32 y)
        {
            elementPositionLabel.Content = "[GELEMENT X:" + x + " Y:" + y + "]";
        }
    }
}
