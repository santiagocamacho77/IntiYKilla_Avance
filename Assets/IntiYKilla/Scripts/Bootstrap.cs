using UnityEngine;

namespace IntiYKilla
{
    // Arma el nivel completo por código: cámara, fondo, escenario, Sisa, la llama,
    // enemigos e interfaz. Se usa de dos formas:
    // - El editor (menú Inti y Killa > Generar escenas editables) llama a Build() para crear
    //   las escenas IntiYKilla_Nivel1/2/3 con todos los objetos, listas para editar a mano.
    // - Si se pone en una escena vacía, arma el nivel al darle Play.
    // Nivel 1 = El Pueblo, Nivel 2 = La Laguna, Nivel 3 = El Bosque.
    public class Bootstrap : MonoBehaviour
    {
        const float KillY = -7f;

        // Se conservan al recargar la escena para pasar de un nivel al siguiente.
        public static int Level = 1;
        public static bool SkipTitle;

        bool forest;

        Transform world;
        Transform camT;

        void Awake() => Build(Level);

        LevelInfo info;

        public void Build(int level)
        {
            Level = level;
            world = new GameObject("Mundo").transform;

            var manager = new GameObject("Sistemas");
            manager.AddComponent<DayNightManager>();
            var gm = manager.AddComponent<GameManager>();
            manager.AddComponent<Sfx>();
            info = manager.AddComponent<LevelInfo>();
            info.level = level;

            forest = Level == 3;
            SetupCamera();
            BuildBackground();
            if (Level == 3) BuildLevel3(gm);
            else if (Level == 2) BuildLevel2(gm);
            else BuildLevel1(gm);
        }

        // ---------------- Cámara y fondo ----------------
        void SetupCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera") { tag = "MainCamera" };
                cam = go.AddComponent<Camera>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 6.5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.12f);
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 100f;
            if (cam.GetComponent<AudioListener>() == null) cam.gameObject.AddComponent<AudioListener>();
            camT = cam.transform;
            camT.position = new Vector3(0f, 2f, -10f);
        }

        void BuildBackground()
        {
            // Cielo de día y de noche (se mezclan según el momento).
            var skyDay = AddSprite("Cielo día", SpriteFactory.Sky(false), -100, camT, new Vector3(0, 0, 50));
            skyDay.transform.localScale = new Vector3(60f, 1f, 1f);
            var skyNight = AddSprite("Cielo noche", SpriteFactory.Sky(true), -99, camT, new Vector3(0, 0, 49));
            skyNight.transform.localScale = new Vector3(60f, 1f, 1f);
            Tint(skyNight, new Color(1, 1, 1, 0), Color.white);

            var stars = AddSprite("Estrellas", SpriteFactory.Stars(), -98, camT, new Vector3(0, 1f, 48));
            stars.transform.localScale = new Vector3(1.6f, 1.2f, 1f);
            Tint(stars, new Color(1, 1, 1, 0), Color.white);

            var sun = AddSprite("Inti", SpriteFactory.Sun(), -97, camT, new Vector3(7.5f, 3.6f, 47));
            Tint(sun, Color.white, new Color(1, 1, 1, 0));
            var moon = AddSprite("Killa", SpriteFactory.Moon(), -97, camT, new Vector3(7.5f, 3.6f, 47));
            Tint(moon, new Color(1, 1, 1, 0), Color.white);

            var clouds = new GameObject("Nubes").AddComponent<CloudLayer>();
            clouds.transform.SetParent(world, false);
            clouds.cam = camT;

            var ambient = new GameObject("Ambiente").AddComponent<AmbientParticles>();
            ambient.transform.SetParent(world, false);
            ambient.cam = camT;

            // Cordillera lejana y cerros cercanos con parallax.
            var far = AddSprite("Andes lejanos",
                SpriteFactory.Mountains(56, 14, 1, new Color(0.45f, 0.5f, 0.72f), new Color(0.95f, 0.97f, 1f), 0.45f), -90, null, Vector3.zero);
            Tint(far, Color.white, new Color(0.45f, 0.5f, 0.85f));
            AddParallax(far, 0.85f, new Vector3(6f, -5f, 40f));

            if (!forest)
            {
                var near = AddSprite("Cerros",
                    SpriteFactory.Mountains(80, 9, 3, new Color(0.42f, 0.55f, 0.35f), new Color(0.6f, 0.7f, 0.45f), 0.95f), -80, null, Vector3.zero);
                Tint(near, Color.white, new Color(0.35f, 0.42f, 0.75f));
                AddParallax(near, 0.6f, new Vector3(16f, -6f, 30f));
            }
            else
            {
                // Dos capas de árboles con parallax para dar profundidad al Bosque.
                var back = AddSprite("Bosque lejano", SpriteFactory.ForestLayer(80, 14, 11, new Color(0.22f, 0.42f, 0.36f)), -85, null, Vector3.zero);
                Tint(back, Color.white, new Color(0.3f, 0.36f, 0.7f));
                AddParallax(back, 0.7f, new Vector3(14f, -6f, 35f));
                var front = AddSprite("Bosque cercano", SpriteFactory.ForestLayer(100, 12, 23, new Color(0.13f, 0.3f, 0.2f)), -80, null, Vector3.zero);
                Tint(front, Color.white, new Color(0.2f, 0.25f, 0.55f));
                AddParallax(front, 0.5f, new Vector3(24f, -7f, 30f));
            }
        }

        void AddParallax(SpriteRenderer sr, float factor, Vector3 offset)
        {
            var p = sr.gameObject.AddComponent<Parallax>();
            p.cam = camT;
            p.factor = factor;
            p.offset = offset;
        }

        // ---------------- Nivel 1: El Pueblo ----------------
        void BuildLevel1(GameManager gm)
        {
            // Tramo A: el pueblo (empieza de noche, con una sombra rondando).
            MakeGround(-14, 18, 0f);
            Wall(-9f);
            MakeDecor(SpriteFactory.House(), new Vector2(1f, 0f), -6);
            MakeDecor(SpriteFactory.House(), new Vector2(10f, 0f), -6);
            for (int i = 0; i < 6; i++) MakeDecor(SpriteFactory.Ichu(), new Vector2(-6f + i * 4.1f, 0f), -4);
            MakeCheckpoint(-3f, 0f, true);
            MakeShadow(5f, 14f, 0f);
            MakeHint(-8, 8,
                "Sisa, el Intihuatana se rompió y el día y la noche se descontrolaron. Ve hacia la derecha y busca el primer pedazo.",
                "Es de noche y una sombra del Supay ronda el pueblo. Pulsa Q para llamar a Inti y disolverla, o lánzale una piedra con tu huaraca (F).");

            // Barranco con flores de kantuta (solo sostienen de día).
            MakeStream(18, 30);
            MakeFlower(21.5f, 0f, 6);
            MakeFlower(26.5f, 0f, 6);
            MakeHint(13, 20,
                "Mis flores de kantuta se abren con la luz. ¡Salta sobre ellas para cruzar!",
                "De noche las flores duermen y no te sostienen. Cambia al día con Q para cruzar.");

            // Tramo B y camino de luna (solo existe de noche, con un espíritu encima).
            MakeGround(30, 42, 0f);
            MakeCheckpoint(32f, 0f, false);
            for (int i = 0; i < 3; i++) MakeDecor(SpriteFactory.Ichu(), new Vector2(34f + i * 2.7f, 0f), -4);
            MakeStream(42, 58);
            MakeMoonStone(45f, 0.6f, 2);
            MakeMoonStone(50f, 1.4f, 2);
            MakeMoonStone(55f, 0.6f, 2);
            MakeSpirit(44f, 56f, 3.2f);
            MakeHint(36, 43,
                "Este barranco no tiene flores... Quizás Killa conozca otro camino.",
                "De noche te muestro caminos ocultos. ¡Cuidado con el espíritu que vuela sobre ellos!");

            // Tramo C: combinar los dos estados (piedra de luna + flor de día).
            MakeGround(58, 68, 0f);
            MakeCheckpoint(59f, 0f, false);
            MakeShadow(61f, 66.5f, 0f);
            MakeMoonStone(62.5f, 1.4f, 2);
            MakeFlower(65.5f, 3.4f, 3);
            MakeHint(58, 68,
                "La repisa está muy alta. Desde la piedra de luna, salta y cambia al día en el aire para que se abra la flor.",
                "Súbete a mi piedra, salta y pulsa Q en el aire: la flor de Inti se abrirá bajo tus pies.");

            // Repisa alta: pedazo del Intihuatana y puerta de piedra.
            MakeBlock(68, 100, 5.6f);
            var gate = MakeGate(82f, 5.6f, 5);
            MakePiece(76f, 6.7f, gate);
            MakeHint(70, 80,
                "¡Ahí está un pedazo del Intihuatana! Acércate y pulsa E para recogerlo.",
                "¡Ahí está un pedazo del Intihuatana! Acércate y pulsa E para recogerlo.");
            Trigger<LevelEnd>("Fin del nivel", new Vector2(88f, 8f), new Vector2(2f, 6f));

            SpawnPlayer(gm, new Vector2(-3f, 0.1f), 84f, 9f);
        }

        // ---------------- Nivel 2: La Laguna ----------------
        // De día el agua es profunda; de noche se congela y se puede cruzar.
        void BuildLevel2(GameManager gm)
        {
            // Orilla de inicio.
            MakeGround(-14, 10, 0f);
            Wall(-9f);
            MakeDecor(SpriteFactory.House(), new Vector2(-4f, 0f), -6);
            for (int i = 0; i < 4; i++) MakeDecor(SpriteFactory.Totora(), new Vector2(3f + i * 1.6f, 0f), -4);
            MakeCheckpoint(-1f, 0f, true);
            MakeHint(-8, 10,
                "De día el agua de la laguna es profunda. Si caes, vuelves a la última apacheta.",
                "De noche el agua se congela y puedes cruzarla... pero una sombra patina sobre el hielo. Espera tu momento.");

            // Laguna 1: una sombra recorre el hielo de noche.
            MakeLake(10, 24);
            MakeShadow(12f, 22f, -0.3f, 2.2f);

            // Islote con apacheta.
            MakeGround(24, 27, 0.5f);
            MakeCheckpoint(25.5f, 0.5f, false);
            MakeDecor(SpriteFactory.Totora(), new Vector2(26.4f, 0.5f), -4);

            // Laguna 2: espíritu que vuela bajo; flor en el agua y repisa con el pedazo.
            MakeLake(27, 47);
            MakeSpirit(29f, 45f, 3.3f);
            MakeFlower(37f, 2.8f, 4);
            MakeFloatingRock(39, 45, 5f);
            var gate = MakeGate(56f, 0f, 5);
            MakePiece(42.5f, 6.1f, gate);
            MakeHint(31, 39,
                "Mi flor se abre de día, pero entonces el hielo se derrite. Párate junto a ella de noche, salta y cambia al día en el aire.",
                "Desde el hielo, junto a la flor: salta y pulsa Q en el aire para que Inti la abra bajo tus pies. ¡Cuidado con el espíritu!");
            MakeHint(39, 46,
                "¡Un pedazo del Intihuatana! Acércate y pulsa E.",
                "¡Un pedazo del Intihuatana! Acércate y pulsa E.");

            // Orilla final con la puerta de piedra.
            MakeGround(47, 70, 0f);
            MakeCheckpoint(48.5f, 0f, false);
            for (int i = 0; i < 3; i++) MakeDecor(SpriteFactory.Totora(), new Vector2(50f + i * 1.7f, 0f), -4);
            MakeShadow(50f, 54.5f, 0f, 1.6f);
            Trigger<LevelEnd>("Fin del nivel", new Vector2(61f, 2f), new Vector2(2f, 6f));

            SpawnPlayer(gm, new Vector2(-1f, 0.1f), 52f, 9f);
        }

        void MakeLake(int x0, int x1)
        {
            int w = x1 - x0;
            const float top = -0.3f;
            int h = Mathf.CeilToInt(top - KillY);
            var go = Node("Laguna", new Vector3((x0 + x1) / 2f, top, 0));
            var sr = SpriteFactory.AddRenderer(go, SpriteFactory.Water(w, h, true), 1);
            Tint(sr, Color.white, SpriteFactory.NightTint);
            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(w, h);
            box.offset = new Vector2(0, -h / 2f);
            var lake = go.AddComponent<LakeTile>();
            lake.sr = sr;
            lake.col = box;
            lake.Init(w, h);
        }

        void MakeFloatingRock(int x0, int x1, float top)
        {
            int w = x1 - x0;
            var sr = AddSprite("Roca flotante", SpriteFactory.StoneBlock(w, 1), 0, null, new Vector3((x0 + x1) / 2f, top, 0));
            Tint(sr, Color.white, SpriteFactory.NightTint);
            var box = sr.gameObject.AddComponent<BoxCollider2D>();
            box.size = new Vector2(w, 1f);
            box.offset = new Vector2(0, -0.5f);
        }

        // ---------------- Nivel 3: El Bosque ----------------
        // De día crecen enredaderas para trepar; de noche Killa revela ramas ocultas.
        void BuildLevel3(GameManager gm)
        {
            // Tramo A: entrada al bosque, con una sombra veloz.
            MakeGround(-14, 16, 0f);
            Wall(-9f);
            MakeTree(-6f, 0f, 0); MakeTree(2f, 0f, 1); MakeTree(9f, 0f, 0);
            for (int i = 0; i < 4; i++) MakeDecor(SpriteFactory.Ichu(), new Vector2(-5f + i * 5.3f, 0f), -4);
            MakeCheckpoint(-3f, 0f, true);
            MakeShadow(4f, 14f, 0f, 2.6f);
            MakeHint(-8, 8,
                "Bienvenida al Bosque, Sisa. De día mis enredaderas crecen y puedes treparlas.",
                "En el Bosque de noche te muestro ramas ocultas... pero la sombra de aquí corre más rápido. Pulsa Q para llamar a Inti.");

            // Acantilado con enredadera (solo crece de día).
            MakeBlock(16, 34, 6f);
            MakeVine(15.4f, 0f, 7);
            MakeHint(11, 16,
                "Acércate a la enredadera y mantén W o la flecha arriba para trepar. Espacio para soltarte.",
                "De noche la enredadera se seca. Cambia al día con Q para que vuelva a crecer.");

            // Lo alto del acantilado: un espíritu vuela de noche.
            MakeCheckpoint(18f, 6f, false);
            MakeTree(24f, 6f, 1); MakeTree(31f, 6f, 0);
            MakeSpirit(21f, 32f, 8.4f);

            // Barranco con ramas de luna (solo existen de noche).
            MakeStream(34, 52);
            MakeMoonStone(37f, 5.4f, 2);
            MakeMoonStone(41.5f, 6.2f, 2);
            MakeMoonStone(46f, 5.4f, 2);
            MakeMoonStone(50f, 6f, 2);
            MakeSpirit(36f, 50f, 9.2f);
            MakeHint(27, 34,
                "No hay camino para cruzar... Killa conoce los secretos del bosque.",
                "Mis ramas de luna solo aparecen de noche. ¡Esquiva al espíritu que las vigila!");

            // Tramo B: claro del bosque con un guardián que persigue de noche.
            MakeGround(52, 66, 2f);
            MakeCheckpoint(54f, 2f, false);
            MakeChaser(59f, 5.5f);
            MakeMoonStone(61f, 4.2f, 2);
            MakeVine(65.4f, 6f, 5);
            MakeHint(52, 66,
                "La enredadera está muy alta. Sube a la rama de luna, salta hacia ella y cambia al día en el aire para que crezca.",
                "Cuidado con el guardián: te persigue de noche. Desde mi rama, salta hacia la enredadera y pulsa Q en el aire.");

            // Repisa alta: pedazo del Intihuatana y puerta de piedra.
            MakeBlock(66, 104, 10f);
            MakeTree(70f, 10f, 0);
            MakeShadow(72f, 79f, 10f, 2f);
            var gate = MakeGate(84f, 10f, 5);
            MakePiece(77f, 11.1f, gate);
            MakeHint(67, 82,
                "¡El tercer pedazo del Intihuatana! Acércate y pulsa E para recogerlo.",
                "¡El tercer pedazo del Intihuatana! Acércate y pulsa E... y no dejes que la sombra te toque.");
            Trigger<LevelEnd>("Fin del nivel", new Vector2(90f, 12f), new Vector2(2f, 6f));

            SpawnPlayer(gm, new Vector2(-3f, 0.1f), 88f, 13f);
        }

        void SpawnPlayer(GameManager gm, Vector2 start, float maxX, float maxY)
        {
            var player = MakePlayer(start);
            MakeLlama(player, start - new Vector2(1.5f, 0.1f));

            var follow = camT.gameObject.AddComponent<CameraFollow>();
            follow.target = player.transform;
            follow.minX = 0f;
            follow.maxX = maxX;
            follow.minY = 2f;
            follow.maxY = maxY;
            follow.SnapToTarget();

            gm.Player = player;
            gm.Cam = follow;
            info.player = player;
            info.cam = follow;
        }

        // ---------------- Piezas del nivel ----------------
        GameObject Node(string name, Vector3 pos, Transform parent = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent != null ? parent : world, false);
            go.transform.localPosition = pos;
            return go;
        }

        SpriteRenderer AddSprite(string name, Sprite sprite, int order, Transform parent, Vector3 pos)
        {
            var go = Node(name, pos, parent);
            return SpriteFactory.AddRenderer(go, sprite, order);
        }

        static void Tint(SpriteRenderer sr, Color day, Color night)
        {
            var t = sr.gameObject.AddComponent<DayNightTint>();
            t.day = day;
            t.night = night;
            sr.color = night;
        }

        void MakeGround(int x0, int x1, float top)
        {
            int w = x1 - x0;
            int h = Mathf.CeilToInt(top - KillY);
            var sr = AddSprite("Suelo", SpriteFactory.Ground(w, h, forest), 0, null, new Vector3((x0 + x1) / 2f, top, 0));
            Tint(sr, Color.white, SpriteFactory.NightTint);
            var box = sr.gameObject.AddComponent<BoxCollider2D>();
            box.size = new Vector2(w, h);
            box.offset = new Vector2(0, -h / 2f);
        }

        void Wall(float x)
        {
            var go = Node("Límite", new Vector3(x, 5f, 0));
            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(1f, 30f);
        }

        void MakeBlock(int x0, int x1, float top)
        {
            int w = x1 - x0;
            int h = Mathf.CeilToInt(top - KillY);
            var sr = AddSprite("Repisa de piedra", SpriteFactory.StoneBlock(w, h), 0, null, new Vector3((x0 + x1) / 2f, top, 0));
            Tint(sr, Color.white, SpriteFactory.NightTint);
            var box = sr.gameObject.AddComponent<BoxCollider2D>();
            box.size = new Vector2(w, h);
            box.offset = new Vector2(0, -h / 2f);
        }

        void MakeDecor(Sprite sprite, Vector2 pos, int order)
        {
            var sr = AddSprite(sprite.name, sprite, order, null, pos);
            Tint(sr, Color.white, SpriteFactory.NightTint);
        }

        void MakeStream(int x0, int x1)
        {
            int w = x1 - x0;
            var sr = AddSprite("Río", SpriteFactory.Water(w, 2, false), -2, null, new Vector3((x0 + x1) / 2f, -5.2f, 0));
            Tint(sr, Color.white, SpriteFactory.NightTint);
            var box = sr.gameObject.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(w, 1.6f);
            box.offset = new Vector2(0, -1f);
            sr.gameObject.AddComponent<Hazard>();
        }

        void MakeFlower(float x, float top, int stem)
        {
            var root = Node("Flor de kantuta", new Vector3(x, top, 0));
            var box = root.AddComponent<BoxCollider2D>();
            box.size = new Vector2(2f, 0.35f);
            box.offset = new Vector2(0f, -0.18f);
            box.usedByEffector = true;
            var effector = root.AddComponent<PlatformEffector2D>();
            effector.useOneWay = true;
            effector.surfaceArc = 160f;

            var visual = Node("Visual", Vector3.zero, root.transform);
            var sr = SpriteFactory.AddRenderer(visual, SpriteFactory.Flower(stem, true), -1);
            Tint(sr, Color.white, SpriteFactory.NightTint);

            var flower = root.AddComponent<FlowerPlatform>();
            flower.sr = sr;
            flower.top = box;
            flower.Init(stem);
        }

        void MakeMoonStone(float x, float top, int w)
        {
            var root = Node("Piedra de luna", new Vector3(x, top, 0));
            var sprite = forest ? SpriteFactory.MoonBranch(w) : SpriteFactory.MoonStone(w);
            var sr = SpriteFactory.AddRenderer(root, sprite, 1);
            var box = root.AddComponent<BoxCollider2D>();
            box.size = new Vector2(w, 0.5f);
            box.offset = new Vector2(0f, -0.25f);

            var glowGo = Node("Brillo", new Vector3(0f, -0.25f, 0f), root.transform);
            glowGo.transform.localScale = new Vector3(w * 1.6f, 1.4f, 1f);
            var glow = SpriteFactory.AddRenderer(glowGo, SpriteFactory.Glow(), 0);

            var ms = root.AddComponent<MoonStone>();
            ms.sr = sr;
            ms.col = box;
            ms.glow = glow;
        }

        void MakeShadow(float minX, float maxX, float groundY, float speed = 1.8f)
        {
            var go = Node("Sombra del Supay", new Vector3((minX + maxX) / 2f, groundY, 0));
            var sr = SpriteFactory.AddRenderer(go, SpriteFactory.ShadowSprite(), 8);
            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(0.55f, 0.5f);
            box.offset = new Vector2(0f, 0.3f);
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            var e = go.AddComponent<Enemy>();
            e.kind = Enemy.Kind.Shadow;
            e.minX = minX;
            e.maxX = maxX;
            e.speed = speed;
            e.sr = sr;
            e.col = box;
        }

        void MakeSpirit(float minX, float maxX, float y)
        {
            var go = Node("Espíritu del Supay", new Vector3(minX, y, 0));
            var sr = SpriteFactory.AddRenderer(go, SpriteFactory.SpiritSprite(), 8);
            var circle = go.AddComponent<CircleCollider2D>();
            circle.isTrigger = true;
            circle.radius = 0.3f;
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            var e = go.AddComponent<Enemy>();
            e.kind = Enemy.Kind.Spirit;
            e.minX = minX;
            e.maxX = maxX;
            e.speed = 2.2f;
            e.waveAmplitude = 1f;
            e.waveSpeed = 2.4f;
            e.sr = sr;
            e.col = circle;
        }

        void MakeTree(float x, float y, int variant)
        {
            var sr = AddSprite("Árbol", SpriteFactory.Tree(variant), -7, null, new Vector3(x, y, 0));
            Tint(sr, Color.white, SpriteFactory.NightTint);
        }

        void MakeVine(float x, float baseY, int heightUnits)
        {
            var root = Node("Enredadera", new Vector3(x, baseY, 0));
            var zone = root.AddComponent<BoxCollider2D>();
            zone.isTrigger = true;
            zone.size = new Vector2(0.9f, heightUnits);
            zone.offset = new Vector2(0f, heightUnits / 2f);
            var visual = Node("Visual", Vector3.zero, root.transform);
            var sr = SpriteFactory.AddRenderer(visual, SpriteFactory.Vine(heightUnits, true), 3);
            Tint(sr, Color.white, SpriteFactory.NightTint);
            var vine = root.AddComponent<Vine>();
            vine.sr = sr;
            vine.zone = zone;
            vine.Init(heightUnits);
        }

        void MakeChaser(float x, float y)
        {
            var go = Node("Guardián del bosque", new Vector3(x, y, 0));
            var sr = SpriteFactory.AddRenderer(go, SpriteFactory.ChaserSprite(), 8);
            var circle = go.AddComponent<CircleCollider2D>();
            circle.isTrigger = true;
            circle.radius = 0.32f;
            var rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            var e = go.AddComponent<Enemy>();
            e.kind = Enemy.Kind.Chaser;
            e.speed = 2.6f;
            e.chaseRadius = 7f;
            e.sr = sr;
            e.col = circle;
        }

        void MakeCheckpoint(float x, float y, bool lit)
        {
            var go = Node("Apacheta", new Vector3(x, y, 0));
            var sr = SpriteFactory.AddRenderer(go, SpriteFactory.Apacheta(lit), -3);
            Tint(sr, Color.white, SpriteFactory.NightTint);
            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(1f, 3f);
            box.offset = new Vector2(0f, 1.5f);
            var cp = go.AddComponent<Checkpoint>();
            cp.sr = sr;
        }

        void MakeHint(float x0, float x1, string dayText, string nightText)
        {
            var h = Trigger<HintTrigger>("Pista", new Vector2((x0 + x1) / 2f, 3f), new Vector2(x1 - x0, 10f));
            h.dayText = dayText;
            h.nightText = nightText;
        }

        T Trigger<T>(string name, Vector2 center, Vector2 size) where T : Component
        {
            var go = Node(name, center);
            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = size;
            return go.AddComponent<T>();
        }

        Gate MakeGate(float x, float baseY, int h)
        {
            var go = Node("Puerta de piedra", new Vector3(x, baseY, 0));
            var sr = SpriteFactory.AddRenderer(go, SpriteFactory.Gate(h), 2);
            Tint(sr, Color.white, SpriteFactory.NightTint);
            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(1.5f, h);
            box.offset = new Vector2(0f, h / 2f);
            var gate = go.AddComponent<Gate>();
            gate.col = box;
            gate.riseHeight = h + 0.5f;
            return gate;
        }

        void MakePiece(float x, float y, Gate gate)
        {
            var root = Node("Pedazo del Intihuatana", new Vector3(x, y, 0));
            var visual = Node("Visual", Vector3.zero, root.transform);
            var glowGo = Node("Brillo", Vector3.zero, visual.transform);
            glowGo.transform.localScale = new Vector3(2.2f, 2.2f, 1f);
            var glow = SpriteFactory.AddRenderer(glowGo, SpriteFactory.Glow(), 4);
            glow.color = new Color(1f, 0.85f, 0.35f, 0.9f);
            SpriteFactory.AddRenderer(visual, SpriteFactory.StoneSprite(), 5);
            var piece = root.AddComponent<StonePiece>();
            piece.visual = visual.transform;
            piece.gate = gate;
        }

        PlayerController MakePlayer(Vector2 pos)
        {
            var go = Node("Sisa", pos);
            var rb = go.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.gravityScale = 3.2f;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

            var col = go.AddComponent<CapsuleCollider2D>();
            col.size = new Vector2(0.55f, 1f);
            col.offset = new Vector2(0f, 0.5f);
            if (SpriteFactory.Baker == null)
                col.sharedMaterial = new PhysicsMaterial2D("SinFriccion") { friction = 0f, bounciness = 0f };

            var visual = Node("Visual", Vector3.zero, go.transform);
            var sr = SpriteFactory.AddRenderer(visual, SpriteFactory.SisaIdleSprite(), 10);
            Tint(sr, Color.white, new Color(0.8f, 0.85f, 1f));

            var player = go.AddComponent<PlayerController>();
            player.Init(sr);
            player.killY = KillY;
            player.RespawnPoint = pos;
            return player;
        }

        void MakeLlama(PlayerController player, Vector2 pos)
        {
            var go = Node("Llama", pos);
            var sr = SpriteFactory.AddRenderer(go, SpriteFactory.LlamaSprite(), 9);
            Tint(sr, Color.white, new Color(0.8f, 0.85f, 1f));
            var llama = go.AddComponent<LlamaFollower>();
            llama.player = player;
            llama.sr = sr;
        }
    }
}
