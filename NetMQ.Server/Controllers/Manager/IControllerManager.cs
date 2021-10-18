using System;

namespace NetMQ.Server.Controllers.Manager
{
    public interface IControllerManager
    {
        public void AddController(Type controllerType);

        public bool TryGetController(string key, out IServerController controller);

    }
}