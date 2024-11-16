using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class LoseTargetAuthoring : MonoBehaviour
    {
        public float loseTargetDistance;

        public class Baker : Baker<LoseTargetAuthoring>
        {
            public override void Bake(LoseTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new LoseTarget { LoseTargetDistance = authoring.loseTargetDistance });
            }
        }
    }
}

public struct LoseTarget : IComponentData
{
    public float LoseTargetDistance;
}
