using System;
using System.Text;

namespace Core.model.core.archive
{
    public class ChannelsArchiveItem : ArchiveItem
    {
        public String ChannelValue { get; set; }

        public ChannelsArchiveItem()
        {
            Type = ArchiveType.Channels;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("[Channel]");
            sb.Append(" [ID=").Append(ChannelID);
            sb.Append("] [VAL=").Append(ChannelValue);
            sb.Append("] [TS=").Append(DateTime.FromFileTime(TimeStamp)).Append("]");
            return sb.ToString();
        }
    }
}
