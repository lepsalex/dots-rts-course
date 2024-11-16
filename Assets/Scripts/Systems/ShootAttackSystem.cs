using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    partial struct ShootAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitiesReferences>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (
                var (localTransform, shootAttack, target, unitMover) in SystemAPI
                    .Query<RefRW<LocalTransform>, RefRW<ShootAttack>, RefRO<Target>, RefRW<UnitMover>>()
                    .WithDisabled<MoveOverride>()
            )
            {
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                // check target distance
                var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.TargetEntity);
                var distanceToTarget = math.distance(localTransform.ValueRO.Position, targetLocalTransform.Position);

                // move closer if the target is out of range
                if (distanceToTarget > shootAttack.ValueRO.AttackDistance)
                {
                    unitMover.ValueRW.TargetPosition = targetLocalTransform.Position;
                    continue;
                }

                // stop moving if within range and face the target
                unitMover.ValueRW.TargetPosition = localTransform.ValueRO.Position;

                var aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
                aimDirection = math.normalize(aimDirection);

                var targetRotation = quaternion.LookRotation(aimDirection, math.up());
                localTransform.ValueRW.Rotation = math.slerp(
                    localTransform.ValueRO.Rotation,
                    targetRotation,
                    SystemAPI.Time.DeltaTime * unitMover.ValueRO.RotationSpeed
                );

                // only shoot attack  if the timer has elapsed
                shootAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (shootAttack.ValueRO.Timer > 0f)
                    continue;

                // reset shoot attack timer
                shootAttack.ValueRW.Timer = shootAttack.ValueRO.TimerMax;

                // spawn a bullet
                var bulletEntity = state.EntityManager.Instantiate(entitiesReferences.BulletPrefabEntity);
                var bulletSpawnLocation = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.BulletSpawnLocalPosition);
                SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnLocation));

                // set bullet damage to attack damage
                var bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bulletBullet.ValueRW.DamageAmount = shootAttack.ValueRO.DamageAmount;

                // set bullet target to attack target
                var bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
                bulletTarget.ValueRW.TargetEntity = target.ValueRO.TargetEntity;

                // trigger the OnShootAttack event
                shootAttack.ValueRW.OnShootAttack.IsTriggered = true;
                shootAttack.ValueRW.OnShootAttack.ShootFromPosition = bulletSpawnLocation;
            }
        }
    }
}
