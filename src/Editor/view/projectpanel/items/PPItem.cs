using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Editor.view.projectpanel.items
{
    public class PPItem : TreeViewItem
    {
        public Int32 ID { get; set; }
        public PPItemType Type { get; protected set; }

        public IDictionary<Int32, PPItem> PPChildren { get; set; }

        public PPItem()
        {
            PPChildren = new Dictionary<Int32, PPItem>();
        }
    }
}
