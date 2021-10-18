using System;
using DevTools.Core.ContainerService;
using Microsoft.Extensions.DependencyInjection;

namespace NetMQ.Server.Configuration
{
    public static class NetMqServerConfigurationExtensions
    {

        public static IContainerService AddNetMqServer(this IContainerService service)
        {
            service.AddDependenciesInjector<NetMqServerDependenciesInjector>();
            return service;
        }

        public static IServiceCollection AddNetMqServer(this IServiceCollection services, Action<ServerOptions> options)
        {
            services.Configure(options);
            services.AddHostedService<ServerStarter>();
            return services;
        }
    }
}