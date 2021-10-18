using DevTools.Core.ContainerService.DependenciesInjector;
using DevTools.Core.ContainerService.RegisterService;

namespace NetMQ.Client
{
    public class NetMqClientDependenciesInjector: IDependenciesInjector
    {
        private readonly IRegisterService _registerService;

        public NetMqClientDependenciesInjector(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        public void Inject()
        {
            _registerService.RegisterSingleton<IClient, SingleThreadPollerClient>();
        }
    }
}