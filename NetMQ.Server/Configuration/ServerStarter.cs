using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetMQ.Server.Server;

namespace NetMQ.Server.Configuration
{
    internal class ServerStarter: IHostedService
    {
        private readonly IControllersDetector _detector;
        private readonly IOptions<ServerOptions> _options;
        private readonly IServer _server;

        public ServerStarter(IControllersDetector detector, IOptions<ServerOptions> options, IServer server)
        {
            _detector = detector;
            _options = options;
            _server = server;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.Value.ControllerAssemblies != null)
            {
                _detector.ConfigureAssemblies(_options.Value.ControllerAssemblies);
            }
            _detector.DetectControllers();
            _server.Start();
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _server.Stop();
            return Task.CompletedTask;
        }
    }
}