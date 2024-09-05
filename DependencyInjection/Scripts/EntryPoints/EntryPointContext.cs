using UnityEngine;

namespace DependencyInjection.EntryPoints
{
    public class EntryPointContext
    {
        public Transform Parent { get; }

        public EntryPointContext(Transform parent)
        {
            Parent = parent;
        }
    }
}