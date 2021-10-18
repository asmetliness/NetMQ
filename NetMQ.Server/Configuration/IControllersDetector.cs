using System.Reflection;

namespace NetMQ.Server.Configuration
{
    internal interface IControllersDetector
    {
        void DetectControllers();

        void ConfigureAssemblies(Assembly[] assemblies);
    }
}