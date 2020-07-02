using Leopotam.Ecs;

namespace ATB.DxfToNcConverter.Tests
{
    public abstract class SystemTestBase<T> : TestBase where T : IEcsSystem, new()
    {
        protected EcsWorld World { get; private set; }
        protected T System { get; private set; }

        protected sealed override void SetupWorld(EcsWorld world)
        {
            World = world;
            
            SetupFilters(World);
        }

        protected virtual void SetupFilters(EcsWorld world)
        {
        }

        protected override void SetupSystems(EcsSystems systems)
        {
            System = new T();
            
            systems.Add(System);
        }
    }
}