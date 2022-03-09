using System;
using System.Collections;
using System.Text;

namespace Core.model.core.channel
{
    public class BitArrayChannel : Channel
    {
        public BitArray BitStorage { get; private set; }

        public BitArrayChannel() : this(0) { }

        public BitArrayChannel(Int32 id)
            : base(id)
        {
            Type = ChannelType.BitArray;
            BitStorage = new BitArray(32);
        }

        public Boolean GetBit(Int32 index)
        {
            return BitStorage.Get(index);
        }

        public void SetBit(Int32 index, Boolean value)
        {
            BitStorage.Set(index, value);
        }

        // Get Value
        public override Boolean GetBoolValue() { return GetIntFromBitArray() > 0; }
        public override Int32 GetIntValue() { return GetIntFromBitArray(); }
        public override UInt32 GetUIntValue() { return (UInt32)GetIntFromBitArray(); }
        public override Single GetFloatValue() { return (Single)GetIntFromBitArray(); }
        public override Double GetDoubleValue() { return (Double)GetIntFromBitArray(); }
        public override String GetStringValue() { return GetIntFromBitArray().ToString(); }

        // Set Value
        public override void SetBoolValue(Boolean value) { }
        public override void SetIntValue(Int32 value) { }
        public override void SetUIntValue(UInt32 value) { }
        public override void StFloatValue(Single value) { }
        public override void SetDoubleValue(Double value) { }
        public override void SetStringValue(String value) { }

        private Int32 GetIntFromBitArray()
        {
            Int32[] array = new Int32[1];
            BitStorage.CopyTo(array, 0);
            return array[0];
        }

        public override String ToString()
        {
            return String.Format("[BitArrayChannel]: ID={0}; NAME=\"{1}\"; VALUE={2};", ID, Name, BitStorage.ToString());
        }
    }
}
