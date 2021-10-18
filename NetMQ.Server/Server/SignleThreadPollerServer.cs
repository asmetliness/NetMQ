using Microsoft.Extensions.Options;
using NetMQ.Core;
using NetMQ.Server.Configuration;
using NetMQ.Server.Controllers;
using NetMQ.Server.Controllers.Manager;
using NetMQ.Sockets;

namespace NetMQ.Server.Server
{
    internal class SingleThreadPollerServer: IServer
    {

        private readonly NetMQPoller _poller;
        private readonly NetMQQueue<NetMQMessage> _queue;
        private readonly RouterSocket _router;
        private IOptions<ServerOptions> _options;

        private readonly IControllerManager _controllerManager;

        public SingleThreadPollerServer(IOptions<ServerOptions> options,  IControllerManager controllerManager)
        {
            _options = options;
            _controllerManager = controllerManager;
            
            _poller = new NetMQPoller();
            _queue = new NetMQQueue<NetMQMessage>();
            _router = new RouterSocket();

            _router.ReceiveReady += OnRouterReceiveReady;
            _queue.ReceiveReady += OnQueueReceiveReady;
            
            _poller.Add(_queue);
            _poller.Add(_router);
        }

        
        
        public void Start()
        {
            _router.Bind(Helpers.ConvertIp(_options.Value.Ip));
            _poller.RunAsync();
        }

        public void Stop()
        {
            _poller.StopAsync();
        }

        public void Dispose()
        {
            _poller?.Dispose();
        }


        private void OnRouterReceiveReady(object sender, NetMQSocketEventArgs eventArgs)
        {
            NetMQMessage message = null;
            var router = eventArgs.Socket;
            while (router.TryReceiveMultipartMessage(ref message))
            {
                if (message.FrameCount == 4)
                {
                    var clientAddress = message[0];
                    var metadata = message[2].FromMessagePack<RequestMetadata>();

                    if (_controllerManager.TryGetController(metadata.RequestDtoKey, out var controller))
                    {
                        var requestFrame = message[3];

                        var response = controller.HandleRequest(requestFrame)
                            .GetAwaiter().GetResult();

                        var responseMessage = new NetMQMessage();
                        responseMessage.Append(clientAddress);
                        responseMessage.AppendEmptyFrame();
                        responseMessage.Append(metadata.ToMessagePack());
                        responseMessage.Append(response);
                        
                        _queue.Enqueue(responseMessage);
                    }
                }
            }
        }


        private void OnQueueReceiveReady(object sender, NetMQQueueEventArgs<NetMQMessage> eventArgs)
        {
            _router.SendMultipartMessage(eventArgs.Queue.Dequeue());
        }
        
        
        
    }
}