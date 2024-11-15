using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class ZombieSpawnerAuthoring : MonoBehaviour
    {
        public float timerMax;
        public float randomWalkingDistanceMin;
        public float randomWalkingDistanceMax;

        public class Baker : Baker<ZombieSpawnerAuthoring>
        {
            public override void Bake(ZombieSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(
                    entity,
                    new ZombieSpawner
                    {
                        TimerMax = authoring.timerMax,
                        RandomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                        RandomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                    }
                );
            }
        }
    }
}

public struct ZombieSpawner : IComponentData
{
    public float Timer;
    public float TimerMax;
    public float RandomWalkingDistanceMin;
    public float RandomWalkingDistanceMax;
}
