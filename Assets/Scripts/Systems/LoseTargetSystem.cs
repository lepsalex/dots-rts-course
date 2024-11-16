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
            foreach (var (localTransform, target, loseTarget) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<Target>, RefRO<LoseTarget>>())
            {
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                // Check if the currently set target distance is
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
