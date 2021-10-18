using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using DevTools.Hosting.Bootstrapper;
using DevTools.Hosting.Configuration;
using DevTools.Hosting.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMQ.Client;
using NetMQ.Client.Configuration;
using Unity.Microsoft.DependencyInjection;

namespace Demo.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.StartAsync();

            var client = host.Services.GetService<IClient>();


            var response = await client.SendRequestAsync<RequestDto, ResponseDto>("Test", RequestDto.Create());
            
            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => { config.DefaultConfig(); })
                .UseUnityServiceProvider()
                .UseSerilogLoggerService()
                .UseBootstrapper<Bootstrapper>()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddNetMqClient(options =>
                    {
                        options.ServerIps = new[] {"127.0.0.1:9999"};
                    });
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}