using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    partial struct MoveOverrideSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (
                var (localTransform, moveOverride, moveOverrideEnabled, unitMover) in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRO<MoveOverride>,
                    EnabledRefRW<MoveOverride>,
                    RefRW<UnitMover>
                >()
            )
            {
                if (math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.TargetPosition) > UnitMoverSystem.ReachedTargetPositionDistanceSq)
                {
                    // move to override position
                    unitMover.ValueRW.TargetPosition = moveOverride.ValueRO.TargetPosition;
                }
                else
                {
                    // reached override position
                    moveOverrideEnabled.ValueRW = false;
                }
            }
        }
    }
}
