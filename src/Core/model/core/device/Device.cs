using System;
using System.ComponentModel;
using Core.service.propertygrid;

namespace Core.model.core.device
{
    public abstract class Device
    {
        [ReadOnly(true)]
        [SortedCategory("Device", 0, 10), PropertyOrder(0)]
        [DisplayName("ID")]
        [Description("Device ID")]
        public Int32 ID { get; set; }

        [SortedCategory("Device", 0, 10), PropertyOrder(1)]
        [DisplayName("Name")]
        [Description("Device Name")]
        public String Name { get; set; }

        [SortedCategory("Device", 0, 10), PropertyOrder(2)]
        [DisplayName("Description")]
        [Description("Device Description")]
        public String Description { get; set; }

        [SortedCategory("Properties", 1, 10), PropertyOrder(0)]
        [DisplayName("IsEnable")]
        [Description("IsEnable")]
        public Boolean IsEnable { get; set; }

        [SortedCategory("Properties", 1, 10), PropertyOrder(1)]
        [DisplayName("Number")]
        [Description("Device Number")]
        public Int32 Number { get; set; }

        [Browsable(false)]
        public Parent Parent { get; set; }
    }
}
