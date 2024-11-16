using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class SetupUnitMoverDefaultPositionAuthoring : MonoBehaviour
    {
        public class Baker : Baker<SetupUnitMoverDefaultPositionAuthoring>
        {
            public override void Bake(SetupUnitMoverDefaultPositionAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SetupUnitMoverDefaultPosition());
            }
        }
    }
}

public struct SetupUnitMoverDefaultPosition : IComponentData { }
