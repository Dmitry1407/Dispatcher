using System;
using System.ComponentModel;

namespace Core.model.core.channel
{
    public class UInt32Channel : Channel
    {
        [Browsable(false)]
        public UInt32 Value { get; set; }

        public UInt32 MinAlarmLimit { get; set; }
        public UInt32 MinWarningLimit { get; set; }
        public UInt32 MaxWarningLimit { get; set; }
        public UInt32 MaxAlarmLimit { get; set; }

        public UInt32Channel() : this(0) { }

        public UInt32Channel(Int32 id)
            : base(id)
        {
            Type = ChannelType.UInt32;
        }

        public Boolean GetBit(Int32 index)
        {
            if (index < 0 || index > 31) { throw (new IndexOutOfRangeException("GetBit from UInt32Channel: index etrror")); }
            UInt32 retVal = (Value << index) & 1;
            return retVal == 1 ? true : false;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > (UInt32)0; }
        public override Int32 GetIntValue() { return (Int32)Value; }
        public override UInt32 GetUIntValue() { return Value; }
        public override Single GetFloatValue() { return (Single)Value; }
        public override Double GetDoubleValue() { return (Double)Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? (UInt32)1 : (UInt32)0; }
        public override void SetIntValue(Int32 value) { Value = (UInt32)value; }
        public override void SetUIntValue(UInt32 value) { Value = value; }
        public override void StFloatValue(Single value) { Value = (UInt32)value; }
        public override void SetDoubleValue(Double value) { Value = (UInt32)value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = UInt32.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[UInt32Channel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
