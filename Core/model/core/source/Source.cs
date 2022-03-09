using System;
using System.Collections.Generic;
using System.ComponentModel;

using Core.model.core.channel;
using Core.service.propertygrid;

namespace Core.model.core.source
{
    public abstract class Source
    {
        [ReadOnly(true)]
        [SortedCategory("Source", 0, 10), PropertyOrder(0)]
        [DisplayName("ID")]
        [Description("Source ID")]
        public Int32 ID { get; set; }

        [ReadOnly(true)]
        [SortedCategory("Source", 0, 10), PropertyOrder(1)]
        [DisplayName("Type")]
        [Description("Source Type")]
        public SourceType Type { get; protected set; }

        [SortedCategory("Source", 0, 10), PropertyOrder(2)]
        [DisplayName("Name")]
        [Description("Source Name")]
        public String Name { get; set; }

        [SortedCategory("Source", 0, 10), PropertyOrder(3)]
        [DisplayName("Description")]
        [Description("Object Description")]
        public String Description { get; set; }

        [SortedCategory("Source", 0, 10), PropertyOrder(4)]
        [DisplayName("IsEnable")]
        [Description("IsEnable")]
        public Boolean IsEnable { get; set; }

        [SortedCategory("Timing", 1, 10), PropertyOrder(0)]
        [DisplayName("Polling")]
        [Description("Polling Time")]
        public Int32 PollingTime { get; set; }

        public abstract void PollingStart();
        public abstract void PollingStop();
        public abstract void Polling();
    }
}
