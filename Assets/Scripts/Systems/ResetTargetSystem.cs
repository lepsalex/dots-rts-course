using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
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

                // set target to null for target entities that no longer exist
                if (!SystemAPI.Exists(target.ValueRO.TargetEntity))
                {
                    target.ValueRW.TargetEntity = Entity.Null;
                }
            }
        }
    }
}
