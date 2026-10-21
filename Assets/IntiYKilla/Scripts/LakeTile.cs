using UnityEngine;

namespace IntiYKilla
{
    // Agua de la Laguna: de día es profunda y no se puede pisar (Sisa reaparece);
    // de noche se congela y se puede cruzar caminando.
    public class LakeTile : MonoBehaviour
    {
        public SpriteRenderer sr;
        public BoxCollider2D col;
        public int widthUnits = 10, heightUnits = 7;
        [SerializeField] Sprite water, ice;

        public void Init(int wUnits, int hUnits)
        {
            widthUnits = wUnits;
            heightUnits = hUnits;
            water = SpriteFactory.Water(wUnits, hUnits, false);
            ice = SpriteFactory.Water(wUnits, hUnits, true);
        }

        void Start()
        {
            if (water == null || ice == null) Init(widthUnits, heightUnits);
            DayNightManager.Instance.Changed += Apply;
            Apply(DayNightManager.Instance.IsNight);
        }

        void OnDestroy()
        {
            if (DayNightManager.Instance != null) DayNightManager.Instance.Changed -= Apply;
        }

        void Apply(bool night)
        {
            sr.sprite = night ? ice : water;
            col.isTrigger = !night;
        }

        void OnTriggerEnter2D(Collider2D other) => Touch(other);
        void OnTriggerStay2D(Collider2D other) => Touch(other);

        void Touch(Collider2D other)
        {
            if (DayNightManager.Instance.IsNight) return;
            var p = other.GetComponent<PlayerController>();
            if (p != null) p.Die(true);
        }
    }
}
