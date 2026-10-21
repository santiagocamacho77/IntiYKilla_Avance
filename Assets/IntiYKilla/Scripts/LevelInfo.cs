using UnityEngine;

namespace IntiYKilla
{
    // Datos de la escena del nivel: qué nivel es y quiénes son Sisa y la cámara.
    public class LevelInfo : MonoBehaviour
    {
        public static LevelInfo Instance { get; private set; }

        [Range(1, 3)] public int level = 1;
        public PlayerController player;
        public CameraFollow cam;

        void Awake()
        {
            Instance = this;
            Bootstrap.Level = level;
        }
    }
}
