using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    partial struct ResetTargetSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _localTransformComponentLookup;
        private EntityStorageInfoLookup _entityStorageInfoLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _localTransformComponentLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
            _entityStorageInfoLookup = state.GetEntityStorageInfoLookup();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _localTransformComponentLookup.Update(ref state);
            _entityStorageInfoLookup.Update(ref state);

            var resetTargetJob = new ResetTargetJob
            {
                EntityStorageInfoLookup = _entityStorageInfoLookup,
                LocalTransformComponentLookup = _localTransformComponentLookup,
            };

            var resetTargetOverrideJob = new ResetTargetOverrideJob()
            {
                EntityStorageInfoLookup = _entityStorageInfoLookup,
                LocalTransformComponentLookup = _localTransformComponentLookup,
            };

            resetTargetJob.ScheduleParallel();
            resetTargetOverrideJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ResetTargetJob : IJobEntity
    {
        [ReadOnly]
        public EntityStorageInfoLookup EntityStorageInfoLookup;

        [ReadOnly]
        public ComponentLookup<LocalTransform> LocalTransformComponentLookup;

        void Execute(ref Target target)
        {
            // skip null entities
            if (target.TargetEntity == Entity.Null)
                return;

            // set target to null for target entities that no longer
            // exist, or that will no longer exist later this frame
            // (we can tell by the missing LocalTransform)
            if (!EntityStorageInfoLookup.Exists(target.TargetEntity) || !LocalTransformComponentLookup.HasComponent(target.TargetEntity))
            {
                target.TargetEntity = Entity.Null;
            }
        }
    }

    [BurstCompile]
    public partial struct ResetTargetOverrideJob : IJobEntity
    {
        [ReadOnly]
        public EntityStorageInfoLookup EntityStorageInfoLookup;

        [ReadOnly]
        public ComponentLookup<LocalTransform> LocalTransformComponentLookup;

        void Execute(ref TargetOverride targetOverride)
        {
            if (targetOverride.TargetEntity == Entity.Null)
                return;

            if (!EntityStorageInfoLookup.Exists(targetOverride.TargetEntity) || !LocalTransformComponentLookup.HasComponent(targetOverride.TargetEntity))
            {
                targetOverride.TargetEntity = Entity.Null;
            }
        }
    }
}
