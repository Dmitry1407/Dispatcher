using System;
using System.ComponentModel;

namespace Core.model.core.channel
{
    public class ByteChannel : Channel
    {
        [Browsable(false)]
        public Byte Value { get; set; }

        public Byte MinAlarmLimit { get; set; }
        public Byte MinWarningLimit { get; set; }
        public Byte MaxWarningLimit { get; set; }
        public Byte MaxAlarmLimit { get; set; }

        public ByteChannel() : this(0) { }

        public ByteChannel(Int32 id)
            : base(id)
        {
            Type = ChannelType.Byte;
        }

        public Boolean GetBit(Int32 index)
        {
            if (index < 0 || index > 7) { throw (new IndexOutOfRangeException("GetBit from ByteChannel: index etrror")); }
            UInt32 retVal = ((UInt32)Value << index) & 1;
            return retVal == 1 ? true : false;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > (Byte)0; }
        public override Int32 GetIntValue() { return (Int32)Value; }
        public override UInt32 GetUIntValue() { return (UInt32)Value; }
        public override Single GetFloatValue() { return (Single)Value; }
        public override Double GetDoubleValue() { return (Double)Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? (Byte)1 : (Byte)0; }
        public override void SetIntValue(Int32 value) { Value = (Byte)value; }
        public override void SetUIntValue(UInt32 value) { Value = (Byte)value; }
        public override void StFloatValue(Single value) { Value = (Byte)value; }
        public override void SetDoubleValue(Double value) { Value = (Byte)value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = Byte.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[ByteChannel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
