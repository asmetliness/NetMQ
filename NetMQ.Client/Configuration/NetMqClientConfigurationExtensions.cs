using System;
using DevTools.Core.ContainerService;
using Microsoft.Extensions.DependencyInjection;

namespace NetMQ.Client.Configuration
{
    public static class NetMqClientConfigurationExtensions
    {
        public static IContainerService AddNetMqClient(this IContainerService container)
        {
            container.AddDependenciesInjector<NetMqClientDependenciesInjector>();
            return container;
        }


        public static IServiceCollection AddNetMqClient(this IServiceCollection services, Action<ClientOptions> config)
        {
            services.Configure(config);
            services.AddHostedService<ClientStarter>();
            return services;
        }
    }
}