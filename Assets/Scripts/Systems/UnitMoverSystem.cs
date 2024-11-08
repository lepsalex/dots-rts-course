using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems
{
    internal partial struct UnitMoverSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitMoverJob = new UnitMoverJob { DeltaTime = SystemAPI.Time.DeltaTime };
            unitMoverJob.ScheduleParallel();
        }
    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    public float DeltaTime;

    void Execute(
        ref LocalTransform localTransform,
        ref PhysicsVelocity physicsVelocity,
        in UnitMover unitMover
    )
    {
        var targetDistance = unitMover.TargetPosition - localTransform.Position;

        // todo: temp solution to stop units jitter on location
        // note that lengthsq is much more efficient than length
        // as length first calls lengthsq then does a square root
        // which is a costly operation!
        if (math.lengthsq(targetDistance) < 1)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }

        var moveDirection = math.normalize(targetDistance);

        // rotate to face movement direction
        localTransform.Rotation = math.slerp(
            localTransform.Rotation,
            quaternion.LookRotation(moveDirection, math.up()),
            unitMover.RotationSpeed * DeltaTime
        );

        // apply physics to move
        physicsVelocity.Linear = moveDirection * unitMover.MoveSpeed;

        // todo: temp lock rotation until we deal with collisions properly
        physicsVelocity.Angular = float3.zero;
    }
}
