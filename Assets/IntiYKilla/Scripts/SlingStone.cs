using UnityEngine;

namespace IntiYKilla
{
    // Piedra lanzada con la huaraca de Sisa: elimina a las sombras, espíritus y guardianes.
    [RequireComponent(typeof(Rigidbody2D))]
    public class SlingStone : MonoBehaviour
    {
        public float life = 0.9f;
        float dir = 1f;

        public static void Throw(Vector3 from, int facing)
        {
            var go = new GameObject("Piedra de huaraca");
            go.transform.position = from;
            var sr = SpriteFactory.AddRenderer(go, SpriteFactory.SlingStoneSprite(), 11);
            var glowGo = new GameObject("Brillo");
            glowGo.transform.SetParent(go.transform, false);
            glowGo.transform.localScale = Vector3.one * 0.9f;
            var glow = SpriteFactory.AddRenderer(glowGo, SpriteFactory.Glow(), 10);
            glow.color = new Color(1f, 0.85f, 0.4f, 0.8f);
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            var c = go.AddComponent<CircleCollider2D>();
            c.isTrigger = true;
            c.radius = 0.18f;
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = new Vector2(facing * 15f, 0f);
#else
            rb.velocity = new Vector2(facing * 15f, 0f);
#endif
            var stone = go.AddComponent<SlingStone>();
            stone.dir = facing;
            sr.flipX = facing < 0;
        }

        void Update()
        {
            transform.Rotate(0f, 0f, -720f * dir * Time.deltaTime);
            life -= Time.deltaTime;
            if (life <= 0f) Destroy(gameObject);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<PlayerController>() != null) return;
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Kill();
                Destroy(gameObject);
                return;
            }
            // Choca con el suelo o una pared; atraviesa las zonas de pista, el agua, etc.
            if (!other.isTrigger) Destroy(gameObject);
        }
    }
}
