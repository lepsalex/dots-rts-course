using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class MeleeAttackAuthoring : MonoBehaviour
    {
        public float timerMax;
        public int damageAmount;
        public float attackDistance;
        public float colliderSize;

        public class Baker : Baker<MeleeAttackAuthoring>
        {
            public override void Bake(MeleeAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(
                    entity,
                    new MeleeAttack
                    {
                        TimerMax = authoring.timerMax,
                        DamageAmount = authoring.damageAmount,
                        AttackDistance = authoring.attackDistance,
                        ColliderSize = authoring.colliderSize,
                    }
                );
            }
        }
    }
}

public struct MeleeAttack : IComponentData
{
    public float Timer;
    public float TimerMax;
    public int DamageAmount;
    public float AttackDistance;
    public float ColliderSize;
}
