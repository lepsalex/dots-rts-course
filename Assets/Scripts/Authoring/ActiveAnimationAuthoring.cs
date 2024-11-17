using ScriptableObjects;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Authoring
{
    public class ActiveAnimationAuthoring : MonoBehaviour
    {
        public class Baker : Baker<ActiveAnimationAuthoring>
        {
            public override void Bake(ActiveAnimationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ActiveAnimation());
            }
        }
    }
}

public struct ActiveAnimation : IComponentData
{
    public int Frame;
    public float FrameTimer;
    public BlobAssetReference<AnimationData> AnimationDataBlobAssetReference;
}
