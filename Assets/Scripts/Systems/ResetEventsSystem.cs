using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    partial struct ResetEventsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Reset selection events
            foreach (var selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
            {
                selected.ValueRW.OnDeselected = false;
                selected.ValueRW.OnSelected = false;
            }
            // Reset health events
            foreach (var selected in SystemAPI.Query<RefRW<Health>>())
            {
                selected.ValueRW.OnHealthChanged = false;
            }
        }
    }
}
