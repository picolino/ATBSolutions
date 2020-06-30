using Leopotam.Ecs;

namespace ABT.DxfToNcConverter.Tests
{
    public abstract class SystemTestBase<T> : TestBase where T : IEcsSystem
    {
        protected abstract T System { get; set; }
    }
}