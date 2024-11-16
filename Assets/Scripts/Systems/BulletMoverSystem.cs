using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    partial struct BulletMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            const float destroyDistanceSq = 0.2f;
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (localTransform, bullet, target, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Bullet>, RefRO<Target>>().WithEntityAccess())
            {
                // target null check
                if (target.ValueRO.TargetEntity == Entity.Null)
                {
                    ecb.DestroyEntity(entity);
                    continue;
                }

                //
                // MOVE
                //

                var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                var targetShootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.TargetEntity);
                var targetPosition = targetLocalTransform.TransformPoint(targetShootVictim.HitLocalPosition);

                // get distance before move to mitigate overshooting problem
                var distanceBeforeMoveSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

                // move bullet towards target
                var moveDirection = targetPosition - localTransform.ValueRO.Position;
                moveDirection = math.normalize(moveDirection);

                localTransform.ValueRW.Position += moveDirection * bullet.ValueRO.Speed * SystemAPI.Time.DeltaTime;

                //
                // HIT CHECK
                //

                var distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

                // Check for target overshoot and move bullet to target position if so
                if (distanceAfterSq > distanceBeforeMoveSq)
                {
                    localTransform.ValueRW.Position = targetPosition;
                }

                // If not hit, exit here
                if (math.distancesq(localTransform.ValueRO.Position, targetPosition) > destroyDistanceSq)
                    continue;

                //
                // ON HIT
                //

                // apply damage
                var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                targetHealth.ValueRW.CurrentHealth -= bullet.ValueRO.DamageAmount;
                targetHealth.ValueRW.OnHealthChanged = true;

                // destroy bullet entity
                ecb.DestroyEntity(entity);
            }
        }
    }
}
