using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        public float timerMax;
        public int damageAmount;
        public float attackDistance;
        public Transform bulletSpawnPositionTransform;

        public class Baker : Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(
                    entity,
                    new ShootAttack
                    {
                        TimerMax = authoring.timerMax,
                        DamageAmount = authoring.damageAmount,
                        AttackDistance = authoring.attackDistance,
                        BulletSpawnLocalPosition = authoring.bulletSpawnPositionTransform.localPosition,
                    }
                );
            }
        }
    }
}

public struct ShootAttack : IComponentData
{
    public float Timer;
    public float TimerMax;
    public int DamageAmount;
    public float AttackDistance;
    public float3 BulletSpawnLocalPosition;
    public OnShootEvent OnShootAttack;

    public struct OnShootEvent
    {
        public bool IsTriggered;
        public float3 ShootFromPosition;
    }
}
