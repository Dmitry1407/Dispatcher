using System;
using System.ComponentModel;
using System.Text;

using Core.service.propertygrid;

namespace Core.model.core.channel
{
    public abstract class Channel
    {
        // Channel identification
        [ReadOnly(true)]
        [SortedCategory("Channel", 0, 10), PropertyOrder(0)]
        [DisplayName("ID")]
        [Description("Channel ID")]
        public Int32 ID { get; set; }

        [ReadOnly(true)]
        [SortedCategory("Channel", 0, 10), PropertyOrder(1)]
        [DisplayName("Type")]
        [Description("Channel Type")]
        public ChannelType Type { get; set; }

        [SortedCategory("Channel", 0, 10), PropertyOrder(2)]
        [DisplayName("Name")]
        [Description("Channel Name")]
        public virtual String Name { get; set; }

        [SortedCategory("Channel", 0, 10), PropertyOrder(3)]
        [DisplayName("Description")]
        [Description("Channel Description")]
        public String Description { get; set; }

        [SortedCategory("Channel", 0, 10), PropertyOrder(4)]
        [DisplayName("NumAddress")]
        [Description("Channel Numerical Address")]
        public Int32 NumAddress { get; set; }

        [SortedCategory("Channel", 0, 10), PropertyOrder(5)]
        [DisplayName("StrAddress")]
        [Description("Channel String Address")]
        public String StrAddress { get; set; }

        // Channels parent
        [Browsable(false)]
        public Parent Parent { get; set; }

        // Channels properties
        [SortedCategory("Properties", 1, 10), PropertyOrder(0)]
        [DisplayName("IOMode")]
        [Description("Channel IOMode")]
        public IOMode IOMode { get; set; }

        [SortedCategory("Properties", 1, 10), PropertyOrder(1)]
        [DisplayName("IsEnable")]
        [Description("IsEnable")]
        public Boolean IsEnable { get; set; }

        [SortedCategory("Properties", 1, 10), PropertyOrder(2)]
        [DisplayName("IsArchive")]
        [Description("Channel IsArchive")]
        public Boolean IsArchive { get; set; }

        [Browsable(false)]
        public DateTime TimeStamp { get; set; }

        [Browsable(false)]
        public ChannelStatus Status { get; set; }

        public Channel(Int32 id)
        {
            ID = id;
            IsEnable = true;
            NumAddress = 0;
            StrAddress = "";
        }

        // Get Value
        public abstract Boolean GetBoolValue();
        public abstract Int32 GetIntValue();
        public abstract UInt32 GetUIntValue();
        public abstract Single GetFloatValue();
        public abstract Double GetDoubleValue();
        public abstract String GetStringValue();

        // Set Value
        public abstract void SetBoolValue(Boolean value);
        public abstract void SetIntValue(Int32 value);
        public abstract void SetUIntValue(UInt32 value);
        public abstract void StFloatValue(Single value);
        public abstract void SetDoubleValue(Double value);
        public abstract void SetStringValue(String value);
    }
}
