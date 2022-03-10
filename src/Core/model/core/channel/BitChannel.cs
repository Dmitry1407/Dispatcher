using System;
using System.ComponentModel;

namespace Core.model.core.channel
{
    public class BitChannel : Channel
    {
        [Browsable(false)]
        public Boolean Value { get; set; }

        public BitChannel() : this(0) { }

        public BitChannel(Int32 id)
            : base(id)
        {
            Type = ChannelType.Bit;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value; }
        public override Int32 GetIntValue() { return Value ? 1 : 0; }
        public override UInt32 GetUIntValue() { return Value ? (UInt32)1 : (UInt32)0; }
        public override Single GetFloatValue() { return Value ? 1F : 0F; }
        public override Double GetDoubleValue() { return Value ? 1D : 0D; }
        public override String GetStringValue() { return Value ? "1" : "0"; }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value; }
        public override void SetIntValue(Int32 value) { Value = value == 0 ? false : true; }
        public override void SetUIntValue(UInt32 value) { Value = value == 0 ? false : true; }
        public override void StFloatValue(Single value) { Value = value == 0 ? false : true; }
        public override void SetDoubleValue(Double value) { Value = value == 0D ? false : true; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = Boolean.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[BitChannel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
