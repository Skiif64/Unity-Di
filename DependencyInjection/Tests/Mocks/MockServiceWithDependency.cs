namespace DependencyInjection.Tests.Mocks
{
    internal class MockServiceWithDependency : IMockServiceWithDependency
    {
        public IMockDependency Dependency { get; }

        public MockServiceWithDependency(IMockDependency dependency)
        {
            Dependency = dependency;
        }
    }

    internal interface IMockServiceWithDependency
    {
        
    }

    internal class MockDependency : IMockDependency
    {
        
    }
    
    internal interface IMockDependency
    {
        
    }
}