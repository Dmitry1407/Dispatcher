using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO.Ports;

using Media = System.Windows.Media;

using Core.model;
using Core.model.core;
using Core.model.core.channel;
using Core.model.core.source.sg;
using Core.model.core.source;
using Core.model.core.source.modbus;
using Core.model.design.graphics.shape;
using Core.model.design.graphics.control;
using Core.model.design.graphics;
using Core.model.design.window;
using Core.service.graphics;
using Core.model.core.source.opcda;

namespace Core.service
{
    public class ProjectReader
    {
        private Model model;
        private XmlDocument xDocument;

        public ProjectReader(Model model)
        {
            this.model = model;
        }

        public Boolean Read(String fileName)
        {
            Boolean readSuccess = false;

            try
            {
                model.Clear();
                xDocument = new XmlDocument();
                xDocument.Load(fileName);
                XmlElement xRoot = xDocument.DocumentElement;

                XmlNode elProjectProperties = xRoot.SelectSingleNode(ProjXmlNames.PROPERTIES);
                ReadProjectProperties(elProjectProperties);

                XmlNode elSources = xRoot.SelectSingleNode(ProjXmlNames.SOURCES);
                ReadSources(elSources);

                XmlNode elWindows = xRoot.SelectSingleNode(ProjXmlNames.WINDOWS);
                ReadWindows(elWindows);

                readSuccess = true;
            }
            catch (Exception ex) { }
            return readSuccess;
        }

        // Read Project Properties
        private void ReadProjectProperties(XmlNode elProjectProperties)
        {
            // Names
            model.Properties.Name = GetStrAttribute(elProjectProperties, ProjXmlNames.Name);
            model.Properties.Description = GetStrAttribute(elProjectProperties, ProjXmlNames.Description);
            // Timings
            Int32 pollingTime = GetIntAttribute(elProjectProperties, ProjXmlNames.PollingTime);
            if (pollingTime > 0) { model.Properties.PollingTime = pollingTime; }
            Int32 redrawTime = GetIntAttribute(elProjectProperties, ProjXmlNames.RedrawTime);
            if (redrawTime > 0) { model.Properties.RedrawTime = redrawTime; }
            // Grid
            model.Properties.IsGridDots = GetBoolAttribute(elProjectProperties, ProjXmlNames.Grid);
            Int32 gridSize = GetIntAttribute(elProjectProperties, ProjXmlNames.GridSize);
            if (gridSize > 0) { model.Properties.GridSize = gridSize; }
        }

        // Read Sources
        private void ReadSources(XmlNode elSources)
        {
            foreach (XmlNode elSource in elSources.ChildNodes)
            {
                if (elSource != null) { ReadSource(elSource); }
            }
        }

        // Read Source
        private Source ReadSource(XmlNode elSource)
        {
            Source source = null;
            if (elSource != null)
            {
                Int32 id = GetIntAttribute(elSource, ProjXmlNames.ID);
                String name = GetStrAttribute(elSource, ProjXmlNames.Name);

                String strSourceType = GetStrAttribute(elSource, ProjXmlNames.Type);
                if (strSourceType.Equals(ProjXmlNames.Modbus))
                {   // Modbus
                    source = new ModbusSRC();
                    source.ID = id;
                    model.AddSource(source);
                    String strPortType = GetStrAttribute(elSource, ProjXmlNames.PortType);
                    if (strPortType.Length > 0 && strPortType.Equals(ProjXmlNames.Serial))
                    {
                        String portName = GetStrAttribute(elSource, ProjXmlNames.PortName);
                        Int32 baudRate = GetIntAttribute(elSource, ProjXmlNames.BaudRate);
                        Int32 dataBits = GetIntAttribute(elSource, ProjXmlNames.DataBits);
                        Int32 parity = GetIntAttribute(elSource, ProjXmlNames.Parity);
                        Int32 stopBits = GetIntAttribute(elSource, ProjXmlNames.StopBits);

                        ModbusDRV driver = new ModbusDRV(portName, baudRate, dataBits, (Parity)parity, (StopBits)stopBits);
                        ((ModbusSRC)source).setDriver(driver);
                    }
                    else if (strPortType.Length > 0 && strPortType.Equals(ProjXmlNames.TCP))
                    {
                        String tcpAddress = GetStrAttribute(elSource, ProjXmlNames.TCPAddress);
                        Int32 tcpPPort = GetIntAttribute(elSource, ProjXmlNames.TCPPort);
                        ModbusDRV driver = new ModbusDRV(tcpAddress, tcpPPort != 0 ? tcpPPort : 502);
                        ((ModbusSRC)source).setDriver(driver);
                    }
                    XmlNode elDevices = elSource.SelectSingleNode(ProjXmlNames.DEVICES);
                    ReadMBDevices(elDevices, (ModbusSRC)source);
                }
                else if (strSourceType.Equals(ProjXmlNames.OPC))
                {
                    source = new OPCDASRC();
                    source.ID = id;
                    model.AddSource(source);

                    XmlNode elChannels = elSource.SelectSingleNode(ProjXmlNames.CHANNELS);
                    foreach (XmlNode elChannel in elChannels.ChildNodes)
                    {
                        if (elChannel != null)
                        {
                            Channel channel = ReadChannel(elChannel);
                            channel.Parent = new Parent(id, ParentType.Source);
                            ((OPCDASRC)source).ChannelStorage[channel.ID] = channel;
                            Model.GetInstance().AddChannel(channel);
                        }
                    }
                }
                else if (strSourceType.Equals(ProjXmlNames.SG))
                {
                    source = new SGSRC();
                    source.ID = id;
                    model.AddSource(source);
                    XmlNode elChannels = elSource.SelectSingleNode(ProjXmlNames.CHANNELS);
                    foreach (XmlNode elChannel in elChannels.ChildNodes)
                    {
                        if (elChannel != null)
                        {
                            Channel channel = ReadChannel(elChannel);
                            channel.Parent = new Parent(id, ParentType.Source);
                            ((SGSRC)source).AddChannel((SGChannel)channel);
                        }
                    }
                }
                source.Name = GetStrAttribute(elSource, ProjXmlNames.Name);
                source.Description = GetStrAttribute(elSource, ProjXmlNames.Description);
                source.PollingTime = GetIntAttribute(elSource, ProjXmlNames.PollingTime);
            }
            return source;
        }

        // Read MBDevices
        private void ReadMBDevices(XmlNode elDevices, ModbusSRC source)
        {
            foreach (XmlNode elDevice in elDevices.ChildNodes)
            {
                if (elDevice != null) { source.AddDevice(ReadMBDevice(elDevice)); }
            }
        }

        // Read MBDevice
        private MBDevice ReadMBDevice(XmlNode elDevice)
        {
            MBDevice device = new MBDevice();
            device.Name = GetStrAttribute(elDevice, ProjXmlNames.Name);
            device.ID = GetIntAttribute(elDevice, ProjXmlNames.ID);
            device.Number = GetIntAttribute(elDevice, ProjXmlNames.Number);

            XmlNode elChannels = elDevice.SelectSingleNode(ProjXmlNames.CHANNELS);
            foreach (XmlNode elChannel in elChannels.ChildNodes)
            {
                if (elChannel != null)
                {
                    Channel channel = ReadChannel(elChannel);
                    channel.Parent = new Parent(device.ID, ParentType.Device);
                    device.AddChannel(channel);
                }
            }
            return device;
        }

        // Read Channel
        private Channel ReadChannel(XmlNode elChannel)
        {
            Channel channel = null;
            String strChannelType = GetStrAttribute(elChannel, ProjXmlNames.Type);

            if (strChannelType.Equals(ProjXmlNames.Bit)) { channel = new BitChannel(); }
            else if (strChannelType.Equals(ProjXmlNames.BitArray)) { channel = new BitArrayChannel(); }
            else if (strChannelType.Equals(ProjXmlNames.Byte)) { channel = new ByteChannel(); }
            else if (strChannelType.Equals(ProjXmlNames.SByte)) { channel = new SByteChannel(); }
            else if (strChannelType.Equals(ProjXmlNames.Int_16)) { channel = new Int16Channel(); }
            else if (strChannelType.Equals(ProjXmlNames.UInt_16)) { channel = new UInt16Channel(); }
            else if (strChannelType.Equals(ProjXmlNames.Int_32)) { channel = new Int32Channel(); }
            else if (strChannelType.Equals(ProjXmlNames.UInt_32)) { channel = new UInt32Channel(); }
            else if (strChannelType.Equals(ProjXmlNames.Float)) { channel = new FloatChannel(); }
            else if (strChannelType.Equals(ProjXmlNames.Double)) { channel = new DoubleChannel(); }

            // ChannelGroup
            else if (strChannelType.Equals(ProjXmlNames.Group))
            {
                channel = new ChannelGroup();
                XmlNode elChannels = elChannel.SelectSingleNode(ProjXmlNames.CHANNELS);
                if (elChannels != null)
                {
                    foreach (XmlNode elChannelFromGroup in elChannels.ChildNodes)
                    {
                        if (elChannelFromGroup != null)
                        {
                            Channel channelFromGroup = ReadChannel(elChannelFromGroup);
                            channelFromGroup.Parent = new Parent(channel.ID, ParentType.ChannelGroup);
                            if (channelFromGroup != null) { ((ChannelGroup)channel).AddChannel(channelFromGroup); }
                        }
                    }
                }
            }

            // SignalGenerator
            else if (strChannelType.Equals(ProjXmlNames.SG))
            {
                String strGeneratorType = GetStrAttribute(elChannel, ProjXmlNames.SGType);
                Double minValue = GetDoubleAttribute(elChannel, ProjXmlNames.MinValue);
                Double maxValue = GetDoubleAttribute(elChannel, ProjXmlNames.MaxValue);

                channel = new SGChannel(0, minValue, maxValue);
                if (strGeneratorType.Equals(ProjXmlNames.Square)) { ((SGChannel)channel).SGType = SGType.Square; }
                else if (strGeneratorType.Equals(ProjXmlNames.Triangle)) { ((SGChannel)channel).SGType = SGType.Triangle; }
                else if (strGeneratorType.Equals(ProjXmlNames.Sawtooth)) { ((SGChannel)channel).SGType = SGType.Sawtooth; }
                else if (strGeneratorType.Equals(ProjXmlNames.Sine)) { ((SGChannel)channel).SGType = SGType.Sine; }
                else if (strGeneratorType.Equals(ProjXmlNames.Random)) { ((SGChannel)channel).SGType = SGType.Random; }
            }

            // Add ChannelID
            if (channel != null)
            {
                channel.ID = GetIntAttribute(elChannel, ProjXmlNames.ID);
                channel.NumAddress = GetIntAttribute(elChannel, ProjXmlNames.NumAddress);
                channel.StrAddress = GetStrAttribute(elChannel, ProjXmlNames.StrAddress);
                channel.Name = GetStrAttribute(elChannel, ProjXmlNames.Name);
                channel.Description = GetStrAttribute(elChannel, ProjXmlNames.Description);
            }
            return channel;
        }

        // Read Windows
        private void ReadWindows(XmlNode elWindows)
        {
            foreach (XmlNode elWindow in elWindows.ChildNodes)
            {
                if (elWindow != null) { model.AddWindow(ReadWindow(elWindow)); }
            }
        }

        // Read Window
        private Window ReadWindow(XmlNode elWindow)
        {
            Window window = null;
            if (elWindow != null)
            {
                Int32 x = GetIntAttribute(elWindow, ProjXmlNames.X);
                Int32 y = GetIntAttribute(elWindow, ProjXmlNames.Y);
                Int32 width = GetIntAttribute(elWindow, ProjXmlNames.Width);
                Int32 height = GetIntAttribute(elWindow, ProjXmlNames.Height);

                window = new Window(x, y, width, height);
                window.ID = GetIntAttribute(elWindow, ProjXmlNames.ID);
                window.Name = GetStrAttribute(elWindow, ProjXmlNames.Name);
                window.BackColor = (Media.Color)Media.ColorConverter.ConvertFromString(GetStrAttribute(elWindow, ProjXmlNames.BackColor));

                String strWindowType = GetStrAttribute(elWindow, ProjXmlNames.WindowType);
                if (strWindowType.Equals(ProjXmlNames.FullScreen)) { window.Type = WindowType.FullScreen; }
                else if (strWindowType.Equals(ProjXmlNames.Floating)) { window.Type = WindowType.Floating; }

                XmlNode elGElements = elWindow.SelectSingleNode(ProjXmlNames.ELEMENTS);
                ReadGElements(elGElements, window);
            }
            return window;
        }

        // Read GElements
        private void ReadGElements(XmlNode elGElements, Window window)
        {
            foreach (XmlNode elGElement in elGElements.ChildNodes)
            {
                if (elGElement != null)
                {
                    GElement gElement = ReadGElement(elGElement);
                    window.AddGElement(gElement);
                }
            }
        }

        // Read GElement
        private GElement ReadGElement(XmlNode elGElement)
        {
            GElement gElement = null;
            String strGElementType = elGElement.Name;

            if (strGElementType.Equals(ProjXmlNames.CONTROL)) { gElement = ReadControl(elGElement); }
            else if (strGElementType.Equals(ProjXmlNames.SHAPE)) { gElement = ReadShape(elGElement); }

            if (gElement != null)
            {
                gElement.IsVisible = GetBoolAttribute(elGElement, ProjXmlNames.IsVisible);
                gElement.IsVisibleFromChannel = GetBoolAttribute(elGElement, ProjXmlNames.IsVisibleFromChannel);
                gElement.IsVisibleChannelID = GetIntAttribute(elGElement, ProjXmlNames.IsVisibleChannelID);

                gElement.Thickness = GetIntAttribute(elGElement, ProjXmlNames.Thickness);

                String strBackColor = GetStrAttribute(elGElement, ProjXmlNames.BackColor);
                if (strBackColor.Length > 0) { gElement.BackColor = (Media.Color)Media.ColorConverter.ConvertFromString(strBackColor); }
                String strBorderColor = GetStrAttribute(elGElement, ProjXmlNames.BorderColor);
                if (strBorderColor.Length > 0) { gElement.BorderColor = (Media.Color)Media.ColorConverter.ConvertFromString(strBorderColor); }
            }
            return gElement;
        }

        // Read Shape
        private Shape ReadShape(XmlNode elShape)
        {
            Shape shape = null;

            Int32 x = GetIntAttribute(elShape, ProjXmlNames.X);
            Int32 y = GetIntAttribute(elShape, ProjXmlNames.Y);

            Int32 thicknessOff = GetIntAttribute(elShape, ProjXmlNames.ThicknessOFF);
            Media.Color backColorOff = (Media.Color)Media.ColorConverter.ConvertFromString(GetStrAttribute(elShape, ProjXmlNames.BackColorOFF));
            Media.Color borderColorOff = (Media.Color)Media.ColorConverter.ConvertFromString(GetStrAttribute(elShape, ProjXmlNames.BorderColorOFF));

            Int32 thicknessOn = GetIntAttribute(elShape, ProjXmlNames.ThicknessON);
            Media.Color backColorOn = (Media.Color)Media.ColorConverter.ConvertFromString(GetStrAttribute(elShape, ProjXmlNames.BackColorON));
            Media.Color borderColorOn = (Media.Color)Media.ColorConverter.ConvertFromString(GetStrAttribute(elShape, ProjXmlNames.BorderColorON));

            Boolean thicknessFromChannel = GetBoolAttribute(elShape, ProjXmlNames.ThicknessFromChannel);
            Boolean backColorFromChannel = GetBoolAttribute(elShape, ProjXmlNames.BackColorFromChannel);
            Boolean borderColorFromChannel = GetBoolAttribute(elShape, ProjXmlNames.BorderColorFromChannel);

            Int32 thicknessChannelID = GetIntAttribute(elShape, ProjXmlNames.ThicknessChannelID);
            Int32 backColorChannelID = GetIntAttribute(elShape, ProjXmlNames.BackColorChannelID);
            Int32 borderColorChannelID = GetIntAttribute(elShape, ProjXmlNames.BorderColorChannelID);

            String strShapeType = GetStrAttribute(elShape, ProjXmlNames.Type);
            if (strShapeType.Equals(ProjXmlNames.Line))
            {
                Int32 dX = GetIntAttribute(elShape, ProjXmlNames.DX);
                Int32 dY = GetIntAttribute(elShape, ProjXmlNames.DY);
                Line line = new Line(x, y, dX, dY);
                shape = line;
            }

            else if (strShapeType.Equals(ProjXmlNames.Path))
            {
                Path path = new Path(x, y);
                XmlNode elPathPoints = elShape.SelectSingleNode(ProjXmlNames.POINTS);
                foreach (XmlNode elPoint in elPathPoints.ChildNodes)
                {
                    if (elPoint != null)
                    {
                        Int32 pointX = GetIntAttribute(elPoint, ProjXmlNames.X);
                        Int32 pointY = GetIntAttribute(elPoint, ProjXmlNames.Y);
                        path.AddPoint(pointX, pointY);
                    }
                }
                shape = path;
            }

            else if (strShapeType.Equals(ProjXmlNames.Circle))
            {
                Int32 radius = GetIntAttribute(elShape, ProjXmlNames.Radius);
                Circle circle = new Circle(x, y, radius);
                shape = circle;
            }

            else if (strShapeType.Equals(ProjXmlNames.Rectangle))
            {
                Int32 width = GetIntAttribute(elShape, ProjXmlNames.Width);
                Int32 height = GetIntAttribute(elShape, ProjXmlNames.Height);
                Rectangle rectangle = new Rectangle(x, y, width, height);
                rectangle.Round = GetIntAttribute(elShape, ProjXmlNames.Round);
                shape = rectangle;
            }

            else if (strShapeType.Equals(ProjXmlNames.Polygon))
            {
                Polygon polygon = new Polygon(x, y);
                XmlNode elPolygonPoints = elShape.SelectSingleNode(ProjXmlNames.POINTS);
                foreach (XmlNode elPoint in elPolygonPoints.ChildNodes)
                {
                    if (elPoint != null)
                    {
                        Int32 pointX = GetIntAttribute(elPoint, ProjXmlNames.X);
                        Int32 pointY = GetIntAttribute(elPoint, ProjXmlNames.Y);
                        polygon.AddPoint(pointX, pointY);
                    }
                }
                shape = polygon;
            }

            else if (strShapeType.Equals(ProjXmlNames.Text))
            {
                String str = GetStrAttribute(elShape, ProjXmlNames.Value);
                shape = new Text(x, y, str);
            }

            if (shape != null)
            {
                shape.ThicknessFromChannel = thicknessFromChannel;
                shape.BackColorFromChannel = backColorFromChannel;
                shape.BorderColorFromChannel = borderColorFromChannel;

                shape.ThicknessChannelID = thicknessChannelID;
                shape.BackColorChannelID = backColorChannelID;
                shape.BorderColorChannelID = borderColorChannelID;

                shape.ThicknessOFF = thicknessOff;
                shape.BackColorOFF = backColorOff;
                shape.BorderColorOFF = borderColorOff;

                shape.ThicknessON = thicknessOn;
                shape.BackColorON = backColorOn;
                shape.BorderColorON = borderColorOn;
            }
            return shape;
        }

        // Read Control
        private Control ReadControl(XmlNode elControl)
        {
            Control control = null;

            Int32 x = GetIntAttribute(elControl, ProjXmlNames.X);
            Int32 y = GetIntAttribute(elControl, ProjXmlNames.Y);
            Int32 width = GetIntAttribute(elControl, ProjXmlNames.Width);
            Int32 height = GetIntAttribute(elControl, ProjXmlNames.Height);

            // Create Control
            String strControlType = GetStrAttribute(elControl, ProjXmlNames.Type);
            if (strControlType.Equals(ProjXmlNames.Button))
            {   // Button
                Button button = new Button(x, y, width, height);
                String buttonType = GetStrAttribute(elControl, ProjXmlNames.ButtonType);

                // Button Type
                if (buttonType.Equals(ProjXmlNames.Set)) { button.ButtonType = ButtonType.Set; }
                else if (buttonType.Equals(ProjXmlNames.Reset)) { button.ButtonType = ButtonType.Reset; }
                else if (buttonType.Equals(ProjXmlNames.Moment)) { button.ButtonType = ButtonType.Moment; }
                else if (buttonType.Equals(ProjXmlNames.Toggle)) { button.ButtonType = ButtonType.Toggle; }
                else if (buttonType.Equals(ProjXmlNames.ChangeWindow)) { button.ButtonType = ButtonType.ChangeWindow; }

                // ChangedWindowID
                button.ChangedWindowID = GetIntAttribute(elControl, ProjXmlNames.ChangedWindowID);

                // Text
                String textOFF = GetStrAttribute(elControl, ProjXmlNames.TextOFF);
                if (textOFF.Length > 0 && !textOFF.Equals(Button.TEXT_OFF)) { button.TextOFF = textOFF; }
                String textON = GetStrAttribute(elControl, ProjXmlNames.TextON);
                if (textON.Length > 0 && !textON.Equals(Button.TEXT_ON)) { button.TextON = textON; }

                // Color
                String strBackColorOff = GetStrAttribute(elControl, ProjXmlNames.BackColorOFF);
                if (strBackColorOff.Length > 0) { button.BackColorOFF = (Media.Color)Media.ColorConverter.ConvertFromString(strBackColorOff); }
                String strBackColorOn = GetStrAttribute(elControl, ProjXmlNames.BackColorON);
                if (strBackColorOn.Length > 0) { button.BackColorON = (Media.Color)Media.ColorConverter.ConvertFromString(strBackColorOn); }

                button.Round = GetIntAttribute(elControl, ProjXmlNames.Round);
                control = button;
            }
            else if (strControlType.Equals(ProjXmlNames.NField))
            {   // NField
                NField field = new NField(x, y, width, height);
                String strTextColor = GetStrAttribute(elControl, ProjXmlNames.TextColor);
                if (strTextColor.Length > 0) { field.TextColor = (Media.Color)Media.ColorConverter.ConvertFromString(strTextColor); }
                control = field;
            }
            else if (strControlType.Equals(ProjXmlNames.Bar))
            {   // Bar
                Bar bar = new Bar(x, y, width, height, BarOrientation.Vertical);

                String strOrientation = GetStrAttribute(elControl, ProjXmlNames.Orientation);
                if (strOrientation.Equals(BarOrientation.Horizontal.ToString())) { bar.Orientation = BarOrientation.Horizontal; }
                else if (strOrientation.Equals(BarOrientation.Vertical.ToString())) { bar.Orientation = BarOrientation.Vertical; }

                String strBarColor = GetStrAttribute(elControl, ProjXmlNames.BarColor);
                if (strBarColor.Length > 0) { bar.BarColor = (Media.Color)Media.ColorConverter.ConvertFromString(strBarColor); }

                control = bar;
            }

            // Add Channel
            if (control != null)
            {
                Int32 ChannelID = GetIntAttribute(elControl, ProjXmlNames.ChannelID);
                control.ChannelID = ChannelID;
            }

            return control;
        }

        private String GetStrAttribute(XmlNode node, String attributeName)
        {
            XmlNode attributeValue = node.Attributes.GetNamedItem(attributeName);
            return attributeValue != null ? attributeValue.Value : "";
        }

        private Boolean GetBoolAttribute(XmlNode node, String attributeName)
        {
            Boolean boolValue = false;
            XmlNode attributeValue = node.Attributes.GetNamedItem(attributeName);
            if (attributeValue != null) { boolValue = Boolean.Parse(attributeValue.Value.ToLower()); }
            return boolValue;
        }

        private Int32 GetIntAttribute(XmlNode node, String attributeName)
        {
            XmlNode attributeValue = node.Attributes.GetNamedItem(attributeName);
            return attributeValue != null ? Int32.Parse(attributeValue.Value) : 0;
        }

        private Double GetDoubleAttribute(XmlNode node, String attributeName)
        {
            XmlNode attributeValue = node.Attributes.GetNamedItem(attributeName);
            return attributeValue != null ? Double.Parse(attributeValue.Value) : 0D;
        }
    }
}
