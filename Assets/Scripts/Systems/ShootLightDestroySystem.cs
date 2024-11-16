using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    partial struct ShootLightDestroySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (shootLight, entity) in SystemAPI.Query<RefRW<ShootLight>>().WithEntityAccess())
            {
                shootLight.ValueRW.TimeToLive -= SystemAPI.Time.DeltaTime;

                if (shootLight.ValueRO.TimeToLive <= 0f)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}
