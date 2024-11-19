using System;
using ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace Authoring
{
    public class AnimationDataHolderAuthoring : MonoBehaviour
    {
        public AnimationDataListSO animationDataListSO;

        public class Baker : Baker<AnimationDataHolderAuthoring>
        {
            public override void Bake(AnimationDataHolderAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var entitiesGraphicsSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<EntitiesGraphicsSystem>();
                var animationDataHolder = new AnimationDataHolder();

                var blobBuilder = new BlobBuilder(Allocator.Temp);

                // pass reference otherwise you end up with copies
                ref var animationDataBlobArray = ref blobBuilder.ConstructRoot<BlobArray<AnimationData>>();

                var animationTypes = System.Enum.GetValues(typeof(AnimationDataSO.AnimationType));
                var animationDataBlobBuilderArray = blobBuilder.Allocate<AnimationData>(ref animationDataBlobArray, animationTypes.Length);

                int index = 0;
                foreach (AnimationDataSO.AnimationType animationType in animationTypes)
                {
                    var animationDataSO = authoring.animationDataListSO.GetAnimationDataSO(animationType);
                    var numFrames = animationDataSO.meshArray?.Length ?? 0;

                    animationDataBlobBuilderArray[index].FrameTimerMax = animationDataSO.frameTimerMax;
                    animationDataBlobBuilderArray[index].FrameMax = numFrames;

                    var blobBuilderArray = blobBuilder.Allocate<BatchMeshID>(ref animationDataBlobBuilderArray[index].BatchMeshIdBlobArray, numFrames);

                    for (var i = 0; i < numFrames; i++)
                    {
                        var mesh = animationDataSO.meshArray?[i];
                        blobBuilderArray[i] = entitiesGraphicsSystem.RegisterMesh(mesh);
                    }

                    index++;
                }

                animationDataHolder.AnimationDataBlobArrayBlobAssetReference = blobBuilder.CreateBlobAssetReference<BlobArray<AnimationData>>(
                    Allocator.Persistent
                );

                // remember to dispose, else memory leak
                blobBuilder.Dispose();

                // register animations so they don't get deallocated
                AddBlobAsset(ref animationDataHolder.AnimationDataBlobArrayBlobAssetReference, out var objectHash);

                AddComponent(entity, animationDataHolder);
            }
        }
    }
}

public struct AnimationDataHolder : IComponentData
{
    public BlobAssetReference<BlobArray<AnimationData>> AnimationDataBlobArrayBlobAssetReference;
}

public struct AnimationData
{
    public float FrameTimerMax;
    public int FrameMax;
    public BlobArray<BatchMeshID> BatchMeshIdBlobArray;
}
