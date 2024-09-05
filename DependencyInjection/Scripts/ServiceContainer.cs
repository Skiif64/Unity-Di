using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Singletons;
using UnityEngine;

namespace DependencyInjection
{
    public class ServiceContainer : PersistentSingleton<ServiceContainer>, IServiceProvider, IDisposable
    {
        private readonly ConcurrentDictionary<Type, IServiceDescriptor> _descriptors = new();
        private readonly ConcurrentDictionary<Type, object> _cached = new();
        private readonly List<IDisposable> _disposables = new();

        private bool _initialized;
        private bool _disposed;
        public object GetService(Type requestedType)
        {
            if (!_initialized)
                throw new InvalidOperationException($"{GetType().Name} is not initialized!");
            if (_disposed)
                throw new ObjectDisposedException(nameof(ServiceContainer));

            if (_cached.TryGetValue(requestedType, out var cached))
            {
                return cached;
            }
            
            if (!_descriptors.TryGetValue(requestedType, out var descriptor))
            {
                return null;
            }

            var instance = descriptor.CreateInstance(this);
            _cached.AddOrUpdate(requestedType, _ => instance, (_, _) => instance);
            if (instance is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }
            return instance;
        }

        public void Initialize(ServiceContainerOptions options)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ServiceContainer));
            _descriptors.Clear();
            _descriptors.TryAdd(typeof(IServiceProvider), new FactoryServiceDescriptor(
                typeof(IServiceProvider),
                GetType(),
                _ => this));
            foreach (var descriptor in options.Descriptors)
            {
                if (!_descriptors.TryAdd(descriptor.RegistrationType, descriptor)) 
                    Debug.LogWarning($"Try {descriptor.RegistrationType} more than one time");
            }

            _initialized = true;
        }

        //TODO: Global & scene & scoped & transient scope, IDisposable support, Factory registration 
        public void Dispose()
        {
            if (_disposed) return;
            foreach (var disposable in _disposables)
            {
                if (ReferenceEquals(disposable, this))
                    continue;
                disposable?.Dispose();
            }
            _disposables.Clear();
            _disposed = true;
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}
