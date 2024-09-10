using System;

namespace DependencyInjection
{
    public sealed class FactoryServiceDescriptor : IServiceDescriptor
    {
        public Type RegistrationType { get; }
        public Type ImplementationType { get; }
        public ServiceLifetime Lifetime { get; }

        private readonly Func<IServiceProvider, object> _factoryMethod;

        public FactoryServiceDescriptor(Type registrationType, Type implementationType, Func<IServiceProvider, object> factoryMethod, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            RegistrationType = registrationType;
            ImplementationType = implementationType;
            _factoryMethod = factoryMethod;
            Lifetime = lifetime;
        }
        public object CreateInstance(IServiceProvider provider)
        {
            return _factoryMethod(provider);
            //TODO: type check
        }
    }
}