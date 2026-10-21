using UnityEngine;

namespace IntiYKilla
{
    // Camino de luna (Pueblo) o rama de luna (Bosque): Killa revela estas plataformas solo de noche.
    // De día queda un contorno tenue para que el jugador sepa dónde aparecerán.
    public class MoonStone : MonoBehaviour
    {
        public SpriteRenderer sr;
        public Collider2D col;
        public SpriteRenderer glow;
        float baseY;

        void Start()
        {
            baseY = transform.position.y;
            DayNightManager.Instance.Changed += Apply;
            Apply(DayNightManager.Instance.IsNight);
        }

        void OnDestroy()
        {
            if (DayNightManager.Instance != null) DayNightManager.Instance.Changed -= Apply;
        }

        void Apply(bool night) => col.enabled = night;

        void Update()
        {
            float b = DayNightManager.Instance.Blend;
            sr.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.18f, 1f, b));
            // Brillo suave al aparecer.
            if (glow != null) glow.color = new Color(0.6f, 0.75f, 1f, b * (0.5f + 0.15f * Mathf.Sin(Time.time * 3f)));
        }
    }
}
