using UnityEngine;

namespace IntiYKilla
{
    // Estado del juego (título, jugando, fin del nivel) e interfaz en pantalla.
    public class GameManager : MonoBehaviour
    {
        public enum State { Title, Playing, LevelComplete }

        public static GameManager Instance { get; private set; }

        public PlayerController Player { get; set; }
        public CameraFollow Cam { get; set; }
        public State Current { get; private set; } = State.Title;
        public int Pieces { get; private set; }
        public const int TotalPieces = 6;
        public bool ShowInteract { get; set; }
        public static string LevelName =>
            Bootstrap.Level == 3 ? "Nivel 3 · El Bosque" : Bootstrap.Level == 2 ? "Nivel 2 · La Laguna" : "Nivel 1 · El Pueblo";

        string hintSpeaker, hintText;
        float hintTimer;
        string toast;
        float toastTimer;
        float fade;
        float playTime;
        float flash;
        bool flashNight;
        float shake;
        float endTime;

        Texture2D white, sunIcon, moonIcon, stoneIcon, sisaIcon, llamaIcon, shadowIcon, spiritIcon;
        GUIStyle title, subtitle, body, small, hintName, hintBody;

        void Awake()
        {
            Instance = this;
        }

        public int EnemiesDefeated { get; private set; }

        public void OnEnemyDefeated()
        {
            EnemiesDefeated++;
            Toast("¡Enemigo eliminado!");
        }

        void Start()
        {
            // En las escenas editables, Sisa y la cámara vienen de LevelInfo.
            if (LevelInfo.Instance != null)
            {
                if (Player == null) Player = LevelInfo.Instance.player;
                if (Cam == null) Cam = LevelInfo.Instance.cam;
            }
            white = Texture2D.whiteTexture;
            sunIcon = SpriteFactory.Sun().texture;
            moonIcon = SpriteFactory.Moon().texture;
            stoneIcon = SpriteFactory.StoneSprite().texture;
            sisaIcon = SpriteFactory.SisaIdleSprite().texture;
            llamaIcon = SpriteFactory.LlamaSprite().texture;
            shadowIcon = SpriteFactory.ShadowSprite().texture;
            spiritIcon = SpriteFactory.SpiritSprite().texture;
            if (Player != null) Player.InputEnabled = false;
            // Cada nivel anterior ya entregó su pedazo del Intihuatana.
            Pieces = Bootstrap.Level - 1;
            if (Bootstrap.SkipTitle)
            {
                Bootstrap.SkipTitle = false;
                StartLevel();
            }
            if (DayNightManager.Instance != null) DayNightManager.Instance.Changed += OnDayNightChanged;
        }

        void OnDestroy()
        {
            if (DayNightManager.Instance != null) DayNightManager.Instance.Changed -= OnDayNightChanged;
        }

        // Destello y pequeño temblor de cámara al cambiar entre Inti y Killa.
        void OnDayNightChanged(bool night)
        {
            flash = 1f;
            flashNight = night;
            shake = 0.18f;
            Sfx.Switch(night);
        }

        void LateUpdate()
        {
            if (shake > 0f && Cam != null)
            {
                Cam.transform.position += (Vector3)(Random.insideUnitCircle * shake * 0.6f);
                shake = Mathf.MoveTowards(shake, 0f, Time.deltaTime);
            }
        }

        void Update()
        {
            switch (Current)
            {
                case State.Title:
                    if (GameInput.Level2Pressed() && Bootstrap.Level != 2) LoadLevel(2);
                    else if (GameInput.Level3Pressed() && Bootstrap.Level != 3) LoadLevel(3);
                    else if (GameInput.StartPressed()) StartLevel();
                    break;
                case State.Playing:
                    playTime += Time.deltaTime;
                    break;
                case State.LevelComplete:
                    if (Time.time - endTime > 1f && GameInput.StartPressed()) LoadLevel(Bootstrap.Level >= 3 ? 1 : Bootstrap.Level + 1);
                    break;
            }
            if (Input_Escape()) Application.Quit();

            hintTimer -= Time.deltaTime;
            toastTimer -= Time.deltaTime;
            fade = Mathf.MoveTowards(fade, 0f, Time.deltaTime * 2f);
            flash = Mathf.MoveTowards(flash, 0f, Time.deltaTime * 3f);
        }

        static bool Input_Escape()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(KeyCode.Escape);
#elif ENABLE_INPUT_SYSTEM
            return UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
#else
            return false;
#endif
        }

        void StartLevel()
        {
            Current = State.Playing;
            Player.InputEnabled = true;
            Toast(LevelName);
        }

        void LoadLevel(int level)
        {
            Bootstrap.Level = level;
            Bootstrap.SkipTitle = true;
            string sceneName = "IntiYKilla_Nivel" + level;
            if (Application.CanStreamedLevelBeLoaded(sceneName))
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        public void SetHint(string speaker, string text)
        {
            hintSpeaker = speaker;
            hintText = text;
            hintTimer = 0.25f;
        }

        public void Toast(string text)
        {
            toast = text;
            toastTimer = 2.5f;
        }

        public void CollectPiece()
        {
            Pieces++;
            Toast("¡Recuperaste un pedazo del Intihuatana! Se abrió la puerta de piedra.");
        }

        public void OnPlayerRespawn()
        {
            fade = 1f;
            Cam?.SnapToTarget();
        }

        public void FinishLevel()
        {
            if (Current != State.Playing) return;
            Current = State.LevelComplete;
            endTime = Time.time;
            Player.InputEnabled = false;
        }

        // ---------------- Interfaz ----------------
        void BuildStyles()
        {
            float u = Screen.height / 720f;
            title = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(64 * u), fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            title.normal.textColor = new Color(1f, 0.85f, 0.35f);
            subtitle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(24 * u), alignment = TextAnchor.MiddleCenter };
            subtitle.normal.textColor = Color.white;
            body = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(20 * u), alignment = TextAnchor.MiddleLeft, wordWrap = true };
            body.normal.textColor = Color.white;
            small = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(15 * u), alignment = TextAnchor.MiddleLeft };
            small.normal.textColor = new Color(1f, 1f, 1f, 0.85f);
            hintName = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(20 * u), fontStyle = FontStyle.Bold };
            hintBody = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(20 * u), wordWrap = true };
            hintBody.normal.textColor = Color.white;
        }

        void Panel(Rect r, Color c)
        {
            var old = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, white);
            GUI.color = old;
        }

        void Icon(Rect r, Texture2D tex)
        {
            if (tex != null) GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit, true);
        }

        void OnGUI()
        {
            BuildStyles();
            float u = Screen.height / 720f;
            float W = Screen.width, H = Screen.height;

            if (fade > 0f) Panel(new Rect(0, 0, W, H), new Color(0f, 0f, 0f, fade * 0.7f));
            if (flash > 0f && Current == State.Playing)
                Panel(new Rect(0, 0, W, H), flashNight ? new Color(0.45f, 0.55f, 1f, flash * 0.35f) : new Color(1f, 0.85f, 0.4f, flash * 0.4f));

            switch (Current)
            {
                case State.Title: DrawTitle(W, H, u); break;
                case State.Playing: DrawHud(W, H, u); break;
                case State.LevelComplete: DrawEnd(W, H, u); break;
            }

        }

        void DrawTitle(float W, float H, float u)
        {
            Panel(new Rect(0, 0, W, H), new Color(0.04f, 0.03f, 0.1f, 0.72f));
            GUI.Label(new Rect(0, H * 0.10f, W, 90 * u), "Inti y Killa", title);
            GUI.Label(new Rect(0, H * 0.10f + 80 * u, W, 40 * u), LevelName, subtitle);

            float iconY = H * 0.10f + 135 * u;
            float s = 70 * u;
            float cx = W / 2f;
            Icon(new Rect(cx - s * 3.2f, iconY, s, s), sunIcon);
            Icon(new Rect(cx - s * 1.6f, iconY, s, s), sisaIcon);
            Icon(new Rect(cx, iconY + s * 0.25f, s * 0.8f, s * 0.8f), llamaIcon);
            Icon(new Rect(cx + s * 1.3f, iconY + s * 0.2f, s * 0.8f, s * 0.8f), shadowIcon);
            Icon(new Rect(cx + s * 2.5f, iconY, s, s), moonIcon);

            float bx = W / 2f - 330 * u, by = iconY + s + 25 * u;
            Panel(new Rect(bx - 60 * u, by, 780 * u, 280 * u), new Color(0f, 0f, 0f, 0.45f));
            string controls =
                "A / D o flechas: moverse\n" +
                "Espacio / W: saltar (mantén para saltar más alto)\n" +
                "Q: cambiar entre día (Inti) y noche (Killa)\n" +
                "W / flecha arriba en una enredadera: trepar\n" +
                "F: lanzar piedra con la huaraca · salta encima de los enemigos para eliminarlos\n" +
                "E: recoger el pedazo del Intihuatana\n" +
                "R: volver al último punto de control · Esc: salir";
            GUI.Label(new Rect(bx - 36 * u, by + 10 * u, 740 * u, 260 * u), controls, body);

            float blink = (Mathf.Sin(Time.unscaledTime * 4f) + 1f) / 2f;
            subtitle.normal.textColor = new Color(1f, 1f, 1f, 0.5f + blink * 0.5f);
            string startText = "Pulsa Enter para empezar  ·  Pulsa 2 o 3 para ir directo a la Laguna o al Bosque";
            GUI.Label(new Rect(0, by + 290 * u, W, 40 * u), startText, subtitle);
            subtitle.normal.textColor = Color.white;
        }

        void DrawHud(float W, float H, float u)
        {
            var dn = DayNightManager.Instance;
            bool night = dn != null && dn.IsNight;

            // Zona y pedazos
            Panel(new Rect(12 * u, 12 * u, 360 * u, 104 * u), new Color(0f, 0f, 0f, 0.4f));
            Icon(new Rect(24 * u, 82 * u, 28 * u, 28 * u), shadowIcon);
            GUI.Label(new Rect(60 * u, 81 * u, 300 * u, 30 * u), $"Enemigos eliminados: {EnemiesDefeated}", small);
            GUI.Label(new Rect(24 * u, 16 * u, 340 * u, 30 * u), LevelName, body);
            Icon(new Rect(24 * u, 50 * u, 28 * u, 28 * u), stoneIcon);
            GUI.Label(new Rect(60 * u, 49 * u, 300 * u, 30 * u), $"Pedazos del Intihuatana: {Pieces}/{TotalPieces}", small);

            // Indicador día / noche y pausa entre cambios
            float px = W - 232 * u;
            Panel(new Rect(px, 12 * u, 220 * u, 76 * u), new Color(0f, 0f, 0f, 0.4f));
            Icon(new Rect(px + 10 * u, 18 * u, 60 * u, 60 * u), night ? moonIcon : sunIcon);
            GUI.Label(new Rect(px + 78 * u, 16 * u, 140 * u, 30 * u), night ? "Noche · Killa" : "Día · Inti", body);
            float ready = dn == null ? 1f : 1f - dn.CooldownLeft / dn.cooldown;
            Panel(new Rect(px + 80 * u, 56 * u, 125 * u, 10 * u), new Color(1f, 1f, 1f, 0.2f));
            Panel(new Rect(px + 80 * u, 56 * u, 125 * u * ready, 10 * u), night ? new Color(0.7f, 0.8f, 1f) : new Color(1f, 0.8f, 0.3f));
            GUI.Label(new Rect(px + 80 * u, 64 * u, 140 * u, 22 * u), "Q para cambiar", small);

            // Pista de Inti o Killa
            if (hintTimer > 0f && !string.IsNullOrEmpty(hintText))
            {
                float bw = Mathf.Min(W - 40 * u, 900 * u);
                var r = new Rect((W - bw) / 2f, H - 150 * u, bw, 100 * u);
                Panel(r, new Color(0f, 0f, 0f, 0.6f));
                bool isKilla = hintSpeaker == "Killa";
                Icon(new Rect(r.x + 12 * u, r.y + 14 * u, 72 * u, 72 * u), isKilla ? moonIcon : sunIcon);
                hintName.normal.textColor = isKilla ? new Color(0.75f, 0.82f, 1f) : new Color(1f, 0.8f, 0.3f);
                GUI.Label(new Rect(r.x + 96 * u, r.y + 8 * u, 300 * u, 28 * u), hintSpeaker, hintName);
                GUI.Label(new Rect(r.x + 96 * u, r.y + 34 * u, bw - 110 * u, 64 * u), hintText, hintBody);
            }

            if (ShowInteract)
            {
                subtitle.normal.textColor = new Color(1f, 0.9f, 0.5f);
                GUI.Label(new Rect(0, H * 0.32f, W, 40 * u), "Pulsa E para recoger el pedazo del Intihuatana", subtitle);
                subtitle.normal.textColor = Color.white;
            }

            if (toastTimer > 0f && !string.IsNullOrEmpty(toast))
            {
                var c = subtitle.normal.textColor;
                subtitle.normal.textColor = new Color(1f, 1f, 1f, Mathf.Clamp01(toastTimer));
                GUI.Label(new Rect(0, 100 * u, W, 40 * u), toast, subtitle);
                subtitle.normal.textColor = c;
            }
        }

        void DrawEnd(float W, float H, float u)
        {
            Panel(new Rect(0, 0, W, H), new Color(0.04f, 0.03f, 0.1f, 0.78f));
            GUI.Label(new Rect(0, H * 0.14f, W, 90 * u), "¡Nivel " + Bootstrap.Level + " completado!", title);
            int mins = (int)(playTime / 60f), secs = (int)(playTime % 60f);
            GUI.Label(new Rect(0, H * 0.14f + 85 * u, W, 40 * u),
                $"Pedazos: {Pieces}/{TotalPieces}   ·   Enemigos: {EnemiesDefeated}   ·   Tiempo: {mins}:{secs:00}   ·   Reintentos: {Player.Deaths}", subtitle);

            float bx = W / 2f - 360 * u, by = H * 0.14f + 150 * u;
            Panel(new Rect(bx, by, 720 * u, 200 * u), new Color(0f, 0f, 0f, 0.45f));
            GUI.Label(new Rect(bx + 24 * u, by + 10 * u, 680 * u, 180 * u),
                Bootstrap.Level == 3
                    ? "Sisa recuperó el tercer pedazo del Intihuatana en el Bosque.\n\n" +
                      "Próximas zonas: Ruinas · Volcán · Cueva del Supay.\n" +
                      "¡Gracias por jugar!"
                    : Bootstrap.Level == 2
                        ? "Sisa recuperó el segundo pedazo del Intihuatana en la Laguna.\n\nSiguiente: Nivel 3, El Bosque."
                        : "Sisa recuperó el primer pedazo del Intihuatana.\n\nSiguiente: Nivel 2, La Laguna.", body);
            GUI.Label(new Rect(0, by + 220 * u, W, 40 * u),
                Bootstrap.Level < 3 ? "Pulsa Enter para continuar al Nivel " + (Bootstrap.Level + 1) + " · Esc para salir" : "Pulsa Enter para volver al Nivel 1 · Esc para salir", subtitle);
        }
    }
}
