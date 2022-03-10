using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Core.model;
using Core.model.core.channel;
using Core.model.design.window;
using Core.model.design.graphics;
using Core.model.design.graphics.shape;
using Core.model.design.graphics.control;
using Core.model.core.source;
using Core.model.core.source.modbus;
using Core.model.core.source.opcda;
using Core.model.core.source.sg;

namespace Core.service
{
    public class ProjectWriter
    {
        private Model model;
        private XmlDocument xDocument;

        public ProjectWriter(Model model)
        {
            this.model = model;
        }

        public Boolean Write(String fileName)
        {
            Boolean writeSuccess = false;
            try
            {
                xDocument = new XmlDocument();
                XmlElement xRoot = xDocument.CreateElement(ProjXmlNames.PROJECT);

                WriteProperties(xRoot);
                WriteSources(xRoot);
                WriteWindows(xRoot);

                xDocument.AppendChild(xRoot);
                xDocument.Save(fileName);
                writeSuccess = true;
            }
            catch (Exception ex) { }
            return writeSuccess;
        }

        private void WriteProperties(XmlElement parent)
        {
            XmlElement elProperties = xDocument.CreateElement(ProjXmlNames.PROPERTIES);
            // Names
            AddAttribute(elProperties, ProjXmlNames.Name, model.Properties.Name);
            AddAttribute(elProperties, ProjXmlNames.Description, model.Properties.Description);

            // Timings
            if (model.Properties.PollingTime > 0 && model.Properties.PollingTime != ProjectProperties.DEFAULT_POLLING_TIME)
            {
                AddAttribute(elProperties, ProjXmlNames.PollingTime, model.Properties.PollingTime.ToString());
            }
            if (model.Properties.RedrawTime > 0 && model.Properties.RedrawTime != ProjectProperties.DEFAULT_REDRAW_TIME)
            {
                AddAttribute(elProperties, ProjXmlNames.RedrawTime, model.Properties.RedrawTime.ToString());
            }

            // Grig
            AddAttribute(elProperties, ProjXmlNames.Grid, model.Properties.IsGridDots.ToString());
            if (model.Properties.GridSize > 0 && model.Properties.GridSize != ProjectProperties.DEFAULT_GRID_SIZE)
            {
                AddAttribute(elProperties, ProjXmlNames.GridSize, model.Properties.GridSize.ToString());
            }

            parent.AppendChild(elProperties);
        }

        private void WriteSources(XmlElement parent)
        {
            XmlElement elSources = xDocument.CreateElement(ProjXmlNames.SOURCES);
            foreach (Source source in model.SourceStorage.Values)
            {
                XmlElement elSource = xDocument.CreateElement(ProjXmlNames.SOURCE);
                AddAttribute(elSource, ProjXmlNames.ID, source.ID.ToString());
                if (source.Name != null) { AddAttribute(elSource, ProjXmlNames.Name, source.Name); }
                AddAttribute(elSource, ProjXmlNames.Type, source.Type.ToString());

                switch (source.Type)
                {
                    case SourceType.Modbus:
                        ModbusSRC mbSRC = (ModbusSRC)source;
                        if (mbSRC.Driver != null && mbSRC.Driver.PortType == PortType.Serial)
                        {
                            AddAttribute(elSource, ProjXmlNames.PortType, mbSRC.Driver.PortType.ToString());
                            AddAttribute(elSource, ProjXmlNames.PortName, mbSRC.Driver.PortName);
                            AddAttribute(elSource, ProjXmlNames.BaudRate, mbSRC.Driver.BaudRate.ToString());
                            AddAttribute(elSource, ProjXmlNames.DataBits, mbSRC.Driver.DataBits.ToString());
                            AddAttribute(elSource, ProjXmlNames.Parity, ((Int32)mbSRC.Driver.Parity).ToString());
                            AddAttribute(elSource, ProjXmlNames.StopBits, ((Int32)mbSRC.Driver.StopBits).ToString());
                        }
                        else if (mbSRC.Driver != null && mbSRC.Driver.PortType == PortType.TCP)
                        {
                            AddAttribute(elSource, ProjXmlNames.PortType, mbSRC.Driver.PortType.ToString());
                            AddAttribute(elSource, ProjXmlNames.TCPAddress, mbSRC.Driver.TCPAddress);
                            AddAttribute(elSource, ProjXmlNames.TCPPort, mbSRC.Driver.TCPPort.ToString());
                        }
                        AddMBDevices(elSource, mbSRC.DevicesStorage);
                        break;
                    case SourceType.OPC:
                        OPCDASRC opcSRC = (OPCDASRC)source;
                        AddChannels(elSource, opcSRC.ChannelStorage);
                        break;
                    case SourceType.SG:
                        SGSRC sgSRC = (SGSRC)source;
                        AddChannels(elSource, sgSRC.ChannelStorage);
                        break;
                }
                elSources.AppendChild(elSource);
            }
            parent.AppendChild(elSources);
        }

        private void AddMBDevices(XmlElement parent, IDictionary<Int32, MBDevice> devicesStorage)
        {
            XmlElement elDevices = xDocument.CreateElement(ProjXmlNames.DEVICES);
            foreach (MBDevice device in devicesStorage.Values)
            {
                // Add Device
                XmlElement elDevice = xDocument.CreateElement(ProjXmlNames.DEVICE);
                AddAttribute(elDevice, ProjXmlNames.ID, device.ID.ToString());
                if (device.Name != null) { AddAttribute(elDevice, ProjXmlNames.Name, device.Name); }
                AddAttribute(elDevice, ProjXmlNames.Number, device.Number.ToString());

                // Add Channels
                XmlElement elChannels = xDocument.CreateElement(ProjXmlNames.CHANNELS);
                AddChannelGroup(elChannels, device.StatusStorage);
                AddChannelGroup(elChannels, device.CoilStorage);
                AddChannelGroup(elChannels, device.IRegisterStorage);
                AddChannelGroup(elChannels, device.HRegisterStorage);

                elDevice.AppendChild(elChannels);
                elDevices.AppendChild(elDevice);
            }
            parent.AppendChild(elDevices);
        }

        private void WriteWindows(XmlElement parent)
        {
            XmlElement elWindows = xDocument.CreateElement(ProjXmlNames.WINDOWS);
            foreach (Window window in model.WindowStorage.Values)
            {
                XmlElement elWindow = xDocument.CreateElement(ProjXmlNames.WINDOW);
                AddAttribute(elWindow, ProjXmlNames.ID, window.ID.ToString());
                if (window.Description != null) AddAttribute(elWindow, ProjXmlNames.Description, window.Description);

                AddAttribute(elWindow, ProjXmlNames.X, window.X.ToString());
                AddAttribute(elWindow, ProjXmlNames.Y, window.X.ToString());
                AddAttribute(elWindow, ProjXmlNames.Width, window.Width.ToString());
                AddAttribute(elWindow, ProjXmlNames.Height, window.Height.ToString());
                if (window.Name != null) { AddAttribute(elWindow, ProjXmlNames.Name, window.Name); }
                AddAttribute(elWindow, ProjXmlNames.BackColor, window.BackColor.ToString());
                AddAttribute(elWindow, ProjXmlNames.WindowType, window.Type.ToString());

                AddGElements(elWindow, window);
                elWindows.AppendChild(elWindow);
            }
            parent.AppendChild(elWindows);
        }

        private void AddChannels(XmlElement parent, IDictionary<Int32, Channel> channelStorage)
        {
            XmlElement elChannels = xDocument.CreateElement(ProjXmlNames.CHANNELS);
            foreach (Channel channel in channelStorage.Values)
            {
                if (channel.Type == ChannelType.Group)
                {
                    AddChannelGroup(elChannels, (ChannelGroup)channel);
                }
                else
                {
                    // Add Channel
                    XmlElement elChannel = xDocument.CreateElement(ProjXmlNames.CHANNEL);
                    AddAttribute(elChannel, ProjXmlNames.ID, channel.ID.ToString());
                    if (channel.NumAddress > 0) { AddAttribute(elChannel, ProjXmlNames.NumAddress, channel.NumAddress.ToString()); }
                    else if (channel.StrAddress != null && channel.StrAddress.Length > 0) { AddAttribute(elChannel, ProjXmlNames.StrAddress, channel.StrAddress); }
                    AddAttribute(elChannel, ProjXmlNames.Name, channel.Name);
                    AddAttribute(elChannel, ProjXmlNames.Description, channel.Description);
                    AddAttribute(elChannel, ProjXmlNames.Type, channel.Type.ToString());
                    AddAttribute(elChannel, ProjXmlNames.IsEnable, channel.IsEnable.ToString());
                    AddAttribute(elChannel, ProjXmlNames.IsArchive, channel.IsArchive.ToString());

                    // Add AlarmLimit and WarningLimit
                    switch (channel.Type)
                    {
                        case ChannelType.Byte:
                            AddAttribute(elChannel, ProjXmlNames.MinALimit, ((ByteChannel)channel).MinAlarmLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinWLimit, ((ByteChannel)channel).MinWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxWLimit, ((ByteChannel)channel).MaxWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxALimit, ((ByteChannel)channel).MaxAlarmLimit.ToString());
                            break;

                        case ChannelType.SByte:
                            AddAttribute(elChannel, ProjXmlNames.MinALimit, ((SByteChannel)channel).MinAlarmLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinWLimit, ((SByteChannel)channel).MinWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxWLimit, ((SByteChannel)channel).MaxWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxALimit, ((SByteChannel)channel).MaxAlarmLimit.ToString());
                            break;

                        case ChannelType.Int16:
                            AddAttribute(elChannel, ProjXmlNames.MinALimit, ((Int16Channel)channel).MinAlarmLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinWLimit, ((Int16Channel)channel).MinWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxWLimit, ((Int16Channel)channel).MaxWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxALimit, ((Int16Channel)channel).MaxAlarmLimit.ToString());
                            break;

                        case ChannelType.UInt16:
                            AddAttribute(elChannel, ProjXmlNames.MinALimit, ((UInt16Channel)channel).MinAlarmLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinWLimit, ((UInt16Channel)channel).MinWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxWLimit, ((UInt16Channel)channel).MaxWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxALimit, ((UInt16Channel)channel).MaxAlarmLimit.ToString());
                            break;

                        case ChannelType.Int32:
                            AddAttribute(elChannel, ProjXmlNames.MinALimit, ((Int32Channel)channel).MinAlarmLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinWLimit, ((Int32Channel)channel).MinWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxWLimit, ((Int32Channel)channel).MaxWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxALimit, ((Int32Channel)channel).MaxAlarmLimit.ToString());
                            break;

                        case ChannelType.UInt32:
                            AddAttribute(elChannel, ProjXmlNames.MinALimit, ((UInt32Channel)channel).MinAlarmLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinWLimit, ((UInt32Channel)channel).MinWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxWLimit, ((UInt32Channel)channel).MaxWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxALimit, ((UInt32Channel)channel).MaxAlarmLimit.ToString());
                            break;

                        case ChannelType.Float:
                            AddAttribute(elChannel, ProjXmlNames.MinALimit, ((FloatChannel)channel).MinAlarmLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinWLimit, ((FloatChannel)channel).MinWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxWLimit, ((FloatChannel)channel).MaxWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxALimit, ((FloatChannel)channel).MaxAlarmLimit.ToString());
                            break;

                        case ChannelType.Double:
                            AddAttribute(elChannel, ProjXmlNames.MinALimit, ((DoubleChannel)channel).MinAlarmLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinWLimit, ((DoubleChannel)channel).MinWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxWLimit, ((DoubleChannel)channel).MaxWarningLimit.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxALimit, ((DoubleChannel)channel).MaxAlarmLimit.ToString());
                            break;

                        case ChannelType.SG:
                            AddAttribute(elChannel, ProjXmlNames.SGType, ((SGChannel)channel).SGType.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MinValue, ((SGChannel)channel).MinValue.ToString());
                            AddAttribute(elChannel, ProjXmlNames.MaxValue, ((SGChannel)channel).MaxValue.ToString());
                            break;

                        default:
                            break;
                    }
                    elChannels.AppendChild(elChannel);
                }
            }
            parent.AppendChild(elChannels);
        }

        private void AddChannelGroup(XmlElement parent, ChannelGroup channelGroup)
        {
            XmlElement elGroupChannel = xDocument.CreateElement(ProjXmlNames.CHANNEL);

            AddAttribute(elGroupChannel, ProjXmlNames.ID, channelGroup.ID.ToString());
            AddAttribute(elGroupChannel, ProjXmlNames.Name, channelGroup.Name);
            AddAttribute(elGroupChannel, ProjXmlNames.Description, channelGroup.Description);
            AddAttribute(elGroupChannel, ProjXmlNames.Type, channelGroup.Type.ToString());
            if (channelGroup.ChannelStorage != null && channelGroup.ChannelStorage.Count > 0)
            {
                AddChannels(elGroupChannel, channelGroup.ChannelStorage);
            }

            parent.AppendChild(elGroupChannel);
        }

        private void AddGElements(XmlElement parent, Window window)
        {
            XmlElement elGElements = xDocument.CreateElement(ProjXmlNames.ELEMENTS);
            foreach (GElement element in window.ElementStorage.Values)
            {
                XmlElement elGElement = null;
                if (element is Control)
                {
                    elGElement = xDocument.CreateElement(ProjXmlNames.CONTROL);
                    AddControl(elGElement, (Control)element);
                }
                else if (element is Shape)
                {
                    elGElement = xDocument.CreateElement(ProjXmlNames.SHAPE);
                    AddShape(elGElement, (Shape)element);
                }

                AddAttribute(elGElement, ProjXmlNames.IsVisible, element.IsVisible.ToString());
                AddAttribute(elGElement, ProjXmlNames.IsVisibleFromChannel, element.IsVisibleFromChannel.ToString());
                AddAttribute(elGElement, ProjXmlNames.IsVisibleChannelID, element.IsVisibleChannelID.ToString());

                AddAttribute(elGElement, ProjXmlNames.Thickness, element.Thickness.ToString());
                AddAttribute(elGElement, ProjXmlNames.BackColor, element.BackColor.ToString());
                AddAttribute(elGElement, ProjXmlNames.BorderColor, element.BorderColor.ToString());

                elGElements.AppendChild(elGElement);
            }
            parent.AppendChild(elGElements);
        }

        private void AddShape(XmlElement elGElement, Shape shape)
        {
            AddAttribute(elGElement, ProjXmlNames.Type, shape.SType.ToString());
            AddAttribute(elGElement, ProjXmlNames.X, shape.X.ToString());
            AddAttribute(elGElement, ProjXmlNames.Y, shape.Y.ToString());

            switch (shape.SType)
            {
                case ShapeType.Line:
                    Line line = (Line)shape;
                    AddAttribute(elGElement, ProjXmlNames.DX, line.DX.ToString());
                    AddAttribute(elGElement, ProjXmlNames.DY, line.DY.ToString());
                    break;

                case ShapeType.Path:
                    XmlElement elPathPoints = xDocument.CreateElement(ProjXmlNames.POINTS);
                    foreach (Point point in ((Path)shape).Points)
                    {
                        XmlElement elPoint = xDocument.CreateElement(ProjXmlNames.POINT);
                        AddAttribute(elPoint, ProjXmlNames.X, point.X.ToString());
                        AddAttribute(elPoint, ProjXmlNames.Y, point.Y.ToString());
                        elPathPoints.AppendChild(elPoint);
                    }
                    elGElement.AppendChild(elPathPoints);
                    break;

                case ShapeType.Circle:
                    AddAttribute(elGElement, ProjXmlNames.Radius, ((Circle)shape).Radius.ToString());
                    break;

                case ShapeType.Rectangle:
                    Rectangle rectangle = (Rectangle)shape;
                    AddAttribute(elGElement, ProjXmlNames.Width, rectangle.Width.ToString());
                    AddAttribute(elGElement, ProjXmlNames.Height, rectangle.Height.ToString());
                    AddAttribute(elGElement, ProjXmlNames.Round, rectangle.Round.ToString());
                    break;

                case ShapeType.Polygon:
                    XmlElement elPolygonPoints = xDocument.CreateElement(ProjXmlNames.POINTS);
                    foreach (Point point in ((Polygon)shape).Points)
                    {
                        XmlElement elPoint = xDocument.CreateElement(ProjXmlNames.POINT);
                        AddAttribute(elPoint, ProjXmlNames.X, point.X.ToString());
                        AddAttribute(elPoint, ProjXmlNames.Y, point.Y.ToString());
                        elPolygonPoints.AppendChild(elPoint);
                    }
                    elGElement.AppendChild(elPolygonPoints);
                    break;

                case ShapeType.Text:
                    AddAttribute(elGElement, ProjXmlNames.Value, ((Text)shape).Value.ToString());
                    break;

                default:
                    break;
            }

            AddAttribute(elGElement, ProjXmlNames.ThicknessOFF, shape.ThicknessOFF.ToString());
            AddAttribute(elGElement, ProjXmlNames.BackColorOFF, shape.BackColorOFF.ToString());
            AddAttribute(elGElement, ProjXmlNames.BorderColorOFF, shape.BorderColorOFF.ToString());

            AddAttribute(elGElement, ProjXmlNames.ThicknessON, shape.ThicknessON.ToString());
            AddAttribute(elGElement, ProjXmlNames.BackColorON, shape.BackColorON.ToString());
            AddAttribute(elGElement, ProjXmlNames.BorderColorON, shape.BorderColorON.ToString());

            AddAttribute(elGElement, ProjXmlNames.ThicknessFromChannel, shape.ThicknessFromChannel.ToString());
            AddAttribute(elGElement, ProjXmlNames.BackColorFromChannel, shape.BackColorFromChannel.ToString());
            AddAttribute(elGElement, ProjXmlNames.BorderColorFromChannel, shape.BorderColorFromChannel.ToString());

            AddAttribute(elGElement, ProjXmlNames.ThicknessChannelID, shape.ThicknessChannelID.ToString());
            AddAttribute(elGElement, ProjXmlNames.BackColorChannelID, shape.BackColorChannelID.ToString());
            AddAttribute(elGElement, ProjXmlNames.BorderColorChannelID, shape.BorderColorChannelID.ToString());
        }

        private void AddControl(XmlElement elGElement, Control control)
        {
            AddAttribute(elGElement, ProjXmlNames.Type, control.CType.ToString());
            AddAttribute(elGElement, ProjXmlNames.X, control.X.ToString());
            AddAttribute(elGElement, ProjXmlNames.Y, control.Y.ToString());
            AddAttribute(elGElement, ProjXmlNames.Width, control.Width.ToString());
            AddAttribute(elGElement, ProjXmlNames.Height, control.Height.ToString());
            AddAttribute(elGElement, ProjXmlNames.ChannelID, control.ChannelID.ToString());

            switch (control.CType)
            {
                case ControlType.Button:
                    Button button = (Button)control;
                    AddAttribute(elGElement, ProjXmlNames.ButtonType, button.ButtonType.ToString());
                    if (button.ChangedWindowID > 0) { AddAttribute(elGElement, ProjXmlNames.ChangedWindowID, button.ChangedWindowID.ToString()); }
                    if (button.TextOFF != null && button.TextOFF.Length > 0 && !button.TextOFF.Equals(Button.TEXT_OFF)) { AddAttribute(elGElement, ProjXmlNames.TextOFF, button.TextOFF); }
                    if (button.TextON != null && button.TextON.Length > 0 && !button.TextON.Equals(Button.TEXT_ON)) { AddAttribute(elGElement, ProjXmlNames.TextON, button.TextON); }
                    AddAttribute(elGElement, ProjXmlNames.BackColorOFF, button.BackColorOFF.ToString());
                    AddAttribute(elGElement, ProjXmlNames.BackColorON, button.BackColorON.ToString());
                    AddAttribute(elGElement, ProjXmlNames.Round, button.Round.ToString());
                    break;

                case ControlType.NField:
                    AddAttribute(elGElement, ProjXmlNames.TextColor, ((NField)control).TextColor.ToString());
                    break;

                case ControlType.Bar:
                    Bar bar = (Bar)control;
                    AddAttribute(elGElement, ProjXmlNames.Orientation, bar.Orientation.ToString());
                    AddAttribute(elGElement, ProjXmlNames.BarColor, bar.BarColor.ToString());
                    break;

                default:
                    break;
            }
        }

        private void AddAttribute(XmlElement parent, String attrName, String attrValue)
        {
            XmlAttribute attribute = xDocument.CreateAttribute(attrName);
            attribute.AppendChild(xDocument.CreateTextNode(attrValue));
            parent.Attributes.Append(attribute);
        }
    }
}
