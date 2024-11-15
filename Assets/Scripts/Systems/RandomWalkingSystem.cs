using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    partial struct RandomWalkingSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (randomWalking, unitMover, localTransform) in SystemAPI.Query<RefRW<RandomWalking>, RefRW<UnitMover>, RefRO<LocalTransform>>())
            {
                if (math.distancesq(localTransform.ValueRO.Position, randomWalking.ValueRO.TargetPosition) <= UnitMoverSystem.ReachedTargetPositionDistanceSq)
                {
                    // Reached target position
                    var random = randomWalking.ValueRO.Random;

                    var randomDirection = new float3(random.NextFloat(-1f, 1f), 0f, random.NextFloat(-1f, 1f));
                    randomDirection = math.normalize(randomDirection);

                    var randomDistance = random.NextFloat(randomWalking.ValueRO.DistanceMin, randomWalking.ValueRO.DistanceMax);

                    randomWalking.ValueRW.TargetPosition = randomWalking.ValueRO.OriginPosition + randomDirection * randomDistance;

                    // persist the change in random generation
                    randomWalking.ValueRW.Random = random;
                }
                else
                {
                    // Too far, move closer
                    unitMover.ValueRW.TargetPosition = randomWalking.ValueRO.TargetPosition;
                }
            }
        }
    }
}
