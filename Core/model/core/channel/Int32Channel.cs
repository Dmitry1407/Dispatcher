using System;
using System.ComponentModel;

namespace Core.model.core.channel
{
    public class Int32Channel : Channel
    {
        [Browsable(false)]
        public Int32 Value { get; set; }

        public Int32 MinAlarmLimit { get; set; }
        public Int32 MinWarningLimit { get; set; }
        public Int32 MaxWarningLimit { get; set; }
        public Int32 MaxAlarmLimit { get; set; }

        public Int32Channel() : this(0) { }

        public Int32Channel(Int32 id)
            : base(id)
        {
            Type = ChannelType.Int32;
        }

        public Boolean GetBit(Int32 index)
        {
            if (index < 0 || index > 31) { throw (new IndexOutOfRangeException("GetBit from Int32Channel: index etrror")); }
            Int32 retVal = (Value << index) & 1;
            return retVal == 1 ? true : false;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > 0; }
        public override Int32 GetIntValue() { return Value; }
        public override UInt32 GetUIntValue() { return (UInt32)Value; }
        public override Single GetFloatValue() { return (Single)Value; }
        public override Double GetDoubleValue() { return (Double)Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? (Int32)1 : (Int32)0; }
        public override void SetIntValue(Int32 value) { Value = value; }
        public override void SetUIntValue(UInt32 value) { Value = (Int32)value; }
        public override void StFloatValue(Single value) { Value = (Int32)value; }
        public override void SetDoubleValue(Double value) { Value = (Int32)value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = Int32.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[Int32Channel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
