using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    partial struct ShootAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (var (localTransform, shootAttack, target) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ShootAttack>, RefRO<Target>>())
            {
                if (target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                // only shoot attack  if the timer has elapsed
                shootAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;
                if (shootAttack.ValueRO.Timer > 0f)
                {
                    continue;
                }

                // reset shoot attack timer
                shootAttack.ValueRW.Timer = shootAttack.ValueRO.TimerMax;

                // spawn a bullet
                var bulletEntity = state.EntityManager.Instantiate(entitiesReferences.BulletPrefabEntity);
                SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position + new float3(0f, 2f, 0f)));

                // set bullet damage to attack damage
                var bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bulletBullet.ValueRW.DamageAmount = shootAttack.ValueRO.DamageAmount;

                // set bullet target to attack target
                var bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
                bulletTarget.ValueRW.TargetEntity = target.ValueRO.TargetEntity;
            }
        }
    }
}
