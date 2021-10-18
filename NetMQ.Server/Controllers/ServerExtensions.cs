using NetMQ.Server.Controllers.Manager;

namespace NetMQ.Server.Controllers
{
    public static class ControllerManagerExtensions
    {
        public static void AddController<TController>(this IControllerManager server) where TController : IServerController
        {
            server.AddController(typeof(TController));
        }
    }
}