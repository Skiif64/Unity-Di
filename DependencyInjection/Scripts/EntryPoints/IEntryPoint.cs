namespace DependencyInjection.EntryPoints
{
    public interface IEntryPoint
    {
        void Initialize(EntryPointContext context);
    }
}