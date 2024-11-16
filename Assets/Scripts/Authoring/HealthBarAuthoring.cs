using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class HealthBarAuthoring : MonoBehaviour
    {
        public GameObject barVisualEntity;
        public GameObject healthEntity;

        public class Baker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(
                    entity,
                    new HealthBar
                    {
                        BarVisualEntity = GetEntity(authoring.barVisualEntity, TransformUsageFlags.NonUniformScale),
                        HealthEntity = GetEntity(authoring.healthEntity, TransformUsageFlags.Dynamic),
                    }
                );
            }
        }
    }
}

public struct HealthBar : IComponentData
{
    public Entity BarVisualEntity;
    public Entity HealthEntity;
}
