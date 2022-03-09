using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Core.model.core.channel;
using Core.model.core.source;
using Core.model.core.source.sg;
using Core.model.core.source.modbus;
using Core.model.core.source.opcda;
using Core.model.design.graphics.control;
using Core.model.design.graphics.shape;
using Core.model.design.window;
using Core.service;
using Core.model.core.archive;

namespace Core.model
{
    public class Model
    {
        public String ProjectFileName { get; private set; }
        public String ProjectDirectory { get; private set; }

        public WorkMode WorkMode { get; private set; }
        public ProjectProperties Properties { get; set; }

        public IDictionary<Int32, Source> SourceStorage { get; set; }
        public delegate void SourceStorageChangedHandler(Source source);
        public event SourceStorageChangedHandler SourceStorageAddEvent;
        public event SourceStorageChangedHandler SourceStorageDeleteEvent;
        public IDictionary<Int32, Channel> ChannelStorage { get; set; }
        public delegate void ChannelStorageChangedHandler(Channel channel);
        public event ChannelStorageChangedHandler ChannelStorageAddEvent;
        public event ChannelStorageChangedHandler ChannelStorageDeleteEvent;
        public IDictionary<Int32, Window> WindowStorage { get; set; }
        public delegate void WindowStorageChangedHandler(Window window);
        public event WindowStorageChangedHandler WindowStorageAddEvent;
        public event WindowStorageChangedHandler WindowStorageDeleteEvent;

        private Int32 lastSourceID = 0;
        private Int32 lastDeviceID = 0;
        private Int32 lastChannelID = 0;
        private Int32 lastWindowID = 0;
        private Int32 lastGElementID = 0;

        private IList<SourceThread> SourceThreads { get; set; }

        private Object locker = new Object();

        // Archives
        public Archive ChArchive { get; private set; }
        public Archive EvArchive { get; private set; }

        private static Model model;

        private Model()
        {
            // WorkMode
            WorkMode = WorkMode.Edit;

            // Project File
            ProjectFileName = "";
            ProjectDirectory = "";

            // Project description
            Properties = new ProjectProperties();
            ChannelStorage = new Dictionary<Int32, Channel>();
            SourceStorage = new Dictionary<Int32, Source>();
            WindowStorage = new Dictionary<Int32, Window>();
            SourceThreads = new List<SourceThread>();

            ChArchive = new ChannelsArchive();
            EvArchive = new EventsArchive();
        }

        public static Model GetInstance()
        {
            if (model == null) { model = new Model(); }
            return model;
        }

        public void ChangeWorkMode(WorkMode mode)
        {
            switch (mode)
            {
                case WorkMode.Edit:
                    if (WorkMode != Core.model.WorkMode.Edit)
                    {
                        SetEditMode();
                    }
                    break;
                case WorkMode.Run:
                    if (WorkMode != Core.model.WorkMode.Run)
                    {
                        SetRunMode();
                    }
                    break;
            }
        }

        public void SetEditMode()
        {
            if (WorkMode == Core.model.WorkMode.Edit) return;
            WorkMode = Core.model.WorkMode.Edit;
            foreach (SourceThread thread in SourceThreads)
            {
                if (thread != null) { thread.Stop(); }
            }

            ChArchive.Disconnect();
            ////EArchive.Disconnect();
        }

        public void SetRunMode()
        {
            if (WorkMode == Core.model.WorkMode.Run) return;
            WorkMode = Core.model.WorkMode.Run;
            if (SourceThreads.Count > 0) { SourceThreads.Clear(); }

            ChArchive.Connect();
            ////EArchive.Connect();

            foreach (Source source in SourceStorage.Values)
            {
                if (source.IsEnable)
                {
                    source.PollingTime = source.PollingTime > 0 ? source.PollingTime : Properties.PollingTime;
                    SourceThread thread = new SourceThread(source);
                    SourceThreads.Add(thread);
                    thread.Start();
                }
            }
        }

        private Int32 GetNewSourceID()
        {
            return ++lastSourceID;
        }

        private Int32 GetNewChannelID()
        {
            return ++lastChannelID;
        }

        private Int32 GetNewWindowID()
        {
            return ++lastWindowID;
        }

        public Source CreateSource(SourceType type)
        {
            Int32 sourceID = GetNewSourceID();
            Source source = null;
            switch (type)
            {
                case SourceType.Modbus:
                    source = new ModbusSRC();
                    source.ID = sourceID;
                    break;
                case SourceType.OPC:
                    source = new OPCDASRC();
                    source.ID = sourceID;
                    break;
                case SourceType.SG:
                    source = new SGSRC();
                    source.ID = sourceID;
                    break;
                default:
                    break;
            }
            if (source != null)
            {
                SourceStorage[sourceID] = source;
                if (SourceStorageAddEvent != null)
                {
                    SourceStorageAddEvent(source);
                }
            }
            return source;
        }

        public Channel CreateChannel(ChannelType type)
        {
            Int32 channelID = GetNewChannelID();
            Channel channel = null;
            switch (type)
            {
                case ChannelType.Bit:
                    channel = new BitChannel(channelID);
                    break;
                case ChannelType.BitArray:
                    channel = new BitArrayChannel(channelID);
                    break;
                case ChannelType.Byte:
                    channel = new ByteChannel(channelID);
                    break;
                case ChannelType.SByte:
                    channel = new SByteChannel(channelID);
                    break;
                case ChannelType.Int16:
                    channel = new Int16Channel(channelID);
                    break;
                case ChannelType.UInt16:
                    channel = new UInt16Channel(channelID);
                    break;
                case ChannelType.Int32:
                    channel = new Int32Channel(channelID);
                    break;
                case ChannelType.UInt32:
                    channel = new UInt32Channel(channelID);
                    break;
                case ChannelType.Float:
                    channel = new FloatChannel(channelID);
                    break;
                case ChannelType.Double:
                    channel = new DoubleChannel(channelID);
                    break;
                case ChannelType.Group:
                    channel = new ChannelGroup();
                    channel.ID = channelID;
                    break;
                default:
                    break;
            }
            if (channel != null)
            {
                ChannelStorage[channelID] = channel;
                if (ChannelStorageAddEvent != null)
                {
                    ChannelStorageAddEvent(channel);
                }
            }
            return channel;
        }

        public SGChannel CreateSGChannel(SGType type)
        {
            Int32 channelID = GetNewChannelID();
            SGChannel channel = new SGChannel();
            channel.SGType = type;
            if (channel != null)
            {
                ChannelStorage[channelID] = channel;
                if (ChannelStorageAddEvent != null)
                {
                    ChannelStorageAddEvent(channel);
                }
            }
            return channel;
        }

        public Window CreateWindow()
        {
            Int32 windowID = GetNewWindowID();
            Window window = new Window();
            window.ID = windowID;
            if (window != null)
            {
                WindowStorage[windowID] = window;
                if (WindowStorageAddEvent != null)
                {
                    WindowStorageAddEvent(window);
                }
            }
            return window;
        }

        public void AddSource(Source source)
        {
            if (source != null)
            {
                SourceStorage[source.ID] = source;
                if (SourceStorageAddEvent != null)
                {
                    SourceStorageAddEvent(source);
                }
                if ((source.ID) > lastSourceID)
                {
                    lastSourceID = source.ID;
                }
            }
        }

        public void DeleteSource(Int32 id)
        {
            if (SourceStorage.ContainsKey(id))
            {
                SourceStorageDeleteEvent(SourceStorage[id]);
                SourceStorage.Remove(id);
            }
        }

        public void AddChannel(Channel channel)
        {
            if (channel != null)
            {
                ChannelStorage[channel.ID] = channel;
                if (ChannelStorageAddEvent != null)
                {
                    ChannelStorageAddEvent(channel);
                }
                if ((channel.ID) > lastChannelID)
                {
                    lastChannelID = channel.ID;
                }
            }
        }

        public void DeleteChannel(Int32 id)
        {
            if (ChannelStorage.ContainsKey(id))
            {
                ChannelStorageDeleteEvent(ChannelStorage[id]);
                ChannelStorage.Remove(id);
            }
        }

        public void AddWindow(Window window)
        {
            if (window != null)
            {
                WindowStorage[window.ID] = window;
                if (WindowStorageAddEvent != null)
                {
                    WindowStorageAddEvent(window);
                }
                if ((window.ID) > lastWindowID)
                {
                    lastWindowID = window.ID;
                }
            }
        }

        public void DeleteWindow(Int32 id)
        {
            if (WindowStorage.ContainsKey(id))
            {
                WindowStorageDeleteEvent(WindowStorage[id]);
                WindowStorage.Remove(id);
            }
        }

        public Channel GetChannelByID(Int32 id)
        {
            Channel channel = null;
            if (ChannelStorage.ContainsKey(id))
            {
                channel = ChannelStorage[id];
            }
            return channel;
        }

        public void Clear()
        {
            lastSourceID = 0;
            lastDeviceID = 0;
            lastChannelID = 0;
            lastWindowID = 0;
            lastGElementID = 0;

            SourceStorage.Clear();
            WindowStorage.Clear();
            ChannelStorage.Clear();

            ProjectFileName = "";
            ProjectDirectory = "";
        }

        public Boolean LoadProject(String fileName)
        {
            Boolean loadSuccess = false;
            ProjectReader reader = new ProjectReader(this);
            loadSuccess = reader.Read(fileName);
            if (loadSuccess)
            {
                ProjectFileName = fileName;
                ProjectDirectory = System.IO.Path.GetDirectoryName(fileName);
                WorkMode = WorkMode.Edit;
            }
            return loadSuccess;
        }

        public Boolean SaveProject()
        {
            Boolean saveSuccess = false;
            if (ProjectFileName.Length > 0 && System.IO.File.Exists(ProjectFileName))
            {
                ProjectWriter writer = new ProjectWriter(this);
                saveSuccess = writer.Write(ProjectFileName);
            }
            return saveSuccess;
        }

        public Boolean SaveProject(String fileName)
        {
            Boolean saveSuccess = false;
            ProjectWriter writer = new ProjectWriter(this);
            saveSuccess = writer.Write(fileName);
            if (saveSuccess)
            {
                ProjectFileName = fileName;
                ProjectDirectory = System.IO.Path.GetDirectoryName(fileName);
            }
            return saveSuccess;
        }
    }
}
