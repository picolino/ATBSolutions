using Leopotam.Ecs;
using NUnit.Framework;

namespace ABT.DxfToNcConverter.Tests
{
    public abstract class TestBase
    {
        [SetUp]
        protected virtual void Setup()
        {
        }

        private void SetupEcs()
        {
            var world = new EcsWorld();
            var systems = new EcsSystems(world);

            SetupWorld(world);
            SetupSystems(systems);
            BeforeInitializeSystems(systems);

            systems.Init();
        }
        
        protected virtual void SetupWorld(EcsWorld world)
        {
        }

        protected abstract void SetupSystems(EcsSystems systems);

        protected virtual void BeforeInitializeSystems(EcsSystems systems)
        {
        }
    }
}