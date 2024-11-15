using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    partial struct ZombieSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (localTransform, zombieSpawner) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ZombieSpawner>>())
            {
                // check if it's time to spawn
                zombieSpawner.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (zombieSpawner.ValueRO.Timer > 0f)
                    continue;

                // reset timer
                zombieSpawner.ValueRW.Timer = zombieSpawner.ValueRO.TimerMax;

                // spawn the zombie
                var zombieEntity = state.EntityManager.Instantiate(entitiesReferences.ZombiePrefabEntity);
                SystemAPI.SetComponent(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

                // add a random walking component
                ecb.AddComponent(
                    zombieEntity,
                    new RandomWalking
                    {
                        OriginPosition = localTransform.ValueRO.Position,
                        TargetPosition = localTransform.ValueRO.Position,
                        DistanceMin = zombieSpawner.ValueRO.RandomWalkingDistanceMin,
                        DistanceMax = zombieSpawner.ValueRO.RandomWalkingDistanceMax,
                        Random = new Random((uint)zombieEntity.Index),
                    }
                );
            }
        }
    }
}
