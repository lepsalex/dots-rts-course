using ScriptableObjects;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Authoring
{
    public class ActiveAnimationAuthoring : MonoBehaviour
    {
        public AnimationDataSO soldierIdle;

        public class Baker : Baker<ActiveAnimationAuthoring>
        {
            public override void Bake(ActiveAnimationAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();

                AddComponent(
                    entity,
                    new ActiveAnimation
                    {
                        FrameMax = authoring.soldierIdle.MeshArray.Length,
                        FrameTimerMax = authoring.soldierIdle.FrameTimerMax,
                        Frame0 = entitiesGraphicsSystem.RegisterMesh(authoring.soldierIdle.MeshArray[0]),
                        Frame1 = entitiesGraphicsSystem.RegisterMesh(authoring.soldierIdle.MeshArray[1]),
                    }
                );
            }
        }
    }
}

public struct ActiveAnimation : IComponentData
{
    public int Frame;
    public int FrameMax;
    public float FrameTimer;
    public float FrameTimerMax;
    public BatchMeshID Frame0;
    public BatchMeshID Frame1;
}
