using System;

namespace DependencyInjection.Abstractions
{
    public interface IServiceContainerCache
    {
        bool Contains(Type registeredAs);

        bool TryGet(Type registeredAs, out object service);

        void Add(Type registeredAs, object service);
    }
}