using System;

namespace DependencyInjection
{
    public sealed class FactoryServiceDescriptor : IServiceDescriptor
    {
        public Type RegistrationType { get; }
        public Type ImplementationType { get; }

        private readonly Func<IServiceProvider, object> _factoryMethod;

        public FactoryServiceDescriptor(Type registrationType, Type implementationType, Func<IServiceProvider, object> factoryMethod)
        {
            RegistrationType = registrationType;
            ImplementationType = implementationType;
            _factoryMethod = factoryMethod;
        }
        public object CreateInstance(IServiceProvider provider)
        {
            return _factoryMethod(provider);
            //TODO: type check
        }
    }
}