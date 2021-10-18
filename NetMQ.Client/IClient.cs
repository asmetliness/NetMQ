﻿using System;
using System.Threading.Tasks;

namespace NetMQ.Client
{
    public interface IClient: IDisposable
    {

        void Initialize();
        
        Task<NetMQFrame> SendRequestAsync(string method, byte[] message);
    }
}