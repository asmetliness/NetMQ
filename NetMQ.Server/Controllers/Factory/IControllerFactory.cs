using System;

namespace NetMQ.Server.Controllers.Factory
{
    internal interface IControllerFactory
    {
        IServerController Create(Type controllerType);
    }
}