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

                // handle shoot animation reset (todo: remove this and replace with something more robust)
                if (activeAnimation.ValueRO is { Frame: 0, ActiveAnimationType: AnimationDataSO.AnimationType.SoldierShoot })
                {
                    activeAnimation.ValueRW.ActiveAnimationType = AnimationDataSO.AnimationType.None;
                }

                // handle attack animation reset (todo: remove this and replace with something more robust)
                if (activeAnimation.ValueRO is { Frame: 0, ActiveAnimationType: AnimationDataSO.AnimationType.ZombieAttack })
                {
                    activeAnimation.ValueRW.ActiveAnimationType = AnimationDataSO.AnimationType.None;
                }
            }
        }
    }
}
