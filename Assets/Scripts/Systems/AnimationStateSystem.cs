using ScriptableObjects;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(ShootAttackSystem))]
    [UpdateAfter(typeof(MeleeAttackSystem))]
    partial struct AnimationStateSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (animatedMesh, unitAnimations, unitMover) in SystemAPI.Query<RefRO<AnimatedMesh>, RefRO<UnitAnimations>, RefRO<UnitMover>>())
            {
                var activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.MeshEntity);

                if (unitMover.ValueRO.IsMoving)
                {
                    activeAnimation.ValueRW.NextAnimationType = unitAnimations.ValueRO.WalkAnimationType;
                }
                else
                {
                    activeAnimation.ValueRW.NextAnimationType = unitAnimations.ValueRO.IdleAnimationType;
                }
            }

            foreach (
                var (animatedMesh, unitAnimations, shootAttack, unitMover, target) in SystemAPI.Query<
                    RefRO<AnimatedMesh>,
                    RefRO<UnitAnimations>,
                    RefRO<ShootAttack>,
                    RefRO<UnitMover>,
                    RefRO<Target>
                >()
            )
            {
                if (unitMover.ValueRO.IsMoving)
                    continue;

                var activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.MeshEntity);

                if (target.ValueRO.TargetEntity != Entity.Null)
                {
                    activeAnimation.ValueRW.NextAnimationType = unitAnimations.ValueRO.AimAnimationType;
                }

                if (shootAttack.ValueRO.OnShootAttack.IsTriggered)
                {
                    activeAnimation.ValueRW.NextAnimationType = unitAnimations.ValueRO.ShootAnimationType;
                }
            }

            foreach (var (animatedMesh, unitAnimations, meleeAttack) in SystemAPI.Query<RefRO<AnimatedMesh>, RefRO<UnitAnimations>, RefRO<MeleeAttack>>())
            {
                if (!meleeAttack.ValueRO.OnAttack)
                    continue;

                var activeAnimation = SystemAPI.GetComponentRW<ActiveAnimation>(animatedMesh.ValueRO.MeshEntity);
                activeAnimation.ValueRW.NextAnimationType = unitAnimations.ValueRO.AttackAnimationType;
            }
        }
    }
}
