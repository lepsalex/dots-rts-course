using ScriptableObjects;
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
                // testing animation system (todo: remove)
                if (Input.GetKeyDown(KeyCode.T))
                    activeAnimation.ValueRW.NextAnimationType = AnimationDataSO.AnimationType.SoldierIdle;

                if (Input.GetKeyDown(KeyCode.Y))
                    activeAnimation.ValueRW.NextAnimationType = AnimationDataSO.AnimationType.SoldierWalk;

                ref var animationData = ref animationDataHolder.AnimationDataBlobArrayBlobAssetReference.Value[
                    (int)activeAnimation.ValueRO.ActiveAnimationType
                ];

                // update animation timer
                activeAnimation.ValueRW.FrameTimer += SystemAPI.Time.DeltaTime;

                if (animationData.FrameMax == 0 || activeAnimation.ValueRO.FrameTimer < animationData.FrameTimerMax)
                    continue;

                // calculate the next frame
                activeAnimation.ValueRW.Frame = (activeAnimation.ValueRO.Frame + 1) % animationData.FrameMax;

                // set the animation to the frame
                materialMeshInfo.ValueRW.MeshID = animationData.BatchMeshIdBlobArray[activeAnimation.ValueRO.Frame];

                // reset timer
                activeAnimation.ValueRW.FrameTimer = 0f;
            }
        }
    }
}
