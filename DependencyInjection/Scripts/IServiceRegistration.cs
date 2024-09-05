using DependencyInjection.Builders;

namespace DependencyInjection
{
    public interface IServiceRegistration
    {
        void Register(ServiceContainerOptionsBuilder builder);
    }
}