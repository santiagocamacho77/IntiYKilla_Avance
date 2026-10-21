using UnityEngine;

namespace IntiYKilla
{
    // Ambiente: de día flotan pétalos de kantuta y polen, de noche aparecen luciérnagas.
    public class AmbientParticles : MonoBehaviour
    {
        public Transform cam;
        public int count = 36;
        public Vector2 area = new Vector2(26f, 14f);

        SpriteRenderer[] dots;
        Vector2[] seeds;
        bool[] isFirefly;

        void Start()
        {
            dots = new SpriteRenderer[count];
            seeds = new Vector2[count];
            isFirefly = new bool[count];
            var sprite = SpriteFactory.Pixel();
            for (int i = 0; i < count; i++)
            {
                var go = new GameObject("Partícula");
                go.transform.SetParent(transform, false);
                isFirefly[i] = i % 2 == 0;
                go.transform.localScale = Vector3.one * (isFirefly[i] ? 0.14f : 0.12f);
                dots[i] = SpriteFactory.AddRenderer(go, sprite, 6);
                seeds[i] = new Vector2(Random.value * 100f, Random.value * 100f);
            }
        }

        void LateUpdate()
        {
            if (cam == null || DayNightManager.Instance == null) return;
            float b = DayNightManager.Instance.Blend;
            float t = Time.time;
            for (int i = 0; i < count; i++)
            {
                var s = seeds[i];
                float x, y;
                Color c;
                if (isFirefly[i])
                {
                    // Luciérnagas: vuelan despacio y parpadean.
                    x = Mathf.Repeat(s.x * 7f + Mathf.Sin(t * 0.3f + s.y) * 2f, area.x) - area.x / 2f;
                    y = Mathf.Repeat(s.y * 3f + Mathf.Sin(t * 0.5f + s.x) * 1.5f, area.y) - area.y / 2f;
                    float blink = 0.5f + 0.5f * Mathf.Sin(t * 3f + s.x);
                    c = new Color(1f, 0.95f, 0.5f, b * blink);
                }
                else
                {
                    // Pétalos: caen en diagonal con el viento.
                    x = Mathf.Repeat(s.x * 5f + t * 1.2f + cam.position.x * 0.1f, area.x) - area.x / 2f;
                    y = area.y / 2f - Mathf.Repeat(s.y * 4f + t * 0.8f, area.y) + Mathf.Sin(t * 2f + s.x) * 0.3f;
                    c = (i % 4 == 1) ? new Color(0.88f, 0.21f, 0.35f, (1f - b) * 0.9f) : new Color(1f, 0.92f, 0.6f, (1f - b) * 0.6f);
                }
                dots[i].transform.position = new Vector3(cam.position.x + x, cam.position.y + y, 0f);
                dots[i].color = c;
            }
        }
    }
}
