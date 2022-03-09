using System;

namespace Core.service
{
    public class Utils
    {
        public static Boolean GetUIntBit(UInt32 value, Int32 index)
        {
            UInt32 retVal = (value >> index) & 1;
            return retVal == 1 ? true : false;
        }

        public static UInt32 SetUIntBit(UInt32 value, Int32 index, Boolean bit)
        {
            UInt32 mask = 0x01;
            mask = mask << index;
            return bit ? value | mask : value & ~mask;
        }
    }
}
