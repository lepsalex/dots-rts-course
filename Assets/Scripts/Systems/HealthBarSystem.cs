using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthBarSystem : ISystem
    {
        private ComponentLookup<LocalTransform> _localTransformComponentLookup;
        private ComponentLookup<Health> _healthComponentLookup;
        private ComponentLookup<PostTransformMatrix> _postTransformMatrixComponentLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _localTransformComponentLookup = state.GetComponentLookup<LocalTransform>();
            _healthComponentLookup = state.GetComponentLookup<Health>(isReadOnly: true);
            _postTransformMatrixComponentLookup = state.GetComponentLookup<PostTransformMatrix>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var cameraForward = Vector3.zero;
            if (Camera.main != null)
            {
                cameraForward = Camera.main.transform.forward;
            }

            _localTransformComponentLookup.Update(ref state);
            _healthComponentLookup.Update(ref state);
            _postTransformMatrixComponentLookup.Update(ref state);

            var healthBarJob = new HealthBarJob
            {
                LocalTransformComponentLookup = _localTransformComponentLookup,
                HealthComponentLookup = _healthComponentLookup,
                PostTransformMatrixComponentLookup = _postTransformMatrixComponentLookup,
                CameraForward = cameraForward,
            };
            healthBarJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct HealthBarJob : IJobEntity
    {
        [ReadOnly]
        public float3 CameraForward;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalTransform> LocalTransformComponentLookup;

        [ReadOnly]
        public ComponentLookup<Health> HealthComponentLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<PostTransformMatrix> PostTransformMatrixComponentLookup;

        void Execute(in HealthBar healthBar, Entity entity)
        {
            var localTransform = LocalTransformComponentLookup.GetRefRW(entity);

            // ensure health bar faces the camera
            var parentLocalTransform = LocalTransformComponentLookup[healthBar.HealthEntity];
            if (localTransform.ValueRO.Scale == 1f)
            {
                localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(CameraForward, math.up()));
            }

            // skip calculating health bar visual if health has not changed
            var health = HealthComponentLookup[healthBar.HealthEntity];
            if (!health.OnHealthChanged)
                return;

            // get updated health %
            var healthNormalized = (float)health.CurrentHealth / health.MaxHealth;

            // show/hide bar if unit has full health
            localTransform.ValueRW.Scale = healthNormalized >= 1f ? 0f : 1f;

            // update health bar visual
            var barVisualPostTransform = PostTransformMatrixComponentLookup.GetRefRW(healthBar.BarVisualEntity);
            barVisualPostTransform.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
        }
    }
}
