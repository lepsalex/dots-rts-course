using System;
using UnityEngine;

namespace MonoBehaviours
{
    public class MouseWorldPosition : MonoBehaviour
    {
        public static MouseWorldPosition Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public Vector3 GetPosition()
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            var mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // todo: use layer mask to look for terrain hits only
            return Physics.Raycast(mouseCameraRay, out RaycastHit hit) ? hit.point : Vector3.zero;
        }
    }
}
