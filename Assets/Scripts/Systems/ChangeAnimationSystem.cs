using ScriptableObjects;
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

                // do not exit shoot animation until its reset (todo: make this more generic)
                if (activeAnimation.ValueRO.ActiveAnimationType == AnimationDataSO.AnimationType.SoldierShoot)
                    continue;

                // do not exit zombie attack animation until its reset (todo: make this more generic)
                if (activeAnimation.ValueRO.ActiveAnimationType == AnimationDataSO.AnimationType.ZombieAttack)
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
