using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct ResetEventsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Select component query (including if the component is not enabled via WithPresent)
            foreach (var selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
            {
                selected.ValueRW.OnDeselected = false;
                selected.ValueRW.OnSelected = false;
            }
        }
    }
}
