using System;

namespace NetMQ.Server.Server
{
    public interface IServer: IDisposable
    {
        
        void Start();

        void Stop();
    }
}