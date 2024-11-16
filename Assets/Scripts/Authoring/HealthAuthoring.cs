using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class HealthAuthoring : MonoBehaviour
    {
        public int maxHealth;

        public class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(
                    entity,
                    new Health
                    {
                        MaxHealth = authoring.maxHealth,
                        CurrentHealth = authoring.maxHealth,
                        OnHealthChanged = true, // force an initial update
                    }
                );
            }
        }
    }
}

public struct Health : IComponentData
{
    public int MaxHealth;
    public int CurrentHealth;
    public bool OnHealthChanged;
}
