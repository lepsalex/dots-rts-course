using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems
{
    partial struct MeleeAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorld.CollisionWorld;
            var raycastHitList = new NativeList<RaycastHit>(Allocator.Temp);

            foreach (
                var (localTransform, meleeAttack, target, unitMover) in SystemAPI
                    .Query<RefRW<LocalTransform>, RefRW<MeleeAttack>, RefRO<Target>, RefRW<UnitMover>>()
                    .WithDisabled<MoveOverride>()
            )
            {
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                // check target distance
                var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                var distanceToTarget = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

                var isCloseEnoughToAttack = distanceToTarget <= meleeAttack.ValueRO.AttackDistance;

                // check if target is touching (in case collider is bigger than attack distance)
                var isTouchingTarget = false;

                var dirToTarget = targetLocalTransform.Position - localTransform.ValueRO.Position;
                dirToTarget = math.normalize(dirToTarget);
                var raycastInput = new RaycastInput
                {
                    Start = localTransform.ValueRO.Position,
                    End = localTransform.ValueRO.Position + dirToTarget * meleeAttack.ValueRO.ColliderSize,
                    Filter = CollisionFilter.Default,
                };
                raycastHitList.Clear();
                if (collisionWorld.CastRay(raycastInput, ref raycastHitList))
                {
                    foreach (var raycastHit in raycastHitList)
                    {
                        if (raycastHit.Entity != target.ValueRO.TargetEntity)
                            continue;

                        isTouchingTarget = true;
                        break;
                    }
                }

                // move closer if unable to attack
                if (!isCloseEnoughToAttack && !isTouchingTarget)
                {
                    unitMover.ValueRW.TargetPosition = targetLocalTransform.Position;
                    continue;
                }

                // stop moving if within range and face the target
                unitMover.ValueRW.TargetPosition = localTransform.ValueRO.Position;

                var attackDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
                attackDirection = math.normalize(attackDirection);

                var targetRotation = quaternion.LookRotation(attackDirection, math.up());
                localTransform.ValueRW.Rotation = math.slerp(
                    localTransform.ValueRO.Rotation,
                    targetRotation,
                    SystemAPI.Time.DeltaTime * unitMover.ValueRO.RotationSpeed
                );

                // only melee attack  if the timer has elapsed
                meleeAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (meleeAttack.ValueRO.Timer > 0f)
                    continue;

                // reset melee attack timer
                meleeAttack.ValueRW.Timer = meleeAttack.ValueRO.TimerMax;

                // set attack event to true
                meleeAttack.ValueRW.OnAttack = true;

                // apply damage to the targets health
                var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                targetHealth.ValueRW.CurrentHealth -= meleeAttack.ValueRO.DamageAmount;
                targetHealth.ValueRW.OnHealthChanged = true;
            }
        }
    }
}
