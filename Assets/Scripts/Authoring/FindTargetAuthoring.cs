using Common;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        public float range;
        public Faction targetFaction;
        public float timerMax;

        public class Baker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(
                    entity,
                    new FindTarget
                    {
                        Range = authoring.range,
                        TargetFaction = authoring.targetFaction,
                        TimerMax = authoring.timerMax,
                    }
                );
            }
        }
    }
}

public struct FindTarget : IComponentData
{
    public float Range;
    public Faction TargetFaction;
    public float Timer;
    public float TimerMax;
}
