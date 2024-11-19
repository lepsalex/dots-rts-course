using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace Systems
{
    [UpdateBefore(typeof(ActiveAnimationSystem))]
    partial struct ChangeAnimationSystem : ISystem
    {
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
                // skip if next animation is set to the same as the current one
                if (activeAnimation.ValueRO.NextAnimationType == activeAnimation.ValueRO.ActiveAnimationType)
                    continue;

                // reset values
                activeAnimation.ValueRW.Frame = 0;
                activeAnimation.ValueRW.FrameTimer = 0f;
                activeAnimation.ValueRW.ActiveAnimationType = activeAnimation.ValueRO.NextAnimationType;

                // update the mesh
                ref var animationData = ref animationDataHolder.AnimationDataBlobArrayBlobAssetReference.Value[
                    (int)activeAnimation.ValueRO.ActiveAnimationType
                ];
                materialMeshInfo.ValueRW.MeshID = animationData.BatchMeshIdBlobArray[0];
            }
        }
    }
}
