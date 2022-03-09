using System;
using System.ComponentModel;

namespace Core.model.core.channel
{
    public class DoubleChannel : Channel
    {
        [Browsable(false)]
        public Double Value { get; set; }

        public Double MinAlarmLimit { get; set; }
        public Double MinWarningLimit { get; set; }
        public Double MaxWarningLimit { get; set; }
        public Double MaxAlarmLimit { get; set; }

        public DoubleChannel() : this(0) { }

        public DoubleChannel(Int32 id)
            : base(id)
        {
            Type = ChannelType.Double;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > 0D; }
        public override Int32 GetIntValue() { return (Int32)Value; }
        public override UInt32 GetUIntValue() { return (UInt32)Value; }
        public override Single GetFloatValue() { return (Single)Value; }
        public override Double GetDoubleValue() { return Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? 1D : 0D; }
        public override void SetIntValue(Int32 value) { Value = (Double)value; }
        public override void SetUIntValue(UInt32 value) { Value = (Double)value; }
        public override void StFloatValue(Single value) { Value = (Double)value; }
        public override void SetDoubleValue(Double value) { Value = value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = Double.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[DoubleChannel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
