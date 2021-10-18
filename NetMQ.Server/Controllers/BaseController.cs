using System;
using System.Threading.Tasks;
using MessagePack;

namespace NetMQ.Server.Controllers
{
    public abstract class BaseController<TRequest, TResponse>: IServerController
    {
        public abstract string RequestDtoKey { get; }
        
        public async Task<byte[]> HandleRequest(NetMQFrame requestFrame)
        {
            var request = MessagePackSerializer.Deserialize<TRequest>(requestFrame.Buffer);
            var response = await Handle(request);
            return MessagePackSerializer.Serialize(response);
        }
        public abstract Task<TResponse> Handle(TRequest request);
    }
}