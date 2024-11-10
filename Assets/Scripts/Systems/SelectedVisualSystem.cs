using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(ResetEventsSystem))]
    partial struct SelectedVisualSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
            {
                if (selected.ValueRO.OnSelected)
                {
                    var visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);

                    visualLocalTransform.ValueRW.Scale = selected.ValueRO.ShowScale;
                }
                // else-if is important here - it is possible have both onSelect and
                // onDeselect in one frame in the event of re-selecting the same units
                else if (selected.ValueRO.OnDeselected)
                {
                    var visualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.VisualEntity);

                    visualLocalTransform.ValueRW.Scale = 0f;
                }
            }
        }
    }
}
