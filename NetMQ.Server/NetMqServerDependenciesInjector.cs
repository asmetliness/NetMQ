using DevTools.Core.ContainerService.DependenciesInjector;
using DevTools.Core.ContainerService.RegisterService;
using NetMQ.Server.Configuration;
using NetMQ.Server.Controllers;
using NetMQ.Server.Controllers.Factory;
using NetMQ.Server.Controllers.Manager;
using NetMQ.Server.Server;

namespace NetMQ.Server
{
    public class NetMqServerDependenciesInjector: IDependenciesInjector
    {
        private readonly IRegisterService _registerService;

        public NetMqServerDependenciesInjector(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        public void Inject()
        {
            _registerService.RegisterSingleton<IControllerFactory, ControllerFactory>();
            _registerService.RegisterSingleton<IControllerManager, ControllerManager>();
            _registerService.RegisterSingleton<IControllersDetector, ControllersDetector>();
            _registerService.RegisterSingleton<IServer, Server.Server>();
        }
    }
}