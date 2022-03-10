using System;

namespace Editor.view.projectpanel.items
{
    public class ProjectItem : PPItem
    {
        public ProjectItem()
        {
            Type = PPItemType.Project;
            this.Header = "Project";
        }
    }
}
