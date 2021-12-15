using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevTools.Core.LoggerService;
using DevTools.Core.LoggerService.NullableLogging;
using Microsoft.Extensions.Options;
using NetMQ.Core;
using NetMQ.Server.Configuration;
using NetMQ.Server.Controllers;
using NetMQ.Server.Controllers.Manager;
using NetMQ.Sockets;

namespace NetMQ.Server.Server
{
    internal class Server: IServer
    {

        private const string WorkersIp = "inproc://workers";
        
        private readonly string _address;
        private readonly Proxy _proxy;
        private readonly DealerSocket _workersProxy;
        private readonly RouterSocket _router;
        private readonly NetMQPoller _poller;
        private readonly IOptions<ServerOptions> _options;

        private readonly WrappedLogger _logger;

        private readonly List<Task> _workers;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly IControllerManager _controllerManager;

        public Server(IOptions<ServerOptions> options,  IControllerManager controllerManager, ILoggerService loggerService)
        {
            _logger = loggerService.GetLogger(nameof(IServer)).Wrap();
            _controllerManager = controllerManager;
            _address = Helpers.ConvertIp(options.Value.Ip);
            _options = options;

            _workersProxy = new DealerSocket();
            _router = new RouterSocket();

            _poller = new NetMQPoller();
            _poller.Add(_router);
            _poller.Add(_workersProxy);
            
            _proxy = new Proxy(_router, _workersProxy, null, _poller);

            _workers = new List<Task>(options.Value.WorkersCount);
            
        }

        
        
        public void Start()
        {
            _router.Bind(_address);
            _workersProxy.Bind(WorkersIp);

            for (int i = 0; i < _options.Value.WorkersCount; ++i)
            {
                _workers.Add(Task.Run(Worker));
            }
            
            _poller.RunAsync();
            _proxy.Start();

        }

        public void Stop()
        {
            _poller.Stop();
            _proxy.Stop();
        }

        public void Dispose()
        {
            _cts.Cancel();
            _poller?.Dispose();
            _proxy.Stop();
        }


        private async Task Worker()
        {
            using (var responseSocket = new ResponseSocket())
            {
                responseSocket.Connect(WorkersIp);

                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var message = responseSocket.ReceiveMultipartMessage();

                        var requestMetadata = message[0].FromMessagePack<RequestMetadata>();

                        try
                        {
                            if (_controllerManager.TryGetController(requestMetadata.RequestDtoKey, out var controller))
                            {
                                var requestFrame = message[1];

                                var response = await controller.HandleRequest(requestFrame)
                                        .ConfigureAwait(false);
     
                            
                                var responseMessage = new NetMQMessage();
                                var responseMetadata = ResponseMetadata.CreateSuccess(requestMetadata);
                                responseMessage.Append(responseMetadata.ToMessagePack());
                                responseMessage.Append(response);

                                responseSocket.SendMultipartMessage(responseMessage);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error?.Log(ex);
                            
                            var response = new NetMQMessage();
                            var responseMetadata = ResponseMetadata.CreateError(requestMetadata, ex);
                            response.Append(responseMetadata.ToMessagePack());
                            
                            responseSocket.SendMultipartMessage(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        //we dont know RequestId from request metadata.
                        //we still can send request to the client, but it wont be able to match request and error-response
                        _logger.Error?.Log(ex);
                    }

                }   
            }
        }

        



        
    }
}