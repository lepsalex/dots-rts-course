using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu()]
    public class AnimationDataSO : ScriptableObject
    {
        public Mesh[] MeshArray;
        public float FrameTimerMax;
    }
}
