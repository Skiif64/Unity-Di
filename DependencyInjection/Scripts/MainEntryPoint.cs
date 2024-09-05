using System;
using System.Linq;
using DependencyInjection.EntryPoints;
using UnityEngine;

namespace DependencyInjection
{
    public static class MainEntryPoint
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var root = EntryPointRoot.Instance;
            var context = new EntryPointContext(root.transform);
        
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies
                .SelectMany(assembly => assembly.DefinedTypes)
                .Where(type => type.IsClass && !type.IsAbstract)
                .Where(type => typeof(IEntryPoint).IsAssignableFrom(type));
        
            foreach (var type in types)
            {
                var entryPoint = (IEntryPoint)Activator.CreateInstance(type);
                entryPoint.Initialize(context);
            }
        }
    }
}