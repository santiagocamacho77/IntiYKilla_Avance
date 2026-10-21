using UnityEngine;

namespace IntiYKilla
{
    // Zona donde Inti (de día) o Killa (de noche) le dan una pista a Sisa.
    public class HintTrigger : MonoBehaviour
    {
        [TextArea] public string dayText;
        [TextArea] public string nightText;

        void OnTriggerStay2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>() == null) return;
            var gm = GameManager.Instance;
            if (gm == null) return;
            bool night = DayNightManager.Instance.IsNight;
            string text = night ? nightText : dayText;
            if (!string.IsNullOrEmpty(text)) gm.SetHint(night ? "Killa" : "Inti", text);
        }
    }
}
