namespace Leopotam.Ecs
{
    public static class EcsExtensions
    {
        public static T GetFilter<T>(this EcsWorld world) where T : EcsFilter
        {
            return (world.GetFilter(typeof(T)) as T)!;
        }
    }
}