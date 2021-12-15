using System;
using System.Linq;
using System.Reflection;
using NetMQ.Server.Controllers;
using NetMQ.Server.Controllers.Manager;

namespace NetMQ.Server.Configuration
{
    internal class ControllersDetector: IControllersDetector
    {
        private Assembly[] _assemblies;
        private readonly IControllerManager _controllerManager;

        public ControllersDetector(IControllerManager server)
        {
            _controllerManager = server;
            _assemblies = new[] {Assembly.GetEntryAssembly()};
        }
        
        public void DetectControllers()
        {
            foreach (var assembly in _assemblies)
            {
                var controllerTypes = assembly
                    .DefinedTypes
                    .Where(t => t.GetInterfaces().Any(i => i.Name == nameof(IServerController)))
                    .ToList();

                foreach (var controller in controllerTypes)
                {
                    _controllerManager.AddController(controller);
                }
            }
        }

        public void ConfigureAssemblies(Assembly[] assemblies)
        {
            _assemblies = _assemblies.Concat(assemblies).Distinct().ToArray();
        }
    }
}