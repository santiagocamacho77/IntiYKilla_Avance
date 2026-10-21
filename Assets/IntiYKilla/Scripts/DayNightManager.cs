using System;
using UnityEngine;

namespace IntiYKilla
{
    // Mecánica central: cambiar entre día (Inti) y noche (Killa) con la tecla Q.
    // Tiene una pausa corta entre cambios para que no se use sin pensar.
    public class DayNightManager : MonoBehaviour
    {
        public static DayNightManager Instance { get; private set; }

        public float cooldown = 0.6f;
        public float blendSpeed = 4f;
        // El mundo empieza de noche: el día y la noche están descontrolados.
        public bool startAtNight = true;

        public bool IsNight { get; private set; }
        // 0 = día, 1 = noche. Se usa para las transiciones suaves de color.
        public float Blend { get; private set; }
        public float CooldownLeft { get; private set; }

        public event Action<bool> Changed;

        void Awake()
        {
            Instance = this;
            SetNight(startAtNight, true);
        }

        public void SetNight(bool night, bool instant)
        {
            IsNight = night;
            if (instant) Blend = night ? 1f : 0f;
            Changed?.Invoke(IsNight);
        }

        public bool TrySwitch()
        {
            if (CooldownLeft > 0f) return false;
            CooldownLeft = cooldown;
            SetNight(!IsNight, false);
            return true;
        }

        void Update()
        {
            CooldownLeft = Mathf.Max(0f, CooldownLeft - Time.deltaTime);
            Blend = Mathf.MoveTowards(Blend, IsNight ? 1f : 0f, blendSpeed * Time.deltaTime);
        }
    }
}
