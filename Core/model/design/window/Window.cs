using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Media;

using Core.model.design.graphics;
using Core.model.design.graphics.control;
using Core.model.design.graphics.shape;
using Core.service.propertygrid;

namespace Core.model.design.window
{
    public class Window
    {
        [ReadOnly(true)]
        [SortedCategory("Object", 0, 10), PropertyOrder(0)]
        [DisplayName("ID")]
        [Description("Window ID")]
        public Int32 ID { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(1)]
        [DisplayName("Name")]
        [Description("Window Name")]
        public String Name { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(2)]
        [DisplayName("Description")]
        [Description("Window Description")]
        public String Description { get; set; }

        [SortedCategory("Object", 0, 10), PropertyOrder(3)]
        [DisplayName("WindowType")]
        [Description("Window Type")]
        public WindowType Type { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(0)]
        [DisplayName("X")]
        [Description("X Coordinate")]
        public Int32 X { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(1)]
        [DisplayName("Y")]
        [Description("Y Coordinate")]
        public Int32 Y { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(2)]
        [DisplayName("Width")]
        [Description("Window Width")]
        public Int32 Width { get; set; }

        [SortedCategory("Geometry", 1, 10), PropertyOrder(3)]
        [DisplayName("Height")]
        [Description("Window Height")]
        public Int32 Height { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(0)]
        [DisplayName("Thickness")]
        [Description("Border Thickness")]
        public Int32 BorderThickness { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(1)]
        [DisplayName("BackColor")]
        [Description("Back Color")]
        public Color BackColor { get; set; }

        [SortedCategory("View", 2, 10), PropertyOrder(2)]
        [DisplayName("BorderColor")]
        [Description("Border Color")]
        public Color BorderColor { get; set; }

        [Browsable(false)]
        public IDictionary<Int32, GElement> ElementStorage { get; protected set; }

        [Browsable(false)]
        public Int32 LastElementID { get; private set; }

        public Window() : this(0, 0, 640, 480) { }

        public Window(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            ElementStorage = new Dictionary<Int32, GElement>();
            Type = WindowType.FullScreen;
            Name = "";
            LastElementID = 0;

            X = x;
            Y = y;
            Width = width;
            Height = height;
            BorderThickness = 1;

            BackColor = Colors.DarkGray;
            BorderColor = Colors.Black;
        }

        public void AddGElement(GElement element)
        {
            if (element != null)
            {
                element.ID = LastElementID;
                ElementStorage.Add(LastElementID, element);
                LastElementID++;
            }
        }

        public void DeleteGElement(GElement element)
        {
            if (element != null && ElementStorage.ContainsKey(element.ID))
            {
                ElementStorage.Remove(element.ID);
            }
        }

        public void Clear()
        {
            ElementStorage.Clear();
        }
    }
}
