using System;
using System.Collections.Generic;
using System.ComponentModel;

using Core.model.core.channel;
using Core.model.core.device;
using Core.model.core.source.modbus.byteorder;

namespace Core.model.core.source.modbus
{
    public class MBDevice : Device
    {
        [Browsable(false)]
        public ChannelGroup StatusStorage { get; private set; }
        [Browsable(false)]
        public ChannelGroup CoilStorage { get; private set; }
        [Browsable(false)]
        public ChannelGroup IRegisterStorage { get; private set; }
        [Browsable(false)]
        public ChannelGroup HRegisterStorage { get; private set; }

        public R16ByteOrder R16ByteOrder { get; set; }

        public R32ByteOrder R32ByteOrder { get; set; }

        public MBDevice()
        {
            Name = "MBDevice";
            R16ByteOrder = R16ByteOrder.B10;
            R32ByteOrder = R32ByteOrder.B3210;

            StatusStorage = new ChannelGroup();
            StatusStorage.Name = "Status";
            Model.GetInstance().AddChannel(StatusStorage);

            CoilStorage = new ChannelGroup();
            CoilStorage.Name = "Coils";
            Model.GetInstance().AddChannel(CoilStorage);

            IRegisterStorage = new ChannelGroup();
            IRegisterStorage.Name = "IRegisters";
            Model.GetInstance().AddChannel(IRegisterStorage);

            HRegisterStorage = new ChannelGroup();
            HRegisterStorage.Name = "HRegisters";
            Model.GetInstance().AddChannel(HRegisterStorage);
        }

        public void AddChannel(Channel channel)
        {
            if (channel != null)
            {
                if (channel.Name.Equals("Status")) { StatusStorage = (ChannelGroup)channel; }
                else if (channel.Name.Equals("Coils")) { CoilStorage = (ChannelGroup)channel; }
                else if (channel.Name.Equals("IRegisters")) { IRegisterStorage = (ChannelGroup)channel; }
                else if (channel.Name.Equals("HRegisters")) { HRegisterStorage = (ChannelGroup)channel; }
                Model.GetInstance().AddChannel(channel);
            }
        }
    }
}
