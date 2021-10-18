using System;
using System.Collections.Concurrent;
using NetMQ.Server.Controllers.Factory;

namespace NetMQ.Server.Controllers.Manager
{
    internal class ControllerManager: IControllerManager
    {
        private static readonly ConcurrentDictionary<string, IServerController>
            ControllerRegistrations = new();

        private readonly IControllerFactory _controllerFactory;

        public ControllerManager(IControllerFactory controllerFactory)
        {
            _controllerFactory = controllerFactory;
        }
        
        public void AddController(Type controllerType)
        {
            var controllerInstance = _controllerFactory.Create(controllerType);
            ControllerRegistrations[controllerInstance.RequestDtoKey] = controllerInstance;
        }

        public bool TryGetController(string methodName, out IServerController controller)
        {
            return ControllerRegistrations.TryGetValue(methodName, out controller);
        }
    }
}