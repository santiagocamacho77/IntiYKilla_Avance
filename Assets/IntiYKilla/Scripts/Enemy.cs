using UnityEngine;

namespace IntiYKilla
{
    // Enemigos menores del Supay: solo existen de noche y la luz de Inti los disuelve.
    // Sisa también puede eliminarlos con su huaraca (F) o saltándoles encima.
    // Sombra: camina de un lado a otro sobre el suelo.
    // Espíritu: flota describiendo una onda.
    // Guardián del bosque: persigue a Sisa si se acerca de noche y vuelve a su lugar de día.
    public class Enemy : MonoBehaviour
    {
        public enum Kind { Shadow, Spirit, Chaser }

        public Kind kind;
        public float minX, maxX;
        public float speed = 2f;
        public float waveAmplitude = 1.2f;
        public float waveSpeed = 2f;
        public float chaseRadius = 7f;
        public SpriteRenderer sr;
        public Collider2D col;

        float baseY, dir = 1f, presence;
        Vector3 home;

        void Start()
        {
            baseY = transform.position.y;
            home = transform.position;
        }

        void Update()
        {
            var dn = DayNightManager.Instance;
            bool night = dn != null && dn.IsNight;
            presence = Mathf.MoveTowards(presence, night ? 1f : 0f, Time.deltaTime * 3f);
            col.enabled = presence > 0.6f;

            var p = transform.position;
            if (kind == Kind.Chaser)
            {
                var gm = GameManager.Instance;
                Vector3 target = home + Vector3.up * Mathf.Sin(Time.time * 2f) * 0.4f;
                if (night && gm != null && gm.Player != null && !gm.Player.IsInvulnerable &&
                    Vector2.Distance(gm.Player.transform.position, home) < chaseRadius)
                    target = gm.Player.transform.position + Vector3.up * 0.5f;
                float spd = night ? speed : speed * 2f;
                var np = Vector3.MoveTowards(p, target, spd * Time.deltaTime);
                if (Mathf.Abs(np.x - p.x) > 0.001f) dir = Mathf.Sign(np.x - p.x);
                transform.position = np;
                sr.flipX = dir < 0f;
                transform.localScale = new Vector3(Mathf.Lerp(0.3f, 1f, presence), Mathf.Lerp(0.3f, 1f, presence), 1f);
                var cc = sr.color; cc.a = presence; sr.color = cc;
                return;
            }
            p.x += dir * speed * Time.deltaTime;
            if (p.x > maxX) { p.x = maxX; dir = -1f; }
            if (p.x < minX) { p.x = minX; dir = 1f; }
            if (kind == Kind.Spirit) p.y = baseY + Mathf.Sin(Time.time * waveSpeed) * waveAmplitude;
            transform.position = p;

            sr.flipX = dir < 0f;
            // Al amanecer se encoge y se desvanece; al anochecer vuelve a formarse.
            float wobble = kind == Kind.Shadow ? 1f + Mathf.Sin(Time.time * 10f) * 0.05f : 1f;
            transform.localScale = new Vector3(Mathf.Lerp(0.3f, 1f, presence) * wobble, Mathf.Lerp(0.1f, 1f, presence), 1f);
            var c = sr.color; c.a = presence; sr.color = c;
        }

        void OnTriggerEnter2D(Collider2D other) => Touch(other);
        void OnTriggerStay2D(Collider2D other) => Touch(other);

        void Touch(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null || dead) return;
            // Si Sisa cae desde arriba, aplasta al enemigo y rebota.
            if (player.FallingSpeed < -1f && player.transform.position.y > transform.position.y + (kind == Kind.Shadow ? 0.25f : -0.1f))
            {
                Kill();
                player.Bounce();
                return;
            }
            player.Die();
        }

        bool dead;

        public void Kill()
        {
            if (dead) return;
            dead = true;
            var c = kind == Kind.Shadow ? new Color(0.5f, 0.2f, 0.7f) : kind == Kind.Spirit ? new Color(0.6f, 0.9f, 1f) : new Color(0.9f, 0.3f, 0.5f);
            DefeatBurst.Spawn(transform.position + Vector3.up * (kind == Kind.Shadow ? 0.4f : 0f), c);
            Sfx.Defeat();
            GameManager.Instance?.OnEnemyDefeated();
            gameObject.SetActive(false);
        }
    }
}
