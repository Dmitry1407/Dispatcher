using System;
using System.ComponentModel;

namespace Core.model.core.channel
{
    public class Int16Channel : Channel
    {
        [Browsable(false)]
        public Int16 Value { get; set; }

        public Int16 MinAlarmLimit { get; set; }
        public Int16 MinWarningLimit { get; set; }
        public Int16 MaxWarningLimit { get; set; }
        public Int16 MaxAlarmLimit { get; set; }

        public Int16Channel() : this(0) { }

        public Int16Channel(Int32 id)
            : base(id)
        {
            Type = ChannelType.Int16;
        }

        public Boolean GetBit(Int32 index)
        {
            if (index < 0 || index > 15) { throw (new IndexOutOfRangeException("GetBit from Int16Channel: index etrror")); }
            Int32 retVal = (Value << index) & 1;
            return retVal == 1 ? true : false;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > (Int16)0; }
        public override Int32 GetIntValue() { return (Int32)Value; }
        public override UInt32 GetUIntValue() { return (UInt32)Value; }
        public override Single GetFloatValue() { return (Single)Value; }
        public override Double GetDoubleValue() { return (Double)Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? (Int16)1 : (Int16)0; }
        public override void SetIntValue(Int32 value) { Value = (Int16)value; }
        public override void SetUIntValue(UInt32 value) { Value = (Int16)value; }
        public override void StFloatValue(Single value) { Value = (Int16)value; }
        public override void SetDoubleValue(Double value) { Value = (Int16)value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = Int16.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[Int16Channel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
