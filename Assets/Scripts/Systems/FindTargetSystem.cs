using Common;
using MonoBehaviours;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    partial struct FindTargetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorldSystem = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var collisionWorld = physicsWorldSystem.CollisionWorld;
            var distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
            var collisionFilter = new CollisionFilter
            {
                GroupIndex = 0,
                BelongsTo = ~0u, // flipping 0 with ~ results in every bit being set
                CollidesWith = 1u << GameAssets.UnitsLayer, // bit-shift by the layer number
            };

            foreach (
                var (localTransform, findTarget, target, targetOverride) in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRW<FindTarget>,
                    RefRW<Target>,
                    RefRO<TargetOverride>
                >()
            )
            {
                //
                // Target Timer
                //

                // only check for targets if the timer has elapsed
                findTarget.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.Timer > 0f)
                {
                    continue;
                }

                // reset target find timer
                findTarget.ValueRW.Timer = findTarget.ValueRO.TimerMax;

                //
                // Target Override
                //

                if (targetOverride.ValueRO.TargetEntity != Entity.Null)
                {
                    target.ValueRW.TargetEntity = targetOverride.ValueRO.TargetEntity;
                    continue;
                }

                //
                // Find Target
                //

                // reset hit list
                distanceHitList.Clear();

                // get info on current target if it exists to manage target swapping
                var closestTargetDistance = float.MaxValue;
                var currentTargetDistanceOffset = 0f;
                if (target.ValueRO.TargetEntity != Entity.Null)
                {
                    var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                    closestTargetDistance = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);
                    currentTargetDistanceOffset = 2f;
                }

                // check for targets in range and set target component if appropriate
                if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.Range, ref distanceHitList, collisionFilter))
                {
                    // target the closest unit first
                    distanceHitList.Sort(new DistanceHitDistanceComparer());

                    foreach (var distanceHit in distanceHitList)
                    {
                        // mitigating a DOTS bug where the entity query returns a result but the entity is destroyed
                        if (!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Unit>(distanceHit.Entity))
                            continue;

                        var targetHit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);

                        // skip targets not of the target faction
                        if (targetHit.Faction != findTarget.ValueRO.TargetFaction)
                            continue;

                        // if the current target is within the offset, do not retarget
                        if (distanceHit.Distance + currentTargetDistanceOffset > closestTargetDistance)
                            break;

                        // target the entity at the closest target
                        target.ValueRW.TargetEntity = distanceHit.Entity;
                        break;
                    }
                }
            }
        }
    }
}
