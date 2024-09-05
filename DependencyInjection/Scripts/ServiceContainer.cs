using System;
using System.Collections.Concurrent;
using Singletons;
using UnityEngine;

namespace DependencyInjection
{
    public class ServiceContainer : PersistentSingleton<ServiceContainer>, IServiceProvider
    {
        private readonly ConcurrentDictionary<Type, IServiceDescriptor> _descriptors = new();

        private bool _initialized;
        public object GetService(Type requestedType)
        {
            if (!_initialized)
                throw new InvalidOperationException($"{GetType().Name} is not initialized!");
            
            if (!_descriptors.TryGetValue(requestedType, out var descriptor))
            {
                return null;
            }

            return descriptor.CreateInstance(this);
        }

        public void Initialize(ServiceContainerOptions options)
        {
            _descriptors.Clear();
            foreach (var descriptor in options.Descriptors)
            {
                if (!_descriptors.TryAdd(descriptor.RegistrationType, descriptor)) 
                    Debug.LogWarning($"Try {descriptor.RegistrationType} more than one time");
            }

            _initialized = true;
        }

        //TODO: Global & scene & scoped & transient scope, IDisposable support, Factory registration 
    }
}
