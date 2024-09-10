using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DependencyInjection.Attributes;

namespace DependencyInjection
{
    public sealed class ServiceDescriptor : IServiceDescriptor
    {
        public Type RegistrationType { get; }
        public Type ImplementationType { get; }
        public ServiceLifetime Lifetime { get; }

        private Func<IServiceProvider, object> _compiledFactoryMethod;

        public ServiceDescriptor(Type registrationType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (!registrationType.IsAssignableFrom(implementationType)) 
                throw new ArgumentException($"{registrationType.Name} is not assignable from {implementationType.Name}");
            
            RegistrationType = registrationType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }
        
        public object CreateInstance(IServiceProvider provider)
        {
            if (_compiledFactoryMethod != null)
            {
                return _compiledFactoryMethod(provider);
            }
            var constructors = ImplementationType.GetConstructors();
            var targetConstructor = constructors
                .Where(x => !x.IsStatic)
                .FirstOrDefault(x => x.CustomAttributes
                    .Any(y => y.AttributeType == typeof(DiConstructorAttribute)));
            if (targetConstructor != null)
            {
                _compiledFactoryMethod = CompileExpression(targetConstructor);
                return _compiledFactoryMethod(provider);
            }
            
            targetConstructor = constructors
                .Where(x => x.IsPublic && !x.IsStatic)
                .OrderBy(x => x.GetParameters().Length)
                .FirstOrDefault();
            _compiledFactoryMethod = CompileExpression(targetConstructor);
            return _compiledFactoryMethod(provider);
        }
        

        private Func<IServiceProvider, object> CompileExpression(ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();

            var providerParameter = Expression.Parameter(typeof(IServiceProvider), "provider");
            var parameterExpressions = new Expression[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                Expression<Func<IServiceProvider, object>> providerCallExpression = provider => provider.GetService(parameterType); 
                parameterExpressions[i] = Expression.Convert(
                    Expression.Invoke(providerCallExpression, providerParameter),
                    parameterType);
            }

            var constructorExpression = Expression.New(constructorInfo, parameterExpressions);
            var lambda = Expression.Lambda<Func<IServiceProvider, object>>(constructorExpression, providerParameter);

            return lambda.Compile();
        }
    }

    public enum ServiceLifetime
    {
        Transient,
        Scene,
        Global
    }
}