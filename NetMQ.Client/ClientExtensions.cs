using System;
using System.Threading.Tasks;
using MessagePack;

namespace NetMQ.Client
{
    public static class ClientExtensions
    {
        
        public static Task<TResponse> SendRequestAsync<TRequest, TResponse>(this IClient client, string method, TRequest request)
        {
            return SendRequestAsync<TRequest, TResponse>(client, method, request, TimeSpan.FromSeconds(30));
        }
        
        public static async Task<TResponse> SendRequestAsync<TRequest, TResponse>(this IClient client, string method, TRequest request, TimeSpan timeout)
        {
            var message = MessagePackSerializer.Serialize(request);
            var response = await client.SendRequestAsync(method, message, timeout);
            return MessagePackSerializer.Deserialize<TResponse>(response.Buffer);
        }
    }
}