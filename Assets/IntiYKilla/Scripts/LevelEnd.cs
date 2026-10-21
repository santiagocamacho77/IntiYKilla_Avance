using UnityEngine;

namespace IntiYKilla
{
    public class LevelEnd : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() != null) GameManager.Instance?.FinishLevel();
        }
    }
}
