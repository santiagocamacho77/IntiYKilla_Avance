using UnityEngine;

namespace IntiYKilla
{
    // Enredadera del Bosque: de día crece y Sisa puede treparla (W / flecha arriba);
    // de noche se seca y se encoge.
    public class Vine : MonoBehaviour
    {
        public SpriteRenderer sr;
        public Collider2D zone;
        public int heightUnits = 5;
        [SerializeField] Sprite grown, withered;
        float growth;

        public void Init(int height)
        {
            heightUnits = height;
            grown = SpriteFactory.Vine(heightUnits, true);
            withered = SpriteFactory.Vine(heightUnits, false);
        }

        void Start()
        {
            if (grown == null || withered == null) Init(heightUnits);
            growth = DayNightManager.Instance.IsNight ? 0f : 1f;
        }

        void Update()
        {
            bool night = DayNightManager.Instance.IsNight;
            growth = Mathf.MoveTowards(growth, night ? 0f : 1f, Time.deltaTime * 3f);
            zone.enabled = growth > 0.95f;
            // Crece desde la base: se estira en vertical mientras aparece.
            sr.sprite = growth > 0.05f ? grown : withered;
            sr.transform.localScale = new Vector3(1f, growth > 0.05f ? Mathf.SmoothStep(0.1f, 1f, growth) : 1f, 1f);
        }

        void OnTriggerStay2D(Collider2D other)
        {
            var p = other.GetComponent<PlayerController>();
            if (p != null) p.TouchVine(transform.position.x);
        }
    }
}
