using System;
using DevTools.Core.ContainerService.ResolveService;

namespace NetMQ.Server.Controllers.Factory
{
    internal class ControllerFactory: IControllerFactory
    {
        private readonly IResolveService _resolver;

        public ControllerFactory(IResolveService provider)
        {
            _resolver = provider;
        }

        public IServerController Create(Type controllerType)
        {
            return (IServerController)_resolver.ResolveItem(controllerType);
        }
    }
}