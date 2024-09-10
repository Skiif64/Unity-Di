using System;
using System.Collections.Generic;
using System.Linq;

namespace DependencyInjection.Builders
{
    public class ServiceContainerOptionsBuilder
    {
        private readonly List<ServiceDescriptor> _descriptors = new();


        public ServiceContainerOptions Build() => new (_descriptors);

        public ServiceContainerOptionsBuilder Register<TRegister, TImplementation>(ServiceLifetime lifetime)
            where TImplementation : TRegister
            => Register(typeof(TRegister), typeof(TImplementation), lifetime);
        
        public ServiceContainerOptionsBuilder Register<TImplementation>(ServiceLifetime lifetime)
            => Register(typeof(TImplementation), typeof(TImplementation), lifetime);
        
        public ServiceContainerOptionsBuilder Register(Type registrationType, Type implementationType, ServiceLifetime lifetime)
        {
            if (!registrationType.IsAssignableFrom(implementationType))
                throw new ArgumentException($"Service {implementationType.Name} is not assignable to {registrationType.Name}");
            _descriptors.Add(new ServiceDescriptor(registrationType, implementationType, lifetime));
            return this;
        }
        
        //TODO: extract to extension methods
        public ServiceContainerOptionsBuilder RegisterAsTransient<TRegister, TImplementation>()
            where TImplementation : TRegister
            => Register(typeof(TRegister), typeof(TImplementation), ServiceLifetime.Transient);
        
        public ServiceContainerOptionsBuilder RegisterAsGlobal<TRegister, TImplementation>()
            where TImplementation : TRegister
            => Register(typeof(TRegister), typeof(TImplementation), ServiceLifetime.Global);
        
        public ServiceContainerOptionsBuilder RegisterAsScene<TRegister, TImplementation>()
            where TImplementation : TRegister
            => Register(typeof(TRegister), typeof(TImplementation), ServiceLifetime.Scene);
        
        
        public ServiceContainerOptionsBuilder RegisterAsTransient<TImplementation>()
            => Register(typeof(TImplementation), typeof(TImplementation), ServiceLifetime.Transient);
        
        public ServiceContainerOptionsBuilder RegisterAsGlobal<TImplementation>()
            => Register(typeof(TImplementation), typeof(TImplementation), ServiceLifetime.Global);
        
        public ServiceContainerOptionsBuilder RegisterAsScene<TImplementation>()
            => Register(typeof(TImplementation), typeof(TImplementation), ServiceLifetime.Scene);

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