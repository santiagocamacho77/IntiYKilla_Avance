using UnityEngine;

namespace IntiYKilla
{
    // Efectos de sonido sintetizados por código (provisionales, igual que el arte).
    public class Sfx : MonoBehaviour
    {
        public static Sfx Instance { get; private set; }

        AudioSource source;
        AudioClip jump, toDay, toNight, pickup, hurt, checkpoint, sling, defeat;

        void Awake()
        {
            Instance = this;
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            jump = Tone("salto", new[] { 520f, 780f }, 0.09f, 0.25f);
            toDay = Tone("dia", new[] { 523f, 659f, 784f, 1046f }, 0.07f, 0.22f);
            toNight = Tone("noche", new[] { 784f, 659f, 523f, 392f }, 0.08f, 0.22f);
            pickup = Tone("pedazo", new[] { 523f, 659f, 784f, 1046f, 1318f }, 0.09f, 0.3f);
            hurt = Tone("golpe", new[] { 220f, 160f, 110f }, 0.08f, 0.3f);
            checkpoint = Tone("apacheta", new[] { 659f, 988f }, 0.1f, 0.22f);
            sling = Tone("huaraca", new[] { 880f, 660f }, 0.04f, 0.18f);
            defeat = Tone("enemigo", new[] { 988f, 784f, 1175f, 1568f }, 0.05f, 0.25f);
        }

        // Melodía corta de onda cuadrada suavizada.
        static AudioClip Tone(string name, float[] notes, float noteLen, float volume)
        {
            const int rate = 44100;
            int perNote = (int)(rate * noteLen);
            var data = new float[perNote * notes.Length];
            for (int n = 0; n < notes.Length; n++)
                for (int i = 0; i < perNote; i++)
                {
                    float t = i / (float)rate;
                    float env = Mathf.Clamp01(i / 200f) * (1f - i / (float)perNote);
                    float wave = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * notes[n] * t)) * 0.5f + Mathf.Sin(2f * Mathf.PI * notes[n] * t) * 0.5f;
                    data[n * perNote + i] = wave * env * volume;
                }
            var clip = AudioClip.Create(name, data.Length, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }

        void Play(AudioClip clip) { if (clip != null) source.PlayOneShot(clip); }

        public static void Jump() => Instance?.Play(Instance.jump);
        public static void Switch(bool night) => Instance?.Play(night ? Instance.toNight : Instance.toDay);
        public static void Pickup() => Instance?.Play(Instance.pickup);
        public static void Hurt() => Instance?.Play(Instance.hurt);
        public static void Checkpoint() => Instance?.Play(Instance.checkpoint);
        public static void Sling() => Instance?.Play(Instance.sling);
        public static void Defeat() => Instance?.Play(Instance.defeat);
    }
}
