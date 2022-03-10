using System;
using System.ComponentModel;

using Core.model.design.window;
using Core.service.propertygrid;

namespace Core.model
{
    public class ProjectProperties
    {
        public static readonly Int32 DEFAULT_POLLING_TIME = 1000;
        public static readonly Int32 DEFAULT_REDRAW_TIME = 500;
        public static readonly Int32 DEFAULT_GRID_SIZE = 10;

        [SortedCategory("Project", 0, 10), PropertyOrder(0)]
        [DisplayName("Name")]
        [Description("Project Description")]
        public String Name { get; set; }

        [SortedCategory("Project", 0, 10), PropertyOrder(1)]
        [DisplayName("Description")]
        [Description("Project Description")]
        public String Description { get; set; }

        [SortedCategory("Project", 0, 10), PropertyOrder(2)]
        [DisplayName("StartWindow")]
        [Description("Start Window ID")]
        public Int32 StartWindowID { get; set; }

        [SortedCategory("Timing", 1, 10), PropertyOrder(0)]
        [DisplayName("Polling")]
        [Description("Polling Time")]
        public Int32 PollingTime { get; set; } // ms
        [SortedCategory("Timing", 1, 10), PropertyOrder(1)]
        [DisplayName("Redraw")]
        [Description("Redraw Time")]
        public Int32 RedrawTime { get; set; }  // ms

        [SortedCategory("Grid", 2, 10), PropertyOrder(0)]
        [DisplayName("Grid")]
        [Description("Is Grid Dots")]
        public Boolean IsGridDots { get; set; }
        [SortedCategory("Grid", 2, 10), PropertyOrder(1)]
        [DisplayName("Grid Size")]
        [Description("Grid Size")]
        public Int32 GridSize { get; set; }

        [SortedCategory("Snap", 3, 10), PropertyOrder(0)]
        [DisplayName("Snap")]
        [Description("Snap To Grid")]
        public Boolean SnapToGrid { get; set; }

        public ProjectProperties()
        {
            Name = "";
            Description = "";
            StartWindowID = 0;
            PollingTime = DEFAULT_POLLING_TIME;
            RedrawTime = DEFAULT_REDRAW_TIME;

            IsGridDots = true;
            GridSize = DEFAULT_GRID_SIZE;
            SnapToGrid = true;
        }
    }
}
