using System;
using System.ComponentModel;

namespace Core.model.core.channel
{
    public class FloatChannel : Channel
    {
        [Browsable(false)]
        public Single Value { get; set; }

        public Single MinAlarmLimit { get; set; }
        public Single MinWarningLimit { get; set; }
        public Single MaxWarningLimit { get; set; }
        public Single MaxAlarmLimit { get; set; }

        public FloatChannel() : this(0) { }

        public FloatChannel(Int32 id)
            : base(id)
        {
            Type = ChannelType.Float;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > 0F; }
        public override Int32 GetIntValue() { return (Int32)Value; }
        public override UInt32 GetUIntValue() { return (UInt32)Value; }
        public override Single GetFloatValue() { return Value; }
        public override Double GetDoubleValue() { return (Double)Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? 1F : 0F; }
        public override void SetIntValue(Int32 value) { Value = (Single)value; }
        public override void SetUIntValue(UInt32 value) { Value = (Single)value; }
        public override void StFloatValue(Single value) { Value = value; }
        public override void SetDoubleValue(Double value) { Value = (Single)value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = Single.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[FloatChannel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
