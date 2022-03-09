using System;

using Windows = System.Windows;
using Media = System.Windows.Media;
using Forms = System.Windows.Forms;

namespace Editor.view.propertiespanel
{
    public class PropertiesPanel : Windows.Controls.UserControl
    {
        private Forms.Integration.WindowsFormsHost host;
        public Forms.PropertyGrid PropertyGrid { get; private set; }

        public PropertiesPanel()
        {
            // PropertyGrid creating
            host = new Forms.Integration.WindowsFormsHost();
            PropertyGrid = new Forms.PropertyGrid();
            PropertyGrid.PropertySort = Forms.PropertySort.Categorized;

            host.Child = PropertyGrid;
            this.Content = host;
        }
    }
}
