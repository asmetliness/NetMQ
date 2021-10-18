using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevTools.DapperExtensions.Tracing;
using DevTools.Hosting.Bootstrapper;
using DevTools.Hosting.Configuration;
using DevTools.Hosting.Logging;
using DevTools.Hosting.Tracing;
using Infrastructure.Consul;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMQ.Server.Configuration;
using Unity.Microsoft.DependencyInjection;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => { config.DefaultConfig(); })
                .UseUnityServiceProvider()
                .UseSerilogLoggerService()
                .UseBootstrapper<Bootstrapper>()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddNetMqServer(options =>
                    {
                        options.Ip = "127.0.0.1:9999";
                        options.WorkersCount = 10;
                    });
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}