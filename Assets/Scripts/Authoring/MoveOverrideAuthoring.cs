using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class MoveOverrideAuthoring : MonoBehaviour
    {
        public class Baker : Baker<MoveOverrideAuthoring>
        {
            public override void Bake(MoveOverrideAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveOverride());
                SetComponentEnabled<MoveOverride>(entity, false);
            }
        }
    }
}

public struct MoveOverride : IComponentData, IEnableableComponent
{
    public float3 TargetPosition;
}
