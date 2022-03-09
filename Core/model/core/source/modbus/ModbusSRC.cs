using System;
using System.Collections.Generic;

using Core.model.core.channel;

namespace Core.model.core.source.modbus
{
    public class ModbusSRC : Source
    {
        public IDictionary<Int32, MBDevice> DevicesStorage { get; set; }
        public ModbusDRV Driver { get; private set; }

        public ModbusSRC()
        {
            Type = SourceType.Modbus;
            DevicesStorage = new Dictionary<Int32, MBDevice>();
            Driver = new ModbusDRV();
            IsEnable = true;
        }

        public void setDriver(ModbusDRV driver)
        {
            this.Driver = driver;
        }

        public void AddDevice(MBDevice device)
        {
            if (device != null)
            {
                DevicesStorage.Add(device.ID, device);
            }
        }

        public override void PollingStart()
        {
            Driver.Start();
        }

        public override void PollingStop()
        {
            Driver.Stop();
        }

        public override void Polling()
        {
            foreach (MBDevice device in DevicesStorage.Values)
            {
                // Read Status Bits
                foreach (Channel channel in device.StatusStorage.ChannelStorage.Values)
                {
                    ((BitChannel)channel).Value = Driver.ReadInput((Byte)device.Number, (UInt16)channel.NumAddress);
                }

                // Read Coils Bits
                foreach (Channel channel in device.CoilStorage.ChannelStorage.Values)
                {
                    ((BitChannel)channel).Value = Driver.ReadCoil((Byte)device.Number, (UInt16)channel.NumAddress);
                }

                // Read Input Registers
                foreach (Channel channel in device.IRegisterStorage.ChannelStorage.Values)
                {
                    ((Int16Channel)channel).Value = (Int16)Driver.ReadIRegister((Byte)device.Number, (UInt16)channel.NumAddress);
                }

                // Read Hold Registers
                foreach (Channel channel in device.HRegisterStorage.ChannelStorage.Values)
                {
                    switch (channel.Type)
                    {
                        case (ChannelType.UInt16):
                            ((UInt16Channel)channel).Value = (UInt16)Driver.ReadHRegister((Byte)device.Number, (UInt16)channel.NumAddress);
                            break;
                        case (ChannelType.Int16):
                            ((Int16Channel)channel).Value = (Int16)Driver.ReadHRegister((Byte)device.Number, (UInt16)channel.NumAddress);
                            break;
                        case (ChannelType.UInt32):
                            {
                                UInt16[] registers = Driver.ReadHRegisters((Byte)device.Number, (UInt16)channel.NumAddress, 2);
                                ((UInt32Channel)channel).Value = (UInt32)(registers[1] << 16 | registers[0]);
                                break;
                            }
                        case (ChannelType.Int32):
                            {
                                UInt16[] registers = Driver.ReadHRegisters((Byte)device.Number, (UInt16)channel.NumAddress, 2);
                                ((Int32Channel)channel).Value = registers[1] << 16 | registers[0];
                                break;
                            }
                    }
                }
            }
        }
    }
}
