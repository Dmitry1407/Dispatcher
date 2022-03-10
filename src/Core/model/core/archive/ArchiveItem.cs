using System;

namespace Core.model.core.archive
{
    public class ArchiveItem
    {
        public ArchiveType Type { get; protected set; }
        public Int32 ChannelID { get; set; }
        public Int64 TimeStamp { get; set; }
    }
}
