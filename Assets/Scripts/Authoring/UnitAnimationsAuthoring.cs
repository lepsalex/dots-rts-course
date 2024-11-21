using ScriptableObjects;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class UnitAnimationsAuthoring : MonoBehaviour
    {
        public AnimationDataSO.AnimationType idleAnimationType;
        public AnimationDataSO.AnimationType walkAnimationType;
        public AnimationDataSO.AnimationType attackAnimationType;
        public AnimationDataSO.AnimationType aimAnimationType;
        public AnimationDataSO.AnimationType shootAnimationType;

        public class Baker : Baker<UnitAnimationsAuthoring>
        {
            public override void Bake(UnitAnimationsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(
                    entity,
                    new UnitAnimations
                    {
                        IdleAnimationType = authoring.idleAnimationType,
                        WalkAnimationType = authoring.walkAnimationType,
                        AttackAnimationType = authoring.attackAnimationType,
                        AimAnimationType = authoring.aimAnimationType,
                        ShootAnimationType = authoring.shootAnimationType,
                    }
                );
            }
        }
    }
}

public struct UnitAnimations : IComponentData
{
    public AnimationDataSO.AnimationType IdleAnimationType;
    public AnimationDataSO.AnimationType WalkAnimationType;
    public AnimationDataSO.AnimationType AttackAnimationType;
    public AnimationDataSO.AnimationType AimAnimationType;
    public AnimationDataSO.AnimationType ShootAnimationType;
}
