using System.Collections;
using DependencyInjection.Builders;
using DependencyInjection.Tests.Mocks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace DependencyInjection.Tests
{
    public class ServiceContainerTests
    {
        [UnityTest]
        public IEnumerator GetService_ShouldReturnRequiredInstance_ByRegisteredInterface()
        {
            var options = new ServiceContainerOptionsBuilder()
                .Register<IMockService, MockService>()
                .Build();
            
            ServiceContainer.Instance.Initialize(options);

            var actual = ServiceContainer.Instance.GetService(typeof(IMockService));

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf(typeof(MockService)));
            yield break;
        }
        
        [UnityTest]
        public IEnumerator GetService_ShouldReturnRequiredInstanceWithDependencies_ByRegisteredInterface()
        {
            var options = new ServiceContainerOptionsBuilder()
                .Register<IMockServiceWithDependency, MockServiceWithDependency>()
                .Register<IMockDependency, MockDependency>()
                .Build();
            
            ServiceContainer.Instance.Initialize(options);

            var actual = ServiceContainer.Instance.GetService(typeof(IMockServiceWithDependency));

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.TypeOf(typeof(MockServiceWithDependency)));
            Assert.That(((MockServiceWithDependency)actual).Dependency, Is.Not.Null);
            yield break;
        }
    }
}
