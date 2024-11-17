using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    partial struct LoseTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (
                var (localTransform, target, loseTarget, targetOverride) in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<Target>,
                    RefRO<LoseTarget>,
                    RefRO<TargetOverride>
                >()
            )
            {
                // don't lost null targets
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                // don't lose overide targets
                if (targetOverride.ValueRO.TargetEntity != Entity.Null)
                    continue;

                // check if the currently set target distance is
                // too far away, if so reset (lose) the target
                var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                if (math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position) > loseTarget.ValueRO.LoseTargetDistance)
                {
                    target.ValueRW.TargetEntity = Entity.Null;
                }
            }
        }
    }
}
