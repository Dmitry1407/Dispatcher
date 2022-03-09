using System;
using System.Text;

namespace Core.service
{
    public class ProjXmlNames
    {
        // Element names
        public static readonly String TAG_GROUP = "TAG_GROUP";
        public static readonly String TAGS = "TAGS";
        public static readonly String TAG = "TAG";
        public static readonly String CHANNEL_GROUP = "CHANNEL_GROUP";
        public static readonly String CHANNELS = "CHANNELS";
        public static readonly String CHANNEL = "CHANNEL";
        public static readonly String PROJECT = "PROJECT";
        public static readonly String IOCHANNEL_GROUP = "IOCHANNEL_GROUP";
        public static readonly String IOCHANNELS = "IOCHANNELS";
        public static readonly String IOCHANNEL = "IOCHANNEL";
        public static readonly String PROPERTIES = "PROPERTIES";
        public static readonly String SOURCES = "SOURCES";
        public static readonly String SOURCE = "SOURCE";
        public static readonly String DRIVERS = "DRIVERS";
        public static readonly String DRIVER = "DRIVER";
        public static readonly String DEVICES = "DEVICES";
        public static readonly String DEVICE = "DEVICE";
        public static readonly String WINDOWS = "WINDOWS";
        public static readonly String WINDOW = "WINDOW";
        public static readonly String ELEMENTS = "ELEMENTS";
        public static readonly String ELEMENT = "ELEMENT";
        public static readonly String CONTROLS = "CONTROLS";
        public static readonly String CONTROL = "CONTROL";
        public static readonly String SHAPES = "SHAPES";
        public static readonly String SHAPE = "SHAPE";
        public static readonly String POINTS = "POINTS";
        public static readonly String POINT = "POINT";

        // Project Properties
        public static readonly String Name = "Name";
        public static readonly String Description = "Description";
        public static readonly String PollingTime = "PollingTime";
        public static readonly String RedrawTime = "RedrawTime";
        public static readonly String Grid = "Grid";
        public static readonly String GridSize = "GridSize";

        // Source Properties
        public static readonly String Type = "Type";
        public static readonly String Modbus = "Modbus";
        public static readonly String OPC = "OPC";
        public static readonly String SG = "SG";

        // Modbus Properties
        public static readonly String PortType = "PortType";
        public static readonly String TCP = "TCP";
        public static readonly String Serial = "Serial";
        public static readonly String SerialOverTCP = "SerialOverTCP";
        public static readonly String TCPAddress = "TCPAddress";
        public static readonly String TCPPort = "TCPPort";
        public static readonly String PortName = "PortName";
        public static readonly String BaudRate = "BaudRate";
        public static readonly String DataBits = "DataBits";
        public static readonly String Parity = "Parity";
        public static readonly String StopBits = "StopBits";
        public static readonly String Number = "Number";

        // Modbus Memory Map
        public static readonly String Status = "Status";
        public static readonly String Coils = "Coils";
        public static readonly String IRegisters = "IRegisters";
        public static readonly String HRegisters = "HRegisters";

        // Channel Types
        public static readonly String Group = "Group";
        public static readonly String Bit = "Bit";
        public static readonly String BitArray = "BitArray";
        public static readonly String Byte = "Byte";
        public static readonly String SByte = "SByte";
        public static readonly String Int_16 = "Int16";
        public static readonly String UInt_16 = "UInt16";
        public static readonly String Int_32 = "Int32";
        public static readonly String UInt_32 = "UInt32";
        public static readonly String Float = "Float";
        public static readonly String Double = "Double";

        // Channel Properties
        public static readonly String ID = "ID";
        public static readonly String Address = "Address";
        public static readonly String NumAddress = "NumAddress";
        public static readonly String StrAddress = "StrAddress";
        public static readonly String IsEditable = "IsEditable";
        public static readonly String IsEnable = "IsEnable";
        public static readonly String IsArchive = "IsArchive";
        public static readonly String MinALimit = "MinALimit";
        public static readonly String MinWLimit = "MinWLimit";
        public static readonly String MaxWLimit = "MaxWLimit";
        public static readonly String MaxALimit = "MaxALimit";
        public static readonly String SGType = "SGType";
        public static readonly String MinValue = "MinValue";
        public static readonly String MaxValue = "MaxValue";
        public static readonly String UpdateTime = "UpdateTime";

        // Generator types
        public static readonly String Sawtooth = "Sawtooth";
        public static readonly String Triangle = "Triangle";
        public static readonly String Square = "Square";
        public static readonly String Sine = "Sine";
        public static readonly String Random = "Random";

        // Window Properties
        public static readonly String X = "X";
        public static readonly String Y = "Y";
        public static readonly String Width = "Width";
        public static readonly String Height = "Height";
        public static readonly String BackColor = "BackColor";
        public static readonly String WindowType = "WindowType";
        public static readonly String FullScreen = "FullScreen";
        public static readonly String Floating = "Floating";

        // GElement Properties
        public static readonly String IsVisible = "IsVisible";
        public static readonly String IsVisibleFromChannel = "IsVisibleFromChannel";
        public static readonly String IsVisibleChannelID = "IsVisibleChannelID";

        // Shape Types
        public static readonly String Point = "Point";
        public static readonly String Line = "Line";
        public static readonly String Path = "Path";
        public static readonly String Circle = "Circle";
        public static readonly String Rectangle = "Rectangle";
        public static readonly String Polygon = "Polygon";
        public static readonly String Text = "Text";

        // Shape Properties
        public static readonly String DX = "DX";
        public static readonly String DY = "DY";
        public static readonly String Radius = "Radius";
        public static readonly String Round = "Round";
        public static readonly String ThicknessFromChannel = "ThicknessFromChannel";
        public static readonly String BackColorFromChannel = "BackColorFromChannel";
        public static readonly String BorderColorFromChannel = "BorderColorFromChannel";
        public static readonly String ThicknessChannelID = "ThicknessChannelID";
        public static readonly String BackColorChannelID = "BackColorChannelID";
        public static readonly String BorderColorChannelID = "BorderColorChannelID";
        public static readonly String Thickness = "Thickness";
        public static readonly String BorderColor = "BorderColor";
        public static readonly String ThicknessOFF = "ThicknessOFF";
        public static readonly String BackColorOFF = "BackColorOFF";
        public static readonly String BorderColorOFF = "BorderColorOFF";
        public static readonly String ThicknessON = "ThicknessON";
        public static readonly String BackColorON = "BackColorON";
        public static readonly String BorderColorON = "BorderColorON";
        public static readonly String BarColor = "BarColor";
        public static readonly String TextColor = "TextColor";
        public static readonly String Value = "Value";

        // Control Types
        public static readonly String BitLamp = "BitLamp";
        public static readonly String Button = "Button";
        public static readonly String NField = "NField";
        public static readonly String Bar = "Bar";

        // Control Properties
        public static readonly String TagID = "TagID";
        public static readonly String ChannelID = "ChannelID";
        public static readonly String Orientation = "Orientation";

        // Button Properties
        public static readonly String ButtonType = "ButtonType";
        public static readonly String Set = "Set";
        public static readonly String Reset = "Reset";
        public static readonly String Moment = "Moment";
        public static readonly String Toggle = "Toggle";
        public static readonly String ChangeWindow = "ChangeWindow";
        public static readonly String ChangedWindowID = "ChangedWindowID";
        public static readonly String TextON = "TextON";
        public static readonly String TextOFF = "TextOFF";
    }
}
