using UnityEngine;

namespace IntiYKilla
{
    // Pedazo del Intihuatana. Se recoge acercándose y pulsando E.
    public class StonePiece : MonoBehaviour
    {
        public Transform visual;
        public Gate gate;
        public float radius = 1.4f;
        bool taken;
        float baseY;

        void Start() => baseY = visual.localPosition.y;

        void Update()
        {
            if (taken) return;
            var lp = visual.localPosition;
            lp.y = baseY + Mathf.Sin(Time.time * 2.5f) * 0.15f;
            visual.localPosition = lp;
            visual.Rotate(0f, 0f, Mathf.Sin(Time.time) * 10f * Time.deltaTime);

            var gm = GameManager.Instance;
            if (gm == null || gm.Player == null) return;
            bool near = Vector2.Distance(gm.Player.transform.position + Vector3.up * 0.5f, transform.position) < radius;
            gm.ShowInteract = near;
            if (near && GameInput.InteractPressed())
            {
                taken = true;
                gm.ShowInteract = false;
                gm.CollectPiece();
                Sfx.Pickup();
                if (gate != null) gate.Open();
                gameObject.SetActive(false);
            }
        }
    }
}
