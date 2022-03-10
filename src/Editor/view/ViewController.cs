using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;

using Core.model;
using Core.model.core;
using Core.model.core.channel;
using Core.model.core.device;
using Core.model.core.source;
using Core.model.core.source.modbus;
using Core.model.core.source.opcda;
using Core.model.core.source.sg;
using Core.model.design.graphics;
using Core.model.design.graphics.control;
using Core.model.design.graphics.shape;
using Core.model.design.window;

using Editor.view.projectpanel;
using Editor.view.projectpanel.items;
using Editor.view.propertiespanel;
using Editor.view.statusbar;
using Editor.view.workspace;
using Editor.view.workspace.edit;
using Editor.view.workspace.run;


namespace Editor.view
{
    public class ViewController
    {
        public Model Model { get; private set; }

        public WSView CurrentView { get; private set; }
        public WSView EditView { get; private set; }
        public WSView RunView { get; private set; }

        public ProjectPanel ProjectPanel { get; private set; }
        public PropertiesPanel PropertiesPanel { get; private set; }
        public SBPanel SBPanel { get; private set; }

        public Window CurrentWindow { get; set; }
        public Source CurrentSource { get; set; }
        public Device CurrentDevice { get; set; }
        public Channel CurrentChannel { get; set; }

        public ViewController()
        {
            Model = Model.GetInstance();

            EditView = new WSEditView();
            ((WSEditView)EditView).ViewController = this;
            RunView = new WSRunView();
            ((WSRunView)RunView).ViewController = this;
            CurrentView = EditView;

            // ProjectPanel
            ProjectPanel = new ProjectPanel();
            ProjectPanel.Width = 200D;
            ProjectPanel.SelectedItemChanged += ProjectPanelSelectedItemChanged;

            ////Model.SourceStorageAddEvent += SourceStorageAddHandler;
            ////Model.SourceStorageDeleteEvent += SourceStorageDeleteHandler;
            ////Model.ChannelStorageAddEvent += ChannelStorageAddHandler;
            ////Model.ChannelStorageDeleteEvent += ChannelStorageDeleteHandler;
            ////Model.WindowStorageAddEvent += WindowStorageAddHandler;
            ////Model.WindowStorageDeleteEvent += WindowStorageDeleteHandler;

            // PropertiesPanel
            PropertiesPanel = new PropertiesPanel();
            PropertiesPanel.Width = 280D;
            PropertiesPanel.PropertyGrid.PropertyValueChanged += PropertyValueChanged;

            // Status Bar
            SBPanel = new SBPanel();
        }

        public void AddNewSource(SourceType type)
        {
            Source source = Model.CreateSource(type);
            UpdateProjectPanel();
        }

        public void AddNewChannel(ChannelType type)
        {
            Channel channel = Model.CreateChannel(type);
            UpdateProjectPanel();
        }

        public void AddNewWindow()
        {
            Window window = Model.CreateWindow();
            CurrentWindow = window;
            CurrentView.CurrentWindow = CurrentWindow;
            UpdateProjectPanel();
        }

        public void DeleteWindow(Int32 windowID)
        {
            Model.DeleteWindow(windowID);
        }

        public void AddNewShape(ShapeType shapeType)
        {
            if (IsEditMode() && CurrentWindow != null)
            {
                GElement element = null;
                switch (shapeType)
                {
                    case ShapeType.Line:
                        element = new Line();
                        ((Shape)element).BackColorOFF = System.Windows.Media.Colors.Orange;
                        ((Shape)element).BorderColorOFF = System.Windows.Media.Colors.Orange;
                        ((Shape)element).ThicknessOFF = 1;
                        break;
                    case ShapeType.Path:
                        element = new Path(10, 10);
                        ((Path)element).AddPoint(10, 10);
                        ((Path)element).AddPoint(10, -10);
                        ((Path)element).AddPoint(10, 10);
                        ((Shape)element).BackColorOFF = System.Windows.Media.Colors.Orange;
                        ((Shape)element).BorderColorOFF = System.Windows.Media.Colors.Orange;
                        ((Shape)element).ThicknessOFF = 1;
                        break;
                    case ShapeType.Rectangle:
                        element = new Rectangle(10, 10, 50, 50);
                        ((Shape)element).BackColorOFF = System.Windows.Media.Colors.Orange;
                        ((Shape)element).BorderColorOFF = System.Windows.Media.Colors.Black;
                        ((Shape)element).ThicknessOFF = 1;
                        break;
                    case ShapeType.Circle:
                        element = new Circle(10, 10, 25);
                        ((Shape)element).BackColorOFF = System.Windows.Media.Colors.Orange;
                        ((Shape)element).BorderColorOFF = System.Windows.Media.Colors.Black;
                        ((Shape)element).ThicknessOFF = 1;
                        break;
                    case ShapeType.Polygon:
                        element = new Polygon(10, 10);
                        ((Polygon)element).AddPoint(50, 0);
                        ((Polygon)element).AddPoint(0, 50);
                        ((Polygon)element).AddPoint(-50, 0);
                        ((Shape)element).BackColorOFF = System.Windows.Media.Colors.Orange;
                        ((Shape)element).BorderColorOFF = System.Windows.Media.Colors.Black;
                        ((Shape)element).ThicknessOFF = 1;
                        break;
                    case ShapeType.Text:
                        element = new Text(10, 10, "Text");
                        break;
                    case ShapeType.Picture:
                        element = new Picture();
                        ((Picture)element).FileName = "D:\\TEMP\\Target64.png";
                        ((Picture)element).Width = 64;
                        ((Picture)element).Height = 64;
                        break;
                }
                if (element != null)
                {
                    CurrentWindow.AddGElement(element);
                }
            }
        }

        public void AddNewControl(ControlType controlType)
        {
            if (IsEditMode() && CurrentWindow != null)
            {
                GElement element = null;
                switch (controlType)
                {
                    case ControlType.Button:
                        element = new Button();
                        break;
                    case ControlType.NField:
                        element = new NField();
                        break;
                    case ControlType.Bar:
                        element = new Bar();
                        break;
                }
                if (element != null)
                {
                    CurrentWindow.AddGElement(element);
                }
            }
        }

        public void AddGElement(GElement element)
        {
            if (IsEditMode() && CurrentWindow != null && element != null)
            {
                CurrentWindow.AddGElement(element);
            }
        }

        public void DeleteGElement(GElement element)
        {
            if (IsEditMode() && CurrentWindow != null && element != null)
            {
                if (CurrentWindow.ElementStorage.Count > 0 && CurrentWindow.ElementStorage.ContainsKey(element.ID))
                {
                    CurrentWindow.DeleteGElement(element);
                }
            }
        }

        public Boolean IsEditMode()
        {
            return Model.WorkMode == WorkMode.Edit;
        }

        public void SetEditMode()
        {
            CurrentView.Clear();
            CurrentView = EditView;

            // StopPeriodRedraw
            ((WSRunView)RunView).StopPeriodicRedraw();

            Model.SetEditMode();
            CurrentView.CurrentWindow = CurrentWindow;
            CurrentView.DrawCurrentWindow();
        }

        public void SetRunMode()
        {
            CurrentView.Clear();
            CurrentView = RunView;
            Model.SetRunMode();

            // StartPeriodRedraw
            CurrentView.CurrentWindow = CurrentWindow;
            ((WSRunView)RunView).StartPeriodicRedraw();
        }

        public void ChangeWindow(Int32 windowID)
        {
            if (windowID > 0 && Model.WindowStorage.ContainsKey(windowID))
            {
                CurrentWindow = Model.WindowStorage[windowID];
                PropertiesPanel.PropertyGrid.SelectedObject = CurrentWindow;
                CurrentView.Clear();
                CurrentView.CurrentWindow = CurrentWindow;
                SBPanel.SetWindowSize(CurrentWindow.Height, CurrentWindow.Width);
                CurrentView.DrawCurrentWindow();
            }
        }

        private void ProjectPanelSelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            PPItem selectedItem = (PPItem)ProjectPanel.SelectedItem;
            switch (selectedItem.Type)
            {
                case PPItemType.Project:
                    PropertiesPanel.PropertyGrid.SelectedObject = Model.Properties;
                    break;
                case PPItemType.Sources:
                    break;
                case PPItemType.Source:
                    CurrentSource = Model.SourceStorage[selectedItem.ID];
                    PropertiesPanel.PropertyGrid.SelectedObject = CurrentSource;
                    break;
                case PPItemType.Device:
                    Source selectedSource = Model.SourceStorage[((PPItem)selectedItem.Parent).ID];
                    if (selectedSource.Type == SourceType.Modbus)
                    {
                        CurrentDevice = ((ModbusSRC)selectedSource).DevicesStorage[selectedItem.ID];
                        PropertiesPanel.PropertyGrid.SelectedObject = CurrentDevice;
                    }
                    break;
                case PPItemType.Channel:
                    CurrentChannel = Model.GetChannelByID(selectedItem.ID);
                    PropertiesPanel.PropertyGrid.SelectedObject = CurrentChannel;
                    break;
                case PPItemType.Windows:
                    break;
                case PPItemType.Window:
                    ChangeWindow(selectedItem.ID);
                    break;
                default:
                    break;
            }
        }

        public void UpdateProjectPanel()
        {
            // Clear ProjectPanel
            ProjectPanel.Clear();

            // Create Sources
            foreach (Source source in Model.SourceStorage.Values)
            {
                SourceItem sourceItem = CreatePPSource(source);
                ProjectPanel.Sources.PPChildren[sourceItem.ID] = sourceItem;
                ProjectPanel.Sources.Items.Add(sourceItem);

                switch (source.Type)
                {
                    case SourceType.Modbus:
                        ModbusSRC mbSRC = (ModbusSRC)source;
                        foreach (MBDevice mbDevice in mbSRC.DevicesStorage.Values)
                        {
                            DeviceItem deviceItem = CreatePPDevice(mbDevice);
                            sourceItem.PPChildren[mbDevice.ID] = deviceItem;
                            sourceItem.Items.Add(deviceItem);

                            ChannelItem StatusItem = CreatePPChannel(mbDevice.StatusStorage);
                            deviceItem.PPChildren[mbDevice.StatusStorage.ID] = StatusItem;
                            deviceItem.Items.Add(StatusItem);
                            foreach (Channel channel in mbDevice.StatusStorage.ChannelStorage.Values)
                            {
                                ChannelItem channelItem = CreatePPChannel(channel);
                                StatusItem.PPChildren[channel.ID] = channelItem;
                                StatusItem.Items.Add(channelItem);
                            }

                            ChannelItem CoilItem = CreatePPChannel(mbDevice.CoilStorage);
                            deviceItem.PPChildren[mbDevice.CoilStorage.ID] = CoilItem;
                            deviceItem.Items.Add(CoilItem);
                            foreach (Channel channel in mbDevice.CoilStorage.ChannelStorage.Values)
                            {
                                ChannelItem channelItem = CreatePPChannel(channel);
                                CoilItem.PPChildren[channel.ID] = channelItem;
                                CoilItem.Items.Add(channelItem);
                            }

                            ChannelItem IRegisterItem = CreatePPChannel(mbDevice.IRegisterStorage);
                            deviceItem.PPChildren[mbDevice.IRegisterStorage.ID] = IRegisterItem;
                            deviceItem.Items.Add(IRegisterItem);
                            foreach (Channel channel in mbDevice.IRegisterStorage.ChannelStorage.Values)
                            {
                                ChannelItem channelItem = CreatePPChannel(channel);
                                IRegisterItem.PPChildren[channel.ID] = channelItem;
                                IRegisterItem.Items.Add(channelItem);
                            }

                            ChannelItem HRegisterItem = CreatePPChannel(mbDevice.HRegisterStorage);
                            deviceItem.PPChildren[mbDevice.HRegisterStorage.ID] = HRegisterItem;
                            deviceItem.Items.Add(HRegisterItem);
                            foreach (Channel channel in mbDevice.HRegisterStorage.ChannelStorage.Values)
                            {
                                ChannelItem channelItem = CreatePPChannel(channel);
                                HRegisterItem.PPChildren[channel.ID] = channelItem;
                                HRegisterItem.Items.Add(channelItem);
                            }
                        }
                        break;

                    case SourceType.OPC:
                        OPCDASRC opcSRC = (OPCDASRC)source;
                        foreach (Channel opcChannel in opcSRC.ChannelStorage.Values)
                        {
                            ChannelItem channelItem = CreatePPChannel(opcChannel);
                            sourceItem.PPChildren[opcChannel.ID] = channelItem;
                            sourceItem.Items.Add(channelItem);
                        }
                        break;

                    case SourceType.SG:
                        SGSRC sgSRC = (SGSRC)source;
                        foreach (SGChannel sgChannel in sgSRC.ChannelStorage.Values)
                        {
                            ChannelItem channelItem = CreatePPChannel(sgChannel);
                            sourceItem.PPChildren[sgChannel.ID] = channelItem;
                            sourceItem.Items.Add(channelItem);
                        }
                        break;
                }
            }

            // Create Windows
            foreach (Window window in Model.WindowStorage.Values)
            {
                WindowItem windowItem = CreatePPWindow(window);
                ProjectPanel.Windows.PPChildren[windowItem.ID] = windowItem;
                ProjectPanel.Windows.Items.Add(windowItem);
            }
        }

        private SourceItem CreatePPSource(Source source)
        {
            SourceItem sourceItem = new SourceItem();
            sourceItem.ID = source.ID;
            sourceItem.Header = source.Name != null && !source.Name.Equals("") ? source.Name : "Source " + source.ID;
            return sourceItem;
        }

        private DeviceItem CreatePPDevice(Device device)
        {
            DeviceItem deviceItem = new DeviceItem();
            deviceItem.ID = device.ID;
            deviceItem.Header = device.Name != null && !device.Name.Equals("") ? device.Name : "Device " + device.ID;
            return deviceItem;
        }

        private ChannelItem CreatePPChannel(Channel channel)
        {
            ChannelItem channelItem = new ChannelItem();
            channelItem.ID = channel.ID;
            channelItem.Header = channel.Name != null && !channel.Name.Equals("") ? channel.Name : "Channel " + channel.ID;
            return channelItem;
        }

        private WindowItem CreatePPWindow(Window window)
        {
            WindowItem windowItem = new WindowItem();
            windowItem.ID = window.ID;
            windowItem.Header = window.Name != null && !window.Name.Equals("") ? window.Name : "Window " + window.ID;
            return windowItem;
        }

        private void SourceStorageAddHandler(Source source)
        {
            SourceItem sourceItem = new SourceItem();
            sourceItem.ID = source.ID;
            sourceItem.Header = String.Format("{0:d2} - Source", source.ID);
            ProjectPanel.Sources.PPChildren[sourceItem.ID] = sourceItem;
            ProjectPanel.Sources.Items.Add(sourceItem);
        }

        private void SourceStorageDeleteHandler(Source source)
        {
        }

        private void ChannelStorageAddHandler(Channel channel)
        {
            ChannelItem channelItem = new ChannelItem();
            channelItem.ID = channel.ID;
            channelItem.Header = String.Format("{0:d2} - Channel", channel.ID);

            if (channel.Parent == null)
            {
                PPItem selectedItem = (PPItem)ProjectPanel.SelectedItem;
                switch (selectedItem.Type)
                {
                    case PPItemType.Source:
                        channel.Parent = new Parent(selectedItem.ID, ParentType.Source);
                        break;
                    case PPItemType.Device:
                        channel.Parent = new Parent(selectedItem.ID, ParentType.Device);
                        break;
                    case PPItemType.Channel:
                        channel.Parent = new Parent(selectedItem.ID, ParentType.ChannelGroup);
                        break;
                    default:
                        return;
                }
            }

            PPItem parentItem = null;
            switch (channel.Parent.ParentType)
            {
                case ParentType.Source:
                    parentItem = (SourceItem)ProjectPanel.Sources.PPChildren[channel.Parent.ParentID];
                    parentItem.Items.Add(channelItem);
                    break;
                case ParentType.Device:
                    break;
                case ParentType.ChannelGroup:
                    ////parentItem = (ChannelItem)ProjectPanel.Sources.Items[(int)channel.Parent.ParentID];
                    parentItem.Items.Add(channelItem);
                    break;
            }
        }

        private void ChannelStorageDeleteHandler(Channel channel)
        {
        }

        private void WindowStorageAddHandler(Window window)
        {
            WindowItem windowItem = new WindowItem();
            windowItem.ID = window.ID;
            windowItem.Header = String.Format("{0:d2} - Window", window.ID);
            ProjectPanel.Windows.PPChildren[windowItem.ID] = windowItem;
            ProjectPanel.Windows.Items.Add(windowItem);
        }

        private void WindowStorageDeleteHandler(Window window)
        {
        }

        private void PropertyValueChanged(Object sender, Forms.PropertyValueChangedEventArgs e)
        {
            if (sender is Forms.PropertyGrid)
            {
                if (((Forms.PropertyGrid)sender).SelectedObject is GElement)
                {
                    GElement element = (GElement)((Forms.PropertyGrid)sender).SelectedObject;
                    CurrentView.RedrawElement(element);
                }
                else if (((Forms.PropertyGrid)sender).SelectedObject is Window)
                {
                    Window vindow = (Window)((Forms.PropertyGrid)sender).SelectedObject;
                    ChangeWindow(vindow.ID);
                }
            }
        }
    }
}
