using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu()]
    public class AnimationDataListSO : ScriptableObject
    {
        public List<AnimationDataSO> animationDataSOList;

        public AnimationDataSO GetAnimationDataSO(AnimationDataSO.AnimationType animationType)
        {
            return animationDataSOList.Find(animationDataSO => animationDataSO.animationType == animationType);
        }
    }
}
