using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Core.model.core.channel
{
    public class SByteChannel : Channel
    {
        [Browsable(false)]
        public SByte Value { get; set; }

        public SByte MinAlarmLimit { get; set; }
        public SByte MinWarningLimit { get; set; }
        public SByte MaxWarningLimit { get; set; }
        public SByte MaxAlarmLimit { get; set; }

        public SByteChannel() : this(0) { }

        public SByteChannel(Int32 id)
            : base(id)
        {
            Type = ChannelType.SByte;
        }

        public Boolean GetBit(Int32 index)
        {
            if (index < 0 || index > 7) { throw (new IndexOutOfRangeException("GetBit from SByteChannel: index etrror")); }
            UInt32 retVal = ((UInt32)Value << index) & 1;
            return retVal == 1 ? true : false;
        }

        // Get Value
        public override Boolean GetBoolValue() { return Value > (SByte)0; }
        public override Int32 GetIntValue() { return (Int32)Value; }
        public override UInt32 GetUIntValue() { return (UInt32)Value; }
        public override Single GetFloatValue() { return (Single)Value; }
        public override Double GetDoubleValue() { return (Double)Value; }
        public override String GetStringValue() { return Value.ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { Value = value ? (SByte)1 : (SByte)0; }
        public override void SetIntValue(Int32 value) { Value = (SByte)value; }
        public override void SetUIntValue(UInt32 value) { Value = (SByte)value; }
        public override void StFloatValue(Single value) { Value = (SByte)value; }
        public override void SetDoubleValue(Double value) { Value = (SByte)value; }
        public override void SetStringValue(String value)
        {
            if (value != null && value.Length > 0)
            {
                Value = SByte.Parse(value);
            }
        }

        public override String ToString()
        {
            return String.Format("[SByteChannel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, Value);
        }
    }
}
