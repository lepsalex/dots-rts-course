using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    partial struct ZombieSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

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
            }
        }
    }
}
