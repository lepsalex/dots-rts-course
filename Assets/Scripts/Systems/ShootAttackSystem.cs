using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    partial struct ShootAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (shootAttack, target) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>>())
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

                // shoot attack the target
                Debug.Log("SHOOT");
            }
        }
    }
}
