using UnityEngine;

namespace IntiYKilla
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float smooth = 6f;
        public float minX = -2f, maxX = 100f, minY = 2f, maxY = 8f;
        public Vector2 lookAhead = new Vector2(2f, 1.5f);

        void Start() => SnapToTarget();

        public void SnapToTarget()
        {
            if (target == null) return;
            transform.position = Desired();
        }

        Vector3 Desired()
        {
            float x = Mathf.Clamp(target.position.x + lookAhead.x, minX, maxX);
            float y = Mathf.Clamp(target.position.y + lookAhead.y, minY, maxY);
            return new Vector3(x, y, -10f);
        }

        void LateUpdate()
        {
            if (target == null) return;
            transform.position = Vector3.Lerp(transform.position, Desired(), 1f - Mathf.Exp(-smooth * Time.deltaTime));
        }
    }
}
