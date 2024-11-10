using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class UnitMoverAuthoring : MonoBehaviour
    {
        public float moveSpeed;
        public float rotationSpeed;

        public class Baker : Baker<UnitMoverAuthoring>
        {
            public override void Bake(UnitMoverAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMover { MoveSpeed = authoring.moveSpeed, RotationSpeed = authoring.rotationSpeed });
            }
        }
    }
}

public struct UnitMover : IComponentData
{
    public float MoveSpeed;
    public float RotationSpeed;
    public float3 TargetPosition;
}
