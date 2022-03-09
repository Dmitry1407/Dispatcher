using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Core.model.core.channel
{
    public class ChannelGroup : Channel
    {
        [Browsable(false)]
        public IDictionary<Int32, Channel> ChannelStorage { get; private set; }

        public Boolean IsEditable { get; protected set; }

        public ChannelGroup() : this("") { }

        public ChannelGroup(String name) : this(name, true) { }

        public ChannelGroup(String name, Boolean isEditable) : this(name, isEditable, 0) { }

        public ChannelGroup(String name, Boolean isEditable, Int32 id)
            : base(id)
        {
            Type = ChannelType.Group;
            ChannelStorage = new Dictionary<Int32, Channel>();
            IsEditable = isEditable;
        }

        public void AddChannel(Channel channel)
        {
            if (channel != null)
            {
                ChannelStorage[channel.ID] = channel;
                Model.GetInstance().AddChannel(channel);
            }
        }

        // Get Value
        public override Boolean GetBoolValue() { return false; }
        public override Int32 GetIntValue() { return 0; }
        public override UInt32 GetUIntValue() { return 0; }
        public override Single GetFloatValue() { return 0F; }
        public override Double GetDoubleValue() { return 0D; }
        public override String GetStringValue() { return ""; }

        // Set Value
        public override void SetBoolValue(Boolean value) { }
        public override void SetIntValue(Int32 value) { }
        public override void SetUIntValue(UInt32 value) { }
        public override void StFloatValue(Single value) { }
        public override void SetDoubleValue(Double value) { }
        public override void SetStringValue(String value) { }

        public override String ToString()
        {
            return String.Format("[ChannelGroup]: NAME=\"{0}\";", Name);
        }
    }
}
