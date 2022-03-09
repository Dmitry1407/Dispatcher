using System;
using System.Collections.Generic;
using System.Threading;

using Core.model.core.channel;

namespace Core.model.core.source.sg
{
    public class SGSRC : Source
    {
        public IDictionary<Int32, Channel> ChannelStorage { get; private set; }

        public SGSRC()
        {
            Type = SourceType.SG;
            Name = "Generator";
            IsEnable = true;
            ChannelStorage = new Dictionary<Int32, Channel>();
        }

        public void AddChannel(SGChannel channel)
        {
            if (channel != null)
            {
                ChannelStorage.Add(channel.ID, channel);
                Model.GetInstance().AddChannel(channel);
            }
        }

        public override void PollingStart()
        {
            foreach (SGChannel channel in ChannelStorage.Values)
            {
                channel.Init();
            }
        }

        public override void PollingStop() { }

        public override void Polling()
        {
            foreach (SGChannel channel in ChannelStorage.Values)
            {
                channel.UpdateValue();
                Model.GetInstance().ChArchive.Insert(channel.ID, channel.GetStringValue(), DateTime.Now.ToFileTime());
            }
        }
    }
}
