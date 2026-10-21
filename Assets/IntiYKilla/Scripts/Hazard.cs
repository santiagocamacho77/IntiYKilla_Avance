using UnityEngine;

namespace IntiYKilla
{
    // Agua o caídas: Sisa reaparece en el último punto seguro.
    public class Hazard : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null) player.Die(true);
        }
    }
}
