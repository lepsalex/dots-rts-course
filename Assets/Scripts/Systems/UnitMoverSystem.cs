using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems
{
    internal partial struct UnitMoverSystem : ISystem
    {
        public const float ReachedTargetPositionDistanceSq = 2f;

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitMoverJob = new UnitMoverJob { DeltaTime = SystemAPI.Time.DeltaTime };
            unitMoverJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct UnitMoverJob : IJobEntity
    {
        public float DeltaTime;

        void Execute(ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity, ref UnitMover unitMover)
        {
            var targetDistance = unitMover.TargetPosition - localTransform.Position;

            // note that lengthsq is much more efficient than length
            // as length first calls lengthsq then does a square root
            // which is a costly operation!
            if (math.lengthsq(targetDistance) <= UnitMoverSystem.ReachedTargetPositionDistanceSq)
            {
                physicsVelocity.Linear = float3.zero;
                physicsVelocity.Angular = float3.zero;
                unitMover.IsMoving = false;
                return;
            }

            // set moving true and get move direction
            unitMover.IsMoving = true;
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
}
