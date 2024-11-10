using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    partial struct HealthDeathTestSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (health, entity) in SystemAPI.Query<RefRO<Health>>().WithEntityAccess())
            {
                if (health.ValueRO.CurrentHealth <= 0)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}
