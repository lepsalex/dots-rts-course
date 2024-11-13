using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class ShootVictimAuthoring : MonoBehaviour
    {
        public Transform hitPositionTransform;

        public class Baker : Baker<ShootVictimAuthoring>
        {
            public override void Bake(ShootVictimAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootVictim { HitLocalPosition = authoring.hitPositionTransform.localPosition });
            }
        }
    }
}

public struct ShootVictim : IComponentData
{
    public float3 HitLocalPosition;
}
