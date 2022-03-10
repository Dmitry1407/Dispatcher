using System;
using System.Text;

namespace Core.model.core.archive
{
    public class EventsArchiveItem : ArchiveItem
    {
        public String EventValue { get; set; }

        public EventsArchiveItem()
        {
            Type = ArchiveType.Events;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("[Event]");
            sb.Append(" [ID=").Append(ChannelID);
            sb.Append("] [VAL=").Append(EventValue);
            sb.Append("] [TS=").Append(DateTime.FromFileTime(TimeStamp)).Append("]");
            return sb.ToString();
        }
    }
}
