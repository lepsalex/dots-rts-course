using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class TargetOverrideAuthoring : MonoBehaviour
    {
        public class Baker : Baker<TargetOverrideAuthoring>
        {
            public override void Bake(TargetOverrideAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TargetOverride());
            }
        }
    }
}

public struct TargetOverride : IComponentData
{
    public Entity TargetEntity;
}
