using System.Collections.Generic;

namespace DependencyInjection
{
    public class ServiceContainerOptions
    {
        public IReadOnlyCollection<IServiceDescriptor> Descriptors { get; }

        public ServiceContainerOptions(IReadOnlyCollection<IServiceDescriptor> descriptors)
        {
            Descriptors = descriptors;
        }
    }
}