using System.Threading.Tasks;
using MessagePack;

namespace NetMQ.Client
{
    public static class ClientExtensions
    {
        public static async Task<TResponse> SendRequestAsync<TRequest, TResponse>(this IClient client, string method, TRequest request)
        {
            var message = MessagePackSerializer.Serialize(request);
            var response = await client.SendRequestAsync(method, message);
            return MessagePackSerializer.Deserialize<TResponse>(response.Buffer);
        }
    }
}