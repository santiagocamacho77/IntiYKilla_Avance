using UnityEngine;

namespace IntiYKilla
{
    // Capas del fondo que se mueven más lento que la cámara.
    public class Parallax : MonoBehaviour
    {
        public Transform cam;
        public float factor = 0.5f;
        public Vector3 offset;

        void LateUpdate()
        {
            if (cam == null) return;
            transform.position = new Vector3(cam.position.x * factor + offset.x,
                                             cam.position.y * factor * 0.3f + offset.y,
                                             offset.z);
        }
    }
}
