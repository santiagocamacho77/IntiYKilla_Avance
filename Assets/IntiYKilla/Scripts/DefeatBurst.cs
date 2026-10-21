using UnityEngine;

namespace IntiYKilla
{
    // Chispas de luz cuando un enemigo es eliminado.
    public class DefeatBurst : MonoBehaviour
    {
        Transform[] bits;
        Vector2[] vel;
        SpriteRenderer[] srs;
        float t;
        const float Duration = 0.6f;

        public static void Spawn(Vector3 at, Color color)
        {
            var go = new GameObject("Destello");
            go.transform.position = at;
            var b = go.AddComponent<DefeatBurst>();
            b.Init(color);
        }

        void Init(Color color)
        {
            int n = 12;
            bits = new Transform[n];
            vel = new Vector2[n];
            srs = new SpriteRenderer[n];
            for (int i = 0; i < n; i++)
            {
                var g = new GameObject("Chispa");
                g.transform.SetParent(transform, false);
                g.transform.localScale = Vector3.one * 0.16f;
                srs[i] = SpriteFactory.AddRenderer(g, SpriteFactory.Pixel(), 12);
                srs[i].color = i % 2 == 0 ? color : new Color(1f, 0.95f, 0.6f);
                float a = i * Mathf.PI * 2f / n;
                vel[i] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * (3f + (i % 3));
                bits[i] = g.transform;
            }
        }

        void Update()
        {
            t += Time.deltaTime;
            for (int i = 0; i < bits.Length; i++)
            {
                bits[i].localPosition += (Vector3)(vel[i] * Time.deltaTime);
                vel[i] *= 0.92f;
                var c = srs[i].color; c.a = 1f - t / Duration; srs[i].color = c;
            }
            if (t >= Duration) Destroy(gameObject);
        }
    }
}
