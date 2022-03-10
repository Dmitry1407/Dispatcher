using System;

using Windows = System.Windows;
using Media = System.Windows.Media;
using System.Windows.Controls;
using Editor.view.projectpanel.items;

namespace Editor.view.projectpanel
{
    public class ProjectPanel : Windows.Controls.TreeView
    {
        public ProjectItem Project { get; private set; }
        public SourcesItem Sources { get; private set; }
        public WindowsItem Windows { get; private set; }

        public ProjectPanel()
        {
            Project = new ProjectItem();
            Sources = new SourcesItem();
            Windows = new WindowsItem();

            this.Items.Add(Project);
            this.Items.Add(Sources);
            this.Items.Add(Windows);
        }

        public void Clear()
        {
            if (Sources != null && Sources.Items.Count > 0)
            {
                Sources.Items.Clear();
            }
            if (Windows != null && Windows.Items.Count > 0)
            {
                Windows.Items.Clear();
            }
        }
    }
}
