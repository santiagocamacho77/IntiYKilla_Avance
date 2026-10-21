using UnityEngine;

namespace IntiYKilla
{
    // Puerta de piedra que se abre al recuperar el pedazo del Intihuatana de la zona.
    public class Gate : MonoBehaviour
    {
        public Collider2D col;
        public float riseHeight = 5f;
        bool opening;
        float risen;

        public void Open() => opening = true;

        void Update()
        {
            if (!opening || risen >= riseHeight) return;
            float step = Time.deltaTime * 2.5f;
            risen += step;
            transform.position += Vector3.up * step;
            if (risen >= riseHeight * 0.5f) col.enabled = false;
        }
    }
}
