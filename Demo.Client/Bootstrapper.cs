using DevTools.Core.LoggerService;
using DevTools.Hosting.Bootstrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetMQ.Client.Configuration;
using Unity;

namespace Demo.Client
{
    public class Bootstrapper: BaseServiceBootstrapper
    {
        public Bootstrapper(IConfiguration configuration, IUnityContainer container, ILoggerService loggerService, ILoggerFactory loggerFactory) : base(configuration, container, loggerService, loggerFactory)
        {
        }

        protected override void InjectAdditionalDependencies()
        {
            ContainerService.AddNetMqClient();
        }
    }
}