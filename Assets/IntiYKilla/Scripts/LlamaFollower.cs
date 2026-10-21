using UnityEngine;

namespace IntiYKilla
{
    // La llama acompaña a Sisa siguiendo su recorrido con un poco de retraso.
    // No tiene habilidades: solo reacciona (da un saltito cuando cambia el día o la noche).
    public class LlamaFollower : MonoBehaviour
    {
        public PlayerController player;
        public SpriteRenderer sr;
        public int delayFrames = 22;
        float hop;
        float lastX;

        void Start()
        {
            DayNightManager.Instance.Changed += _ => hop = 1f;
            lastX = transform.position.x;
        }

        void LateUpdate()
        {
            if (player == null) return;
            var trail = player.Trail;
            Vector3 target;
            if (trail.Count > delayFrames) target = trail[trail.Count - 1 - delayFrames];
            else target = player.transform.position - new Vector3(player.Facing * 1.2f, 0f, 0f);
            target.x -= player.Facing * 0.6f;

            hop = Mathf.MoveTowards(hop, 0f, Time.deltaTime * 2.5f);
            target.y += Mathf.Sin(hop * Mathf.PI) * 0.6f;

            transform.position = Vector3.Lerp(transform.position, target, 1f - Mathf.Exp(-12f * Time.deltaTime));
            float dx = transform.position.x - lastX;
            if (Mathf.Abs(dx) > 0.002f) sr.flipX = dx < 0f;
            lastX = transform.position.x;
        }
    }
}
