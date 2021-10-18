using System.Threading.Tasks;

namespace NetMQ.Server.Controllers
{
    public interface IServerController
    {
        public string RequestDtoKey { get; }
        public Task<byte[]> HandleRequest(NetMQFrame request);
    }
}