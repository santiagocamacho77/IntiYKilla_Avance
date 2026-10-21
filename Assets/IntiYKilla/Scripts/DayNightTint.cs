using UnityEngine;

namespace IntiYKilla
{
    // Cambia el color (y opcionalmente la transparencia) de un sprite según el día o la noche.
    public class DayNightTint : MonoBehaviour
    {
        public Color day = SpriteFactory.DayTint;
        public Color night = SpriteFactory.NightTint;
        SpriteRenderer sr;

        void Awake() => sr = GetComponent<SpriteRenderer>();

        void LateUpdate()
        {
            var dn = DayNightManager.Instance;
            if (dn == null || sr == null) return;
            sr.color = Color.Lerp(day, night, dn.Blend);
        }
    }
}
