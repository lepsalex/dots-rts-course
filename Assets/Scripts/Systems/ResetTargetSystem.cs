using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    partial struct ResetTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var target in SystemAPI.Query<RefRW<Target>>())
            {
                // skip null entities
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                // set target to null for target entities that no longer
                // exist, or that will no longer exist later this frame
                // (we can tell by the missing LocalTransform)
                if (!SystemAPI.Exists(target.ValueRO.TargetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.TargetEntity))
                {
                    target.ValueRW.TargetEntity = Entity.Null;
                }
            }

            // similar logic for target override
            foreach (var targetOverride in SystemAPI.Query<RefRW<TargetOverride>>())
            {
                if (targetOverride.ValueRO.TargetEntity == Entity.Null)
                    continue;

                if (!SystemAPI.Exists(targetOverride.ValueRO.TargetEntity) || !SystemAPI.HasComponent<LocalTransform>(targetOverride.ValueRO.TargetEntity))
                {
                    targetOverride.ValueRW.TargetEntity = Entity.Null;
                }
            }
        }
    }
}
