using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.Builders
{
    public class ServiceContainerOptionsBuilder
    {
        private readonly List<ServiceDescriptor> _descriptors = new();


        public ServiceContainerOptions Build() => new (_descriptors);

        public ServiceContainerOptionsBuilder Register<TRegister, TImplementation>()
            where TImplementation : TRegister
            => Register(typeof(TRegister), typeof(TImplementation));
        
        public ServiceContainerOptionsBuilder Register(Type registrationType, Type implementationType)
        {
            if (!registrationType.IsAssignableFrom(implementationType))
                throw new ArgumentException($"Service {implementationType.Name} is not assignable to {registrationType.Name}");
            _descriptors.Add(new ServiceDescriptor(registrationType, implementationType));
            return this;
        }

        public ServiceContainerOptionsBuilder RegisterFromAssembly()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => typeof(IServiceRegistration).IsAssignableFrom(type));
            foreach (var type in types)
            {
                var registration = (IServiceRegistration)Activator.CreateInstance(type);
                registration.Register(this);
            }

            return this;
        }
    }
}