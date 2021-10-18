using System.Reflection;

namespace NetMQ.Server.Configuration
{
    public class ServerOptions
    {
        public string Ip { get; set; }

        public int WorkersCount { get; set; } = 1;

        public Assembly[] ControllerAssemblies { get; set; } = null;
    }
}