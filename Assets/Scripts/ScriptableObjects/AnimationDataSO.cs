using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu()]
    public class AnimationDataSO : ScriptableObject
    {
        public enum AnimationType
        {
            None,
            SoldierIdle,
            SoldierWalk,
            SoldierAim,
            SoldierShoot,
            ZombieIdle,
            ZombieWalk,
            ZombieAttack,
            ScoutIdle,
            ScoutWalk,
            ScoutAim,
            ScoutAttack,
        }

        public AnimationType animationType;
        public Mesh[] meshArray;
        public float frameTimerMax;
    }
}
