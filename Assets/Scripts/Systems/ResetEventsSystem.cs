using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    partial struct ResetEventsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ResetSelectedEventsJob().ScheduleParallel();
            new ResetHealthEventsJob().ScheduleParallel();
            new ResetShootAttackEventsJob().ScheduleParallel();
        }
    }

    [BurstCompile]
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    public partial struct ResetSelectedEventsJob : IJobEntity
    {
        void Execute(ref Selected selected)
        {
            selected.OnDeselected = false;
            selected.OnSelected = false;
        }
    }

    [BurstCompile]
    public partial struct ResetHealthEventsJob : IJobEntity
    {
        void Execute(ref Health health)
        {
            health.OnHealthChanged = false;
        }
    }

    [BurstCompile]
    public partial struct ResetShootAttackEventsJob : IJobEntity
    {
        void Execute(ref ShootAttack shootAttack)
        {
            shootAttack.OnShootAttack.IsTriggered = false;
            shootAttack.OnShootAttack.ShootFromPosition = float3.zero;
        }
    }
}
