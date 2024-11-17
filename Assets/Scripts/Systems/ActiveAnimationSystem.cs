using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Systems
{
    partial struct ActiveAnimationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AnimationDataHolder>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var animationDataHolder = SystemAPI.GetSingleton<AnimationDataHolder>();

            foreach (var (activeAnimation, materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
            {
                // ensure animation is setup (todo: will be removed for a better solution later)
                if (!activeAnimation.ValueRO.AnimationDataBlobAssetReference.IsCreated)
                {
                    activeAnimation.ValueRW.AnimationDataBlobAssetReference = animationDataHolder.SoldierIdle;
                }

                // testing animation system (todo: remove)
                if (Input.GetKeyDown(KeyCode.T))
                    activeAnimation.ValueRW.AnimationDataBlobAssetReference = animationDataHolder.SoldierIdle;

                if (Input.GetKeyDown(KeyCode.Y))
                    activeAnimation.ValueRW.AnimationDataBlobAssetReference = animationDataHolder.SoldierWalk;

                // update animation timer
                activeAnimation.ValueRW.FrameTimer += SystemAPI.Time.DeltaTime;

                if (activeAnimation.ValueRO.FrameTimer < activeAnimation.ValueRO.AnimationDataBlobAssetReference.Value.FrameTimerMax)
                    continue;

                // calculate the next frame
                activeAnimation.ValueRW.Frame = (activeAnimation.ValueRO.Frame + 1) % activeAnimation.ValueRO.AnimationDataBlobAssetReference.Value.FrameMax;

                // set the animation to the frame
                materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.AnimationDataBlobAssetReference.Value.BatchMeshIdBlobArray[
                    activeAnimation.ValueRO.Frame
                ];

                // reset timer
                activeAnimation.ValueRW.FrameTimer = 0f;
            }
        }
    }
}
