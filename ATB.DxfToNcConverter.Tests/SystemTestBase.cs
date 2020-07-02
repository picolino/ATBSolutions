using Leopotam.Ecs;

namespace ATB.DxfToNcConverter.Tests
{
    public abstract class SystemTestBase<T> : TestBase where T : IEcsSystem, new()
    {
        protected T System { get; private set; }

        protected override void SetupSystems(EcsSystems systems)
        {
            System = new T();
            
            systems.Add(System);
        }
    }
}