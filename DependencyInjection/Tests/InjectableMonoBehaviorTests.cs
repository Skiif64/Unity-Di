using System.Collections;
using DependencyInjection.Builders;
using DependencyInjection.Tests.Mocks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DependencyInjection.Tests
{
    public class InjectableMonoBehaviorTests
    {
        [UnityTest]
        public IEnumerator Awake_ShouldInjectRequiredDependencies()
        {
            var options = new ServiceContainerOptionsBuilder()
                .Register<IMockService, MockService>()
                .Build();
            
            ServiceContainer.Instance.Initialize(options);
            //yield return null;
            var actual = new GameObject("[TestObject]").AddComponent<MockComponent>();
            //yield return new WaitForSeconds(0.1f);
            Assert.That(actual.Service, Is.Not.Null);
            yield break;
        }
    }
}