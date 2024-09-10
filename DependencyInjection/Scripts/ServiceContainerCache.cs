using System;
using System.Collections.Concurrent;
using DependencyInjection.Abstractions;

namespace DependencyInjection
{
    public sealed class ServiceContainerCache : IServiceContainerCache
    {
        private readonly ConcurrentDictionary<Type, object> _cached = new();

        public bool Contains(Type registeredAs) => _cached.ContainsKey(registeredAs);

        public bool TryGet(Type registeredAs, out object service) => _cached.TryGetValue(registeredAs, out service);

        public void Add(Type registeredAs, object service) => _cached.AddOrUpdate(registeredAs,
            _ => service,
            (_, _) => service);
    }
}