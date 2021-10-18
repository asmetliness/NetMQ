using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace NetMQ.Client.Configuration
{
    public class ClientStarter: IHostedService
    {
        private readonly IClient _client;
        
        public ClientStarter(IClient client)
        {
            _client = client;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.Initialize();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}