using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    partial struct SetupUnitMoverDefaultPositionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (
                var (localTransform, unitMover, defaultPosition, entity) in SystemAPI
                    .Query<RefRW<LocalTransform>, RefRW<UnitMover>, RefRO<SetupUnitMoverDefaultPosition>>()
                    .WithEntityAccess()
            )
            {
                unitMover.ValueRW.TargetPosition = localTransform.ValueRO.Position;
                ecb.RemoveComponent<SetupUnitMoverDefaultPosition>(entity);
            }
        }
    }
}
