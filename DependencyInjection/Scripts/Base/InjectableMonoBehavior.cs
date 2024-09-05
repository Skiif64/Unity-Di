using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DependencyInjection.Attributes;
using UnityEngine;

namespace DependencyInjection.Base
{
    public abstract class InjectableMonoBehavior : MonoBehaviour
    {
        private static readonly List<Action<IServiceProvider>> CachedAccessors = new();
        private static bool _cached;


        private void Awake()
        {
            Inject();
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }
        
        private void Inject()
        {
            if (!_cached)
            {
                foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.NonPublic))
                {
                    if (property.CustomAttributes.All(x => x.AttributeType != typeof(InjectAttribute)))
                        continue;
                    CachedAccessors.Add(CompileAccessor(property.PropertyType, property.Name));
                }
                
                foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField | BindingFlags.NonPublic))
                {
                    if (field.CustomAttributes.All(x => x.AttributeType != typeof(InjectAttribute)))
                        continue;
                    CachedAccessors.Add(CompileAccessor(field.FieldType, field.Name));
                }

                _cached = true;
            }

            foreach (var accessor in CachedAccessors)
            {
                accessor(ServiceContainer.Instance);
            }
        }

        private Action<IServiceProvider> CompileAccessor(Type type, string memberName)
        {
            var constant = Expression.Constant(this);
            var parameter = Expression.Parameter(typeof(IServiceProvider), "provider");

            var member = Expression.PropertyOrField(constant, memberName);

            Expression<Func<IServiceProvider, object>> getService = provider => provider.GetService(type);

            var set = Expression.Assign(member, Expression.Convert(Expression.Invoke(getService, parameter), type));

            var lambda = Expression.Lambda<Action<IServiceProvider>>(set, parameter);
            return lambda.Compile();
        }
    }
}