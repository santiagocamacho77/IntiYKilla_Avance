using UnityEngine;

namespace IntiYKilla
{
    // Flor de kantuta del Pueblo: de día se abre y sirve de plataforma,
    // de noche se cierra y deja de sostener a Sisa.
    public class FlowerPlatform : MonoBehaviour
    {
        public SpriteRenderer sr;
        public Collider2D top;
        public int stemUnits = 4;
        [SerializeField] Sprite openSprite, closedSprite;
        float pop;

        public void Init(int stem)
        {
            stemUnits = stem;
            openSprite = SpriteFactory.Flower(stemUnits, true);
            closedSprite = SpriteFactory.Flower(stemUnits, false);
        }

        void Start()
        {
            if (openSprite == null || closedSprite == null) Init(stemUnits);
            DayNightManager.Instance.Changed += OnChanged;
            Apply(DayNightManager.Instance.IsNight);
        }

        void OnDestroy()
        {
            if (DayNightManager.Instance != null) DayNightManager.Instance.Changed -= OnChanged;
        }

        void OnChanged(bool night)
        {
            Apply(night);
            pop = 1f;
        }

        void Apply(bool night)
        {
            sr.sprite = night ? closedSprite : openSprite;
            top.enabled = !night;
        }

        void Update()
        {
            pop = Mathf.MoveTowards(pop, 0f, Time.deltaTime * 4f);
            float s = 1f + Mathf.Sin(pop * Mathf.PI) * 0.08f;
            sr.transform.localScale = new Vector3(s, 1f, 1f);
        }
    }
}
