using UnityEngine;

namespace IntiYKilla
{
    // Apacheta: al pasar junto a ella se guarda el punto de reaparición.
    public class Checkpoint : MonoBehaviour
    {
        public SpriteRenderer sr;
        bool active;

        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player == null || active) return;
            active = true;
            player.RespawnPoint = transform.position + Vector3.up * 0.1f;
            sr.sprite = SpriteFactory.Apacheta(true);
            Sfx.Checkpoint();
            GameManager.Instance?.Toast("Punto de control: apacheta");
        }
    }
}
