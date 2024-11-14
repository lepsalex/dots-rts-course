using MonoBehaviours;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    partial struct FindTargetSystem : ISystem
    {
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

            foreach (var (localTransform, findTarget, target) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>, RefRW<Target>>())
            {
                // only check for targets if the timer has elapsed
                findTarget.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (findTarget.ValueRO.Timer > 0f)
                {
                    continue;
                }

                // reset target find timer
                findTarget.ValueRW.Timer = findTarget.ValueRO.TimerMax;

                // reset hit list
                distanceHitList.Clear();

                // check for targets in range and set target component if appropriate
                if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.Range, ref distanceHitList, collisionFilter))
                {
                    foreach (var distanceHit in distanceHitList)
                    {
                        // mitigating a DOTS bug where the entity query returns a result but the entity is destroyed
                        if (!SystemAPI.Exists(distanceHit.Entity) || !SystemAPI.HasComponent<Unit>(distanceHit.Entity))
                            continue;

                        var targetHit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                        if (targetHit.Faction == findTarget.ValueRO.TargetFaction)
                        {
                            // todo: find CLOSEST target
                            target.ValueRW.TargetEntity = distanceHit.Entity;
                            break;
                        }
                    }
                }
            }
        }
    }
}
