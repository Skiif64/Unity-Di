using DependencyInjection.Attributes;
using DependencyInjection.Base;
using UnityEngine;

namespace DependencyInjection.Tests.Mocks
{
    internal class InjectableMockComponent : InjectableMonoBehavior
    {
        [Inject] public IMockService Service { get; set; }
    }

    internal class MockComponent : MonoBehaviour
    {
        [Inject] public IMockService Service { get; set; }
    }
}