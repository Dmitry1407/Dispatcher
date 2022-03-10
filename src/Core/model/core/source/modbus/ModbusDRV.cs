using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Modbus.Data;
using Modbus.Device;

using Core.model.core.channel;

namespace Core.model.core.source.modbus
{
    public class ModbusDRV
    {
        public PortType PortType { get; private set; }

        // Serial
        public String PortName { get; private set; }
        public Int32 BaudRate { get; private set; }
        public Int32 DataBits { get; private set; }
        public Parity Parity { get; private set; }
        public StopBits StopBits { get; private set; }

        // TCP
        public String TCPAddress { get; private set; }
        public Int32 TCPPort { get; private set; }

        private ModbusMaster master;


        public ModbusDRV() : this("COM6", 9600, 8, Parity.None, StopBits.One) { }

        public ModbusDRV(String portName, Int32 baudRate, Int32 dataBits, Parity parity, StopBits stopBits)
        {
            PortType = PortType.Serial;

            PortName = portName;
            BaudRate = baudRate;
            DataBits = dataBits;
            Parity = parity;
            StopBits = stopBits;

            TCPAddress = "127.0.0.1";
            TCPPort = 502;
        }

        public ModbusDRV(String address, Int32 port)
        {
            PortType = PortType.TCP;

            TCPAddress = address;
            TCPPort = port;
        }

        public void Start()
        {
            switch (PortType)
            {
                case PortType.Serial:
                    SerialPort port = new SerialPort(PortName);
                    // configure serial port
                    port.BaudRate = BaudRate;
                    port.DataBits = DataBits;
                    port.Parity = Parity;
                    port.StopBits = StopBits;
                    port.Open();
                    master = ModbusSerialMaster.CreateRtu(port);
                    break;

                case PortType.TCP:
                case PortType.SerialOverTCP:
                    TcpClient client = new TcpClient(TCPAddress, TCPPort);
                    client.ReceiveTimeout = 5000;
                    client.SendTimeout = 5000;
                    master = ModbusIpMaster.CreateIp(client);
                    break;
            }
        }

        public void Stop()
        {
            master.Dispose();
        }

        public Boolean ReadInput(Byte numDevice, UInt16 startAddress)
        {
            Boolean[] inputs = null;
            if (master != null)
            {
                // read input value
                UInt16 numInputs = 1;
                inputs = master.ReadInputs(numDevice, startAddress, numInputs);

                for (Int32 i = 0; i < numInputs; i++)
                    Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i] ? 1 : 0);
            }
            return inputs != null ? inputs[0] : false;
        }

        public Boolean[] ReadInputs(Byte numDevice, UInt16 startAddress, UInt16 numInputs)
        {
            Boolean[] inputs = null;
            if (master != null)
            {
                // read input values
                startAddress = 0;
                inputs = master.ReadInputs(numDevice, startAddress, numInputs);

                for (Int32 i = 0; i < numInputs; i++)
                    Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i] ? 1 : 0);
            }
            return inputs;
        }

        public Boolean ReadCoil(Byte numDevice, UInt16 startAddress)
        {
            Boolean[] coils = null;
            if (master != null)
            {
                // read coil
                UInt16 numInputs = 1;
                coils = master.ReadCoils(numDevice, startAddress, numInputs);

                for (Int32 i = 0; i < numInputs; i++)
                    Console.WriteLine("Input {0}={1}", startAddress + i, coils[i] ? 1 : 0);
            }
            return coils != null ? coils[0] : false;
        }

        public Boolean[] ReadCoils(Byte numDevice, UInt16 startAddress, UInt16 numInputs)
        {
            Boolean[] coils = null;
            if (master != null)
            {
                // read coils
                startAddress = 0;
                coils = master.ReadCoils(numDevice, startAddress, numInputs);

                for (Int32 i = 0; i < numInputs; i++)
                    Console.WriteLine("Input {0}={1}", startAddress + i, coils[i] ? 1 : 0);
            }
            return coils;
        }

        public UInt16 ReadIRegister(Byte numDevice, UInt16 startAddress)
        {
            UInt16[] inputs = null;
            if (master != null)
            {
                // read IRegister
                UInt16 numInputs = 1;
                inputs = master.ReadInputRegisters(numDevice, startAddress, numInputs);

                for (Int32 i = 0; i < numInputs; i++)
                    Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i]);
            }
            return inputs != null ? inputs[0] : (UInt16)0;
        }

        public UInt16[] ReadIRegisters(Byte numDevice, UInt16 startAddress, UInt16 numInputs)
        {
            UInt16[] inputs = null;
            if (master != null)
            {
                // read IRegisters
                startAddress = 0;
                inputs = master.ReadInputRegisters(numDevice, startAddress, numInputs);

                for (Int32 i = 0; i < numInputs; i++)
                    Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i]);
            }
            return inputs;
        }

        public UInt16 ReadHRegister(Byte numDevice, UInt16 startAddress)
        {
            UInt16[] inputs = null;
            if (master != null)
            {
                // read HRegister
                UInt16 numInputs = 1;
                inputs = master.ReadHoldingRegisters(numDevice, startAddress, numInputs);

                for (Int32 i = 0; i < numInputs; i++)
                    Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i]);
            }
            return inputs != null ? inputs[0] : (UInt16)0;
        }

        public UInt16[] ReadHRegisters(Byte numDevice, UInt16 startAddress, UInt16 numInputs)
        {
            UInt16[] inputs = null;
            if (master != null)
            {
                // read HRegisters
                inputs = master.ReadHoldingRegisters(numDevice, startAddress, numInputs);

                for (Int32 i = 0; i < numInputs; i++)
                    Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i]);
            }
            return inputs;
        }

        public void WriteHRegisters(Byte numDevice, UInt16 startAddress, UInt16 numRegisters)
        {
            if (master != null)
            {
                UInt16[] registers = new UInt16[] { 1, 2, 3, 4, 5 };
                // write five registers
                master.WriteMultipleRegisters(numDevice, startAddress, registers);

                Console.WriteLine("Write Registers:");
                for (Int32 i = 0; i < numRegisters; i++)
                    Console.WriteLine("Register {0}={1}", startAddress + i, registers[i]);
            }
        }
    }
}
