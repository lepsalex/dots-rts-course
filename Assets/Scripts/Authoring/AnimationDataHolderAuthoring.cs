using ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Authoring
{
    public class AnimationDataHolderAuthoring : MonoBehaviour
    {
        public AnimationDataSO soldierIdle;
        public AnimationDataSO soldierWalk;

        public class Baker : Baker<AnimationDataHolderAuthoring>
        {
            public override void Bake(AnimationDataHolderAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                var animationDataHolder = new AnimationDataHolder();

                // TODO: refactor to process array

                {
                    var blobBuilder = new BlobBuilder(Allocator.Temp);

                    // pass reference otherwise you end up with copies
                    ref var animationData = ref blobBuilder.ConstructRoot<AnimationData>();
                    animationData.FrameTimerMax = authoring.soldierIdle.FrameTimerMax;
                    animationData.FrameMax = authoring.soldierIdle.MeshArray.Length;

                    var blobBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationData.BatchMeshIdBlobArray, authoring.soldierIdle.MeshArray.Length);

                    for (var i = 0; i < authoring.soldierIdle.MeshArray.Length; i++)
                    {
                        var mesh = authoring.soldierIdle.MeshArray[i];
                        blobBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
                    }

                    animationDataHolder.SoldierIdle = blobBuilder.CreateBlobAssetReference<AnimationData>(Allocator.Persistent);

                    // remember to dispose, else memory leak
                    blobBuilder.Dispose();

                    // register animation so it doesn't get deallocated
                    AddBlobAsset(ref animationDataHolder.SoldierIdle, out var objectHash);
                }

                {
                    var blobBuilder = new BlobBuilder(Allocator.Temp);

                    // pass reference otherwise you end up with copies
                    ref var animationData = ref blobBuilder.ConstructRoot<AnimationData>();
                    animationData.FrameTimerMax = authoring.soldierWalk.FrameTimerMax;
                    animationData.FrameMax = authoring.soldierWalk.MeshArray.Length;

                    var blobBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationData.BatchMeshIdBlobArray, authoring.soldierWalk.MeshArray.Length);

                    for (var i = 0; i < authoring.soldierWalk.MeshArray.Length; i++)
                    {
                        var mesh = authoring.soldierWalk.MeshArray[i];
                        blobBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
                    }

                    animationDataHolder.SoldierWalk = blobBuilder.CreateBlobAssetReference<AnimationData>(Allocator.Persistent);

                    // remember to dispose, else memory leak
                    blobBuilder.Dispose();

                    // register animation so it doesn't get deallocated
                    AddBlobAsset(ref animationDataHolder.SoldierWalk, out var objectHash);
                }

                AddComponent(entity, animationDataHolder);
            }
        }
    }
}

public struct AnimationDataHolder : IComponentData
{
    public BlobAssetReference<AnimationData> SoldierIdle;
    public BlobAssetReference<AnimationData> SoldierWalk;
}

public struct AnimationData
{
    public float FrameTimerMax;
    public int FrameMax;
    public BlobArray<BatchMeshID> BatchMeshIdBlobArray;
}
