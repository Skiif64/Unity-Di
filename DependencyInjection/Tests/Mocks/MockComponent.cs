using DependencyInjection.Attributes;
using DependencyInjection.Base;

namespace DependencyInjection.Tests.Mocks
{
    internal class MockComponent : InjectableMonoBehavior
    {
        [Inject] public IMockService Service { get; set; }
    }
}