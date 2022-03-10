using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

using Core.model.core.channel;

using OPC.Data;
using OPC.Common;
using OPC.Data.Interface;

namespace Core.model.core.source.opcda
{
    public class OPCDASRC : Source
    {
        public IDictionary<Int32, Channel> ChannelStorage { get; private set; }

        private OpcServerList serverList;
        public OpcServer Server { get; set; }
        public OpcGroup Group { get; set; }

        private SERVERSTATUS sts;

        private Boolean isConnected;

        // Tags Definition
        private OPCItemDef[] itemDefs;
        private Int32[] itemHandleServers;

        private Int32 CancelID;
        private Int32[] remErrors;

        public OPCDASRC()
        {
            Type = SourceType.OPC;
            ChannelStorage = new Dictionary<Int32, Channel>();
            IsEnable = true;
            isConnected = false;
        }

        public override void PollingStart()
        {
            try
            {
                // Connect to OPC Server
                Server = new OpcServer();
                Server.Connect(Name);
                Thread.Sleep(100);

                Server.SetClientName("Dispatcher " + (object)Process.GetCurrentProcess().Id);
                Server.GetStatus(out sts);
                Server.ShutdownRequested += new ShutdownRequestEventHandler(ServerShutdownRequested);

                // Create OPC Group
                Group = Server.AddGroup("MyGroup", false, 500);

                // Tags Definition
                Int32 i = 0;
                itemDefs = new OPCItemDef[ChannelStorage.Count];
                foreach (Channel opcChannel in ChannelStorage.Values)
                {
                    itemDefs[i++] = new OPCItemDef(opcChannel.StrAddress, true, (Int32)opcChannel.ID, VarEnum.VT_EMPTY);
                }

                OPCItemResult[] itemResults;
                Group.AddItems(itemDefs, out itemResults);
                if (itemResults == null) { return; }
                ////if (HRESULTS.Failed(rItm[0].Error) || HRESULTS.Failed(rItm[1].Error))
                ////{
                ////    Console.WriteLine("OPC Tester: AddItems - some failed");
                ////    Group.Remove(true);
                ////    Server.Disconnect();
                ////    return;
                ////};

                itemHandleServers = new Int32[ChannelStorage.Count];
                for (i = 0; i < ChannelStorage.Count; i++)
                {
                    itemHandleServers[i] = itemResults[i].HandleServer;
                }

                // Asynch read items
                Group.SetEnable(true);
                Group.Active = true;

                // Register Events
                Group.DataChanged += new DataChangeEventHandler(this.GroupDataChange);
                Group.ReadCompleted += new ReadCompleteEventHandler(this.GroupReadComplete);
                Group.WriteCompleted += new WriteCompleteEventHandler(this.GroupWriteComplete);
                isConnected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect to OPC Server ERROR: " + ((object)ex).ToString());
                isConnected = false;
            }
        }

        public override void PollingStop()
        {
            if (isConnected)
            {
                // Unregister Events
                if (Group != null)
                {
                    Group.DataChanged -= new DataChangeEventHandler(this.GroupDataChange);
                    Group.ReadCompleted -= new ReadCompleteEventHandler(this.GroupReadComplete);
                    Group.WriteCompleted -= new WriteCompleteEventHandler(this.GroupWriteComplete);

                    Group.RemoveItems(itemHandleServers, out remErrors);
                    Group.Remove(false);
                    Group = null;
                }

                // Disconnect
                if (Server != null)
                {
                    Server.Disconnect();
                    Server = null;
                }
                isConnected = false;
            }
        }

        public override void Polling() { }

        private void ServerShutdownRequested(object sender, ShutdownRequestEventArgs e)
        {
            Group.DataChanged -= new DataChangeEventHandler(this.GroupDataChange);
            Group.ReadCompleted -= new ReadCompleteEventHandler(this.GroupReadComplete);
            Group.WriteCompleted -= new WriteCompleteEventHandler(this.GroupWriteComplete);
            Server.ShutdownRequested -= new ShutdownRequestEventHandler(ServerShutdownRequested);

            ////int CancelID1;
            ////int[] aE;
            ////this.Group.Read(itemHandleServers, 55667788, out CancelID1, out aE);
            ////this.Group.SetEnable(false);

            this.Group.SetEnable(false);

            this.Group = null;
            this.Server = null;
            isConnected = false;
        }

        public void GroupDataChange(object sender, DataChangeEventArgs e)
        {
            Console.WriteLine("DataChange event: gh={0} id={1} me={2} mq={3}", e.groupHandleClient, e.transactionID, e.masterError, e.masterQuality);
            foreach (OPCItemState s in e.sts)
            {
                if (HRESULTS.Succeeded(s.Error))
                {
                    //// Console.WriteLine(" ih={0} v={1} q={2} t={3}", s.HandleClient, s.DataValue, s.Quality, s.TimeStamp);
                    UpdateChannel(s);
                }
                else
                { Console.WriteLine(" ih={0}    ERROR=0x{1:x} !", s.HandleClient, s.Error); }
            }
        }

        public void GroupReadComplete(object sender, ReadCompleteEventArgs e)
        {
            Console.WriteLine("ReadComplete event: gh={0} id={1} me={2} mq={3}", e.groupHandleClient, e.transactionID, e.masterError, e.masterQuality);
            foreach (OPCItemState s in e.sts)
            {
                if (HRESULTS.Succeeded(s.Error))
                {
                    //// Console.WriteLine(" ih={0} v={1} q={2} t={3}", s.HandleClient, s.DataValue, s.Quality, s.TimeStamp);
                    UpdateChannel(s);
                }
                else
                { Console.WriteLine(" ih={0}    ERROR=0x{1:x} !", s.HandleClient, s.Error); }
            }
        }

        public void GroupWriteComplete(object sender, WriteCompleteEventArgs e)
        {
            Console.WriteLine("WriteComplete event: gh={0} id={1} me={2}", e.groupHandleClient, e.transactionID, e.masterError);
            foreach (OPCWriteResult r in e.res)
            {
                if (HRESULTS.Succeeded(r.Error))
                { Console.WriteLine(" ih={0} e={1}", r.HandleClient, r.Error); }
                else
                { Console.WriteLine(" ih={0}    ERROR=0x{1:x} !", r.HandleClient, r.Error); }
            }
        }

        private VarEnum GetOpcChannelType(ChannelType type)
        {
            VarEnum opcType = VarEnum.VT_UI1;
            switch (type)
            {
                case ChannelType.Bit:
                    opcType = VarEnum.VT_BOOL;
                    break;
                case ChannelType.Byte:
                    opcType = VarEnum.VT_UI1;
                    break;
                case ChannelType.SByte:
                    opcType = VarEnum.VT_I1;
                    break;
                case ChannelType.Int16:
                    opcType = VarEnum.VT_I2;
                    break;
                case ChannelType.UInt16:
                    opcType = VarEnum.VT_UI2;
                    break;
                case ChannelType.Int32:
                    opcType = VarEnum.VT_INT;
                    break;
                case ChannelType.UInt32:
                    opcType = VarEnum.VT_UINT;
                    break;
                case ChannelType.Float:
                    opcType = VarEnum.VT_R4;
                    break;
                case ChannelType.Double:
                    opcType = VarEnum.VT_R8;
                    break;
                default:
                    break;
            }
            return opcType;
        }

        private void UpdateChannel(OPCItemState state)
        {
            Channel channel = ChannelStorage[state.HandleClient];

            // Set Status
            switch (state.Quality)
            {
                case (short)OPC_QUALITY_STATUS.NOT_CONNECTED:
                    channel.Status = ChannelStatus.NotConnected;
                    break;
                case (short)OPC_QUALITY_STATUS.OK:
                    channel.Status = ChannelStatus.OK;
                    break;
                default:
                    channel.Status = ChannelStatus.CommError;
                    break;
            }

            // Set Timestamp
            channel.TimeStamp = DateTime.FromFileTime(state.TimeStamp);

            // Set Value
            if (state.DataValue != null)
            {
                switch (channel.Type)
                {
                    case ChannelType.Bit:
                        ((BitChannel)channel).Value = (Boolean)state.DataValue;
                        break;
                    case ChannelType.Byte:
                        ((ByteChannel)channel).Value = (Byte)state.DataValue;
                        break;
                    case ChannelType.SByte:
                        ((SByteChannel)channel).Value = (SByte)state.DataValue;
                        break;
                    case ChannelType.Int16:
                        ((Int16Channel)channel).Value = (Int16)state.DataValue;
                        break;
                    case ChannelType.UInt16:
                        ((UInt16Channel)channel).Value = (UInt16)state.DataValue;
                        break;
                    case ChannelType.Int32:
                        ((Int32Channel)channel).Value = (Int32)state.DataValue;
                        break;
                    case ChannelType.UInt32:
                        ((UInt32Channel)channel).Value = (UInt32)state.DataValue;
                        break;
                    case ChannelType.Float:
                        ((FloatChannel)channel).Value = (Single)state.DataValue;
                        break;
                    case ChannelType.Double:
                        ((DoubleChannel)channel).Value = (Double)state.DataValue;
                        break;
                }
            }
        }
    }
}
