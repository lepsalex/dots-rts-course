using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace Systems
{
    partial struct ActiveAnimationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (activeAnimation, materialMeshInfo) in SystemAPI.Query<RefRW<ActiveAnimation>, RefRW<MaterialMeshInfo>>())
            {
                // update animation timer
                activeAnimation.ValueRW.FrameTimer += SystemAPI.Time.DeltaTime;

                if (activeAnimation.ValueRO.FrameTimer < activeAnimation.ValueRO.FrameTimerMax)
                    continue;

                activeAnimation.ValueRW.Frame = (activeAnimation.ValueRO.Frame + 1) % activeAnimation.ValueRO.FrameMax;

                // assign the correct mesh based on the frame (temp switch here, will replace with array)
                materialMeshInfo.ValueRW.MeshID = activeAnimation.ValueRO.Frame switch
                {
                    1 => activeAnimation.ValueRO.Frame1,
                    _ => activeAnimation.ValueRO.Frame0,
                };

                // reset timer
                activeAnimation.ValueRW.FrameTimer = 0f;
            }
        }
    }
}
