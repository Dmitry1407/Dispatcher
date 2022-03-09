using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Core.model.core.source
{
    public class SourceThread
    {
        private Thread thread;
        private Source source;
        private volatile Boolean isRun;

        public SourceThread(Source source)
        {
            this.source = source;
            thread = new Thread(this.Run);
            thread.Name = source.ToString() + " #" + source.ID;
        }

        public void Start()
        {
            Console.WriteLine(source.ToString() + "; POLL START!");

            isRun = true;
            thread.Start(source);
        }

        public void Stop()
        {
            isRun = false;
            if (thread != null && thread.IsAlive)
            {
                thread.Join();
            }
            Console.WriteLine(source.ToString() + "isRun = false");
        }

        private void Run(object obj)
        {
            source.PollingStart();
            Console.WriteLine("ThreadID = 0x{0:x}", AppDomain.GetCurrentThreadId());

            while (isRun)
            {
                source.Polling();
                Console.WriteLine(source.ToString() + "; POLL OK!");
                Thread.Sleep(source.PollingTime);
            }

            source.PollingStop();
            Console.WriteLine(source.ToString() + "; POLL STOP!");
        }
    }
}
