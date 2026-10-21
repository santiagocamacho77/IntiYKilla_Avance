using UnityEngine;

namespace IntiYKilla
{
    // Nubes que cruzan el cielo lentamente; de noche se oscurecen.
    public class CloudLayer : MonoBehaviour
    {
        public Transform cam;
        public int count = 5;
        public float width = 34f;

        Transform[] clouds;
        float[] speeds;

        void Start()
        {
            clouds = new Transform[count];
            speeds = new float[count];
            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("Nube");
                go.transform.SetParent(transform, false);
                var sr = SpriteFactory.AddRenderer(go, SpriteFactory.Cloud(i % 2), -95);
                var tint = go.AddComponent<DayNightTint>();
                tint.day = new Color(1f, 1f, 1f, 0.9f);
                tint.night = new Color(0.35f, 0.38f, 0.6f, 0.6f);
                clouds[i] = go.transform;
                speeds[i] = 0.3f + 0.15f * i;
                go.transform.localPosition = new Vector3(i * width / count, 2.5f + (i % 3) * 1.2f, 0f);
            }
        }

        void LateUpdate()
        {
            if (cam == null) return;
            for (int i = 0; i < count; i++)
            {
                var lp = clouds[i].localPosition;
                lp.x += speeds[i] * Time.deltaTime;
                clouds[i].localPosition = lp;
                float rel = Mathf.Repeat(lp.x - cam.position.x * 0.1f, width) - width / 2f;
                clouds[i].position = new Vector3(cam.position.x + rel, cam.position.y + lp.y, 0f);
            }
        }
    }
}
