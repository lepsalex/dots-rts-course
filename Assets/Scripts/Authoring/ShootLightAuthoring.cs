using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ShootLightAuthoring : MonoBehaviour
    {
        public float TimeToLive;

        public class Baker : Baker<ShootLightAuthoring>
        {
            public override void Bake(ShootLightAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootLight { TimeToLive = authoring.TimeToLive });
            }
        }
    }
}

public struct ShootLight : IComponentData
{
    public float TimeToLive;
}
