using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetMQ.Client.Configuration;
using NetMQ.Core;
using NetMQ.Sockets;

namespace NetMQ.Client
{
    public class SingleThreadPollerClient: IClient
    {
        private readonly NetMQPoller _poller;
        private readonly DealerSocket _dealerSocket;
        private readonly NetMQQueue<NetMQMessage> _queue;
        private readonly IOptions<ClientOptions> _options;

        private static readonly ConcurrentDictionary<string, TaskCompletionSource<NetMQFrame>> Requests = new();

        public SingleThreadPollerClient(IOptions<ClientOptions> options)
        {
            _options = options;
            _poller = new NetMQPoller();
            _dealerSocket = new DealerSocket();
            _queue = new NetMQQueue<NetMQMessage>();
            
            _poller.Add(_dealerSocket);
            _poller.Add(_queue);

            _dealerSocket.Options.Identity =
                Encoding.Unicode.GetBytes(Guid.NewGuid().ToString());

            _dealerSocket.ReceiveReady += OnClientReceiveReady;
            _queue.ReceiveReady += OnQueueReceiveReady;
        }

        public void Initialize()
        {
            foreach (var server in _options.Value.ServerIps)      
            {
                _dealerSocket.Connect(Helpers.ConvertIp(server));
            }
            _poller.RunAsync();
        }
        

        public async Task<NetMQFrame> SendRequestAsync(string method, byte[] request, TimeSpan timeout)
        {


                var metadata = new RequestMetadata()
                {
                    RequestDtoKey = method,
                    RequestId = Guid.NewGuid().ToString()
                };

                var message = new NetMQMessage();
                message.AppendEmptyFrame();
                message.Append(metadata.ToMessagePack());
                message.Append(request);

                using var cts = new CancellationTokenSource();
                var tcs = new TaskCompletionSource<NetMQFrame>();

                cts.CancelAfter(timeout);

                using var registration = cts.Token.Register(() => tcs.TrySetCanceled(cts.Token));

                Requests[metadata.RequestId] = tcs;

                _queue.Enqueue(message);

                try
                {
                    return await tcs.Task;
                }
                catch (OperationCanceledException ex)
                {
                    if (cts.IsCancellationRequested)
                    {
                        throw new TimeoutException($"Timeout", ex);
                    }
                    throw;
                }
            
        }

        public void PostRequest(string method, byte[] request)
        {
            var metadata = new RequestMetadata()
            {
                RequestDtoKey = method,
                RequestId = Guid.NewGuid().ToString()
            };

            var message = new NetMQMessage();
            message.AppendEmptyFrame();
            message.Append(metadata.ToMessagePack());
            message.Append(request);
            
            _queue.Enqueue(message);
        }

        public void Dispose()
        {
            _poller?.Dispose();
        }

        private void OnClientReceiveReady(object sender, NetMQSocketEventArgs eventArgs)
        {
            NetMQMessage message = null;
            while (eventArgs.Socket.TryReceiveMultipartMessage(ref message))
            {
                var metadata = message[1].FromMessagePack<ResponseMetadata>();
                var response = message[2];
                
                if (metadata.RequestId != null && Requests.TryRemove(metadata.RequestId, out var tcs))
                {
                    tcs.TrySetResult(response);
                }
            }
        }

        private void OnQueueReceiveReady(object sender, NetMQQueueEventArgs<NetMQMessage> eventArgs)
        {
            
            _dealerSocket.SendMultipartMessage(eventArgs.Queue.Dequeue());
        }
    }
}