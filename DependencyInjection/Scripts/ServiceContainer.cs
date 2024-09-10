using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DependencyInjection.Abstractions;
using DependencyInjection.Attributes;
using Singletons;
using UnityEngine;

namespace DependencyInjection
{
    public class ServiceContainer : PersistentSingleton<ServiceContainer>, IServiceProvider, IGameObjectProvider, IDisposable
    {
        private readonly ConcurrentDictionary<Type, IServiceDescriptor> _descriptors = new();
        private readonly ConcurrentDictionary<Type, List<MemberSetter>> _cachedFieldAccessors = new();

        private readonly IServiceContainerCache _globalCache = new ServiceContainerCache();
        private readonly IServiceContainerCache _sceneCache = new ServiceContainerCache();
        
        private readonly List<IDisposable> _disposables = new();

        private bool _initialized;
        private bool _disposed;
        public object GetService(Type requestedType)
        {
            if (!_initialized)
                throw new InvalidOperationException($"{GetType().Name} is not initialized!");
            if (_disposed)
                throw new ObjectDisposedException(nameof(ServiceContainer));
          
            if (!_descriptors.TryGetValue(requestedType, out var descriptor))
            {
                return null;
            }

            object instance = null;
            if (TryGetFromCache(descriptor, out instance))
            {
                return instance;
            }
            
            instance = descriptor.CreateInstance(this);
            if (instance is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }
            TryAddToCache(descriptor, instance);
            return instance;
        }
        
        public GameObject CreateGameObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            var go = Instantiate(prefab, position, rotation, parent);
            List<MonoBehaviour> components = new(); 
            go.GetComponents(components);
            foreach (var component in components)
            {
                Inject(component);
            }
            return go;
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
        //TODO: separate monobeh & service engine, add MonoBehavior component Registration
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

        private bool TryGetFromCache(IServiceDescriptor descriptor, out object service)
        {
            if (descriptor.Lifetime == ServiceLifetime.Global)
            {
                return _globalCache.TryGet(descriptor.RegistrationType, out service);
            }
            if (descriptor.Lifetime == ServiceLifetime.Scene)
            {
                return _sceneCache.TryGet(descriptor.RegistrationType, out service);
            }

            service = null;
            return false;
        }

        private void TryAddToCache(IServiceDescriptor descriptor, object service)
        {
            if (descriptor.Lifetime == ServiceLifetime.Global)
            {
                _globalCache.Add(descriptor.RegistrationType, service);
            }
            if (descriptor.Lifetime == ServiceLifetime.Scene)
            {
                _sceneCache.Add(descriptor.RegistrationType, service);
            }
        }
        
        private void Inject(MonoBehaviour instance)
        {
            var instanceType = instance.GetType();
            if (!_cachedFieldAccessors.ContainsKey(instanceType))
            {
                //Compile properties
                foreach (var property in instanceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.NonPublic))
                {
                    if (property.CustomAttributes.All(x => x.AttributeType != typeof(InjectAttribute)))
                        continue;
                    _cachedFieldAccessors.AddOrUpdate(instanceType,
                        type => new() {CompileAccessor(type, property.PropertyType, property.Name)},
                        (type, list) =>
                        {
                            list.Add(CompileAccessor(type, property.PropertyType, property.Name));
                            return list;
                        });
                }
                
                //Compile fields
                foreach (var field in instanceType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField | BindingFlags.NonPublic))
                {
                    if (field.CustomAttributes.All(x => x.AttributeType != typeof(InjectAttribute)))
                        continue;
                    _cachedFieldAccessors.AddOrUpdate(instanceType,
                        type => new() {CompileAccessor(type, field.FieldType, field.Name)},
                        (type, list) =>
                        {
                            list.Add(CompileAccessor(type, field.FieldType, field.Name));
                            return list;
                        });
                }
            }

            if (!_cachedFieldAccessors.ContainsKey(instanceType)) return;
            
            //Inject
            foreach (var accessor in _cachedFieldAccessors[instanceType])
            {
                accessor(instance, this);
            }
        }

        private MemberSetter CompileAccessor(Type gameObjectType, Type requestedType, string memberName)
        {
            var serviceProviderParameter = Expression.Parameter(typeof(IServiceProvider), "provider");
            var gameObjectParameter = Expression.Parameter(typeof(MonoBehaviour), "gameObject");

            var member = Expression.PropertyOrField(Expression.Convert(gameObjectParameter, gameObjectType), memberName);

            Expression<Func<IServiceProvider, object>> getService = provider => provider.GetService(requestedType);

            var set = Expression.Assign(member, Expression.Convert(Expression.Invoke(getService, serviceProviderParameter), requestedType));
            
            var lambda = Expression.Lambda<MemberSetter>(set, gameObjectParameter, serviceProviderParameter);
            return lambda.Compile();
        }

        private delegate void MemberSetter(MonoBehaviour monoBehaviour, IServiceProvider provider);
    }
}
