using System;

namespace DependencyInjection
{
    public interface IServiceDescriptor
    {
        Type RegistrationType { get; }
        Type ImplementationType { get; }
        ServiceLifetime Lifetime { get; }

        object CreateInstance(IServiceProvider provider);
    }
}