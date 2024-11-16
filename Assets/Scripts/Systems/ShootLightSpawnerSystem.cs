using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ShootLightSpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (var shootAttack in SystemAPI.Query<RefRO<ShootAttack>>())
            {
                if (!shootAttack.ValueRO.OnShootAttack.IsTriggered)
                    continue;

                // spawn a shoot light
                var shootLightEntity = state.EntityManager.Instantiate(entitiesReferences.ShootLightPrefabEntity);
                SystemAPI.SetComponent(shootLightEntity, LocalTransform.FromPosition(shootAttack.ValueRO.OnShootAttack.ShootFromPosition));
            }
        }
    }
}
