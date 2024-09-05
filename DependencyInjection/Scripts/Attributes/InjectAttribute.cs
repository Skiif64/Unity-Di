using System;

namespace DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)] //TODO: method support
    public sealed class InjectAttribute : Attribute
    {
        
    }
}