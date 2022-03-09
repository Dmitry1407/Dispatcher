using System;
using System.ComponentModel;

namespace Core.model.core.channel
{
    public class UInt16Channel : Channel
    {
        [Browsable(false)]
        public UInt16 Value { get; set; }

        public UInt16 MinAlarmLimit { get; set; }
        public UInt16 MinWarningLimit { get; set; }
        public UInt16 MaxWarningLimit { get; set; }
        public UInt16 MaxAlarmLimit { get; set; }

        public UInt16Channel() : this(0) { }

        public UInt16Channel(Int32 id)
            : base(id)
        {
            Type = ChannelType.UInt16;
        }

        public Boolean GetBit(Int32 index)
        {
            if (index < 0 || index > 15) { throw (new IndexOutOfRangeException("GetBit from UInt16Channel: index etrror")); }
            Int32 retVal = (Value << index) & 1;
            return retVal == 1 ? true : false;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > (UInt16)0; }
        public override Int32 GetIntValue() { return (Int32)Value; }
        public override UInt32 GetUIntValue() { return (UInt32)Value; }
        public override Single GetFloatValue() { return (Single)Value; }
        public override Double GetDoubleValue() { return (Double)Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? (UInt16)1 : (UInt16)0; }
        public override void SetIntValue(Int32 value) { Value = (UInt16)value; }
        public override void SetUIntValue(UInt32 value) { Value = (UInt16)value; }
        public override void StFloatValue(Single value) { Value = (UInt16)value; }
        public override void SetDoubleValue(Double value) { Value = (UInt16)value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = UInt16.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[UInt16Channel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
