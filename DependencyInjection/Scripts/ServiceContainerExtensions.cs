using System;

namespace DependencyInjection
{
    public static class ServiceContainerExtensions
    {
        public static T GetService<T>(this ServiceContainer container) => (T)container.GetService(typeof(T));

        public static T GetRequiredService<T>(this ServiceContainer container)
        {
            var instance = container.GetService<T>();
            if (instance == null)
                throw new InvalidOperationException($"Service for {typeof(T).Name} was not found");
            
            return instance;
        }
    }
}