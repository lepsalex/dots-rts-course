using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct HealthBarSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var cameraForward = Vector3.zero;
            if (Camera.main != null)
            {
                cameraForward = Camera.main.transform.forward;
            }

            foreach (var (localTransform, healthBar) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<HealthBar>>())
            {
                // ensure health bar faces the camera
                var parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.HealthEntity);
                if (localTransform.ValueRO.Scale == 1f)
                {
                    localTransform.ValueRW.Rotation = parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
                }

                // skip calculating health bar visual if health has not changed
                var health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.HealthEntity);
                if (!health.OnHealthChanged)
                    continue;

                // get updated health %
                var healthNormalized = (float)health.CurrentHealth / health.MaxHealth;

                // show/hide bar if unit has full health
                localTransform.ValueRW.Scale = healthNormalized >= 1f ? 0f : 1f;

                // update health bar visual
                var barVisualPostTransform = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.BarVisualEntity);
                barVisualPostTransform.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
            }
        }
    }
}
