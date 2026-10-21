using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace IntiYKilla
{
    // Arte provisional del prototipo: todos los sprites se dibujan por código
    // (pixel art en mapas de caracteres y formas simples). Así el proyecto no
    // depende de imágenes externas y el equipo puede reemplazarlos luego por el arte final.
    public static class SpriteFactory
    {
        public const int PPU = 16;

        static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

        // Lo usa el editor al generar las escenas editables: guarda cada sprite como
        // imagen PNG dentro del proyecto y devuelve el sprite importado.
        public static System.Func<string, Texture2D, float, Vector2, Sprite> Baker;

        public static void ClearCache() => cache.Clear();
        static Material spriteMaterial;
        static bool materialResolved;

        // ---------- Paleta general ----------
        public static readonly Color DayTint = Color.white;
        public static readonly Color NightTint = new Color(0.55f, 0.62f, 0.95f);

        static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }

        static readonly Dictionary<char, Color> Palette = new Dictionary<char, Color>
        {
            { 'k', Hex("#2b1b17") }, // contorno oscuro
            { 'h', Hex("#7a4a2a") }, // sombrero
            { 'r', Hex("#c0392b") }, // rojo
            { 'b', Hex("#1d1414") }, // cabello / ojos
            { 's', Hex("#c98a5e") }, // piel
            { 'e', Hex("#1d1414") }, // ojos
            { 'p', Hex("#d63a6a") }, // lliclla magenta
            { 'y', Hex("#f2c14e") }, // amarillo
            { 'g', Hex("#3a9d5d") }, // verde
            { 'u', Hex("#2e4a8f") }, // pollera azul
            { 'w', Hex("#f4efe6") }, // blanco / lana
            { 'f', Hex("#5a3a22") }, // sandalias
            { 'c', Hex("#cdb894") }, // lana sombreada
            { 'n', Hex("#e78fa8") }, // hocico rosado
        };

        static readonly Dictionary<char, Color> SupayPalette = new Dictionary<char, Color>
        {
            { 'k', Hex("#120a14") },
            { 'd', Hex("#3b1f4a") },
            { 'm', Hex("#5e2d6e") },
            { 'r', Hex("#e0362b") },
            { 'y', Hex("#ffd23f") },
            { 'w', Hex("#f4efe6") },
            { 'g', Hex("#2f8f5b") },
        };

        static readonly Dictionary<char, Color> StonePalette = new Dictionary<char, Color>
        {
            { 'y', Hex("#f2c14e") },
            { 'g', Hex("#9a948c") },
            { 'l', Hex("#cfc8bd") },
            { 'd', Hex("#6b655e") },
        };

        // ---------- Mapas de pixeles (fila superior primero) ----------
        static readonly string[] SisaIdle =
        {
            "....hhhh....",
            "...hhhhhh...",
            "...rrrrrr...",
            ".hhhhhhhhhh.",
            "...bssssb...",
            "...besseb...",
            "...bssssb...",
            "..bppppppb..",
            ".bpyyggyypb.",
            ".spppppppps.",
            "..pygpygpp..",
            "...uuuuuu...",
            "..uuuuuuuu..",
            "..uyuuuuyu..",
            ".uuuuuuuuuu.",
            "...ss..ss...",
            "...ff..ff...",
        };

        static readonly string[] SisaWalk =
        {
            "....hhhh....",
            "...hhhhhh...",
            "...rrrrrr...",
            ".hhhhhhhhhh.",
            "...bssssb...",
            "...besseb...",
            "...bssssb...",
            "..bppppppb..",
            ".bpyyggyypb.",
            ".spppppppps.",
            "..pygpygpp..",
            "...uuuuuu...",
            "..uuuuuuuu..",
            "..uyuuuuyu..",
            ".uuuuuuuuuu.",
            "..ss....ss..",
            "..ff....ff..",
        };

        static readonly string[] Llama =
        {
            "........r..r",
            "........wwww",
            "........wkww",
            "........wwwn",
            "........www.",
            "........ww..",
            ".wwwwwwwww..",
            ".wryyrryrw..",
            ".wwwwwwwww..",
            ".cwwwwwwwc..",
            ".w.w...w.w..",
            ".k.k...k.k..",
        };

        static readonly string[] Supay =
        {
            "k..............k",
            "kk............kk",
            ".kk..........kk.",
            "..kk.dddddd.kk..",
            "...kddddddddk...",
            "...dmmmmmmmmd...",
            "..dmyyymmyyymd..",
            "..dmyrymmyrymd..",
            "..dmyyymmyyymd..",
            "..ddmmmrrmmmdd..",
            "..dmrwrwrwrwmd..",
            "...dmrrrrrrmd...",
            "....dddddddd....",
            "..ddmmmmmmmmdd..",
            ".ddmmgmmmmgmmdd.",
            "ddmmmmmmmmmmmmdd",
            "dmmmmmmmmmmmmmmd",
            "d.d.d.d..d.d.d.d",
        };

        static readonly string[] StonePiece =
        {
            "...yy...",
            "..ygly..",
            ".ygllgy.",
            "yggllggy",
            "ygggggdy",
            ".ygggdy.",
            "..ygdy..",
            "...yy...",
        };

        // Sombra del Supay: enemigo que camina de noche.
        static readonly string[] Shadow =
        {
            "....kkkk....",
            "...kkkkkk...",
            "..kkrkkrkk..",
            "..kkkkkkkk..",
            ".kkkkkkkkkk.",
            ".kkkkkkkkkk.",
            "kkkkkkkkkkkk",
            "kkkkkkkkkkkk",
            "kk.kkkkkk.kk",
            "k..kk..kk..k",
        };

        // Espíritu de la laguna: enemigo que vuela de noche.
        static readonly string[] Spirit =
        {
            "...cccccc...",
            "..cwwwwwwc..",
            ".cwwbwwbwwc.",
            ".cwwbwwbwwc.",
            ".cwwwwwwwwc.",
            ".cwwwbbwwwc.",
            ".cwwwwwwwwc.",
            ".cwwwwwwwwc.",
            "..cwwccwwc..",
            "...cw..wc...",
            "....c...c...",
        };

        // Guardián del bosque: espíritu con un solo ojo que persigue a Sisa de noche.
        static readonly string[] Chaser =
        {
            "....kkkkk...",
            "..kkmmmmmkk.",
            ".kmmmmmmmmmk",
            ".kmmwwwwmmmk",
            "kmmwwyywwmmk",
            "kmmwyrrywmmk",
            "kmmwwyywwmmk",
            ".kmmwwwwmmk.",
            ".kmmmmmmmmk.",
            "..kmmkkmmk..",
            "..km.km.km..",
            "...k..k..k..",
        };

        static readonly Dictionary<char, Color> ChaserPalette = new Dictionary<char, Color>
        {
            { 'k', new Color(0.12f, 0.04f, 0.1f, 0.95f) },
            { 'm', new Color(0.55f, 0.15f, 0.35f, 0.9f) },
            { 'w', Hex("#f4efe6") },
            { 'y', Hex("#ffd23f") },
            { 'r', Hex("#e0362b") },
        };

        static readonly Dictionary<char, Color> ShadowPalette = new Dictionary<char, Color>
        {
            { 'k', new Color(0.08f, 0.03f, 0.12f, 0.92f) },
            { 'r', Hex("#ff4d3d") },
        };

        static readonly Dictionary<char, Color> SpiritPalette = new Dictionary<char, Color>
        {
            { 'c', new Color(0.55f, 0.85f, 1f, 0.75f) },
            { 'w', new Color(0.85f, 0.96f, 1f, 0.85f) },
            { 'b', Hex("#1b2a4a") },
        };

        // ---------- API pública ----------
        public static Sprite SisaIdleSprite() => FromMap("sisa_idle", SisaIdle, Palette, PPU, new Vector2(0.5f, 0f));
        public static Sprite SisaWalkSprite() => FromMap("sisa_walk", SisaWalk, Palette, PPU, new Vector2(0.5f, 0f));
        public static Sprite LlamaSprite() => FromMap("llama", Llama, Palette, PPU, new Vector2(0.5f, 0f));
        public static Sprite SupaySprite() => FromMap("supay", Supay, SupayPalette, 6, new Vector2(0.5f, 0f));
        public static Sprite StoneSprite() => FromMap("stone", StonePiece, StonePalette, 10, new Vector2(0.5f, 0.5f));
        public static Sprite ShadowSprite() => FromMap("shadow", Shadow, ShadowPalette, PPU, new Vector2(0.5f, 0f));
        public static Sprite ChaserSprite() => FromMap("chaser", Chaser, ChaserPalette, PPU, new Vector2(0.5f, 0.5f));
        public static Sprite SpiritSprite() => FromMap("spirit", Spirit, SpiritPalette, PPU, new Vector2(0.5f, 0.5f));

        public static SpriteRenderer AddRenderer(GameObject go, Sprite sprite, int order)
        {
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = order;
            if (Baker == null)
            {
                var mat = SpriteMaterial();
                if (mat != null) sr.sharedMaterial = mat;
            }
            return sr;
        }

        // En proyectos URP 2D los sprites "Lit" se ven negros si no hay luces 2D.
        // Usamos el material sin iluminación del pipeline activo cuando existe.
        static Material SpriteMaterial()
        {
            if (materialResolved) return spriteMaterial;
            materialResolved = true;
            Shader shader = null;
            if (GraphicsSettings.currentRenderPipeline != null)
                shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader != null) spriteMaterial = new Material(shader);
            return spriteMaterial;
        }

        static Texture2D NewTexture(int w, int h)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            var clear = new Color[w * h];
            tex.SetPixels(clear);
            return tex;
        }

        static Sprite Make(string key, Texture2D tex, float ppu, Vector2 pivot)
        {
            tex.Apply();
            if (Baker != null)
            {
                var baked = Baker(key, tex, ppu, pivot);
                if (baked != null)
                {
                    cache[key] = baked;
                    return baked;
                }
            }
            var s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, ppu, 0, SpriteMeshType.FullRect);
            s.name = key;
            cache[key] = s;
            return s;
        }

        static Sprite FromMap(string key, string[] rows, Dictionary<char, Color> pal, float ppu, Vector2 pivot)
        {
            if (cache.TryGetValue(key, out var cached)) return cached;
            int h = rows.Length;
            int w = rows[0].Length;
            var tex = NewTexture(w, h);
            for (int y = 0; y < h; y++)
            {
                string row = rows[h - 1 - y];
                for (int x = 0; x < w && x < row.Length; x++)
                    if (pal.TryGetValue(row[x], out var c)) tex.SetPixel(x, y, c);
            }
            return Make(key, tex, ppu, pivot);
        }

        static void FillRect(Texture2D t, int x0, int y0, int w, int h, Color c)
        {
            for (int y = Mathf.Max(0, y0); y < Mathf.Min(t.height, y0 + h); y++)
                for (int x = Mathf.Max(0, x0); x < Mathf.Min(t.width, x0 + w); x++)
                    t.SetPixel(x, y, c);
        }

        static void Disc(Texture2D t, float cx, float cy, float r, Color c)
        {
            for (int y = 0; y < t.height; y++)
                for (int x = 0; x < t.width; x++)
                {
                    float dx = x + 0.5f - cx, dy = y + 0.5f - cy;
                    if (dx * dx + dy * dy <= r * r) t.SetPixel(x, y, c);
                }
        }

        // Bloque de tierra con pasto (Pueblo / orillas). Tamaño en unidades.
        public static Sprite Ground(int wUnits, int hUnits, bool forest = false)
        {
            string key = $"ground_{wUnits}x{hUnits}_{forest}";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = wUnits * PPU, h = hUnits * PPU;
            var tex = NewTexture(w, h);
            var rnd = new System.Random(wUnits * 73 + hUnits);
            Color dirt = Hex("#8a5a3b"), dirtDark = Hex("#6e4630"), stone = Hex("#a3765a");
            Color grass = Hex("#5fae4e"), grassDark = Hex("#3e8a3a");
            if (forest)
            {
                dirt = Hex("#5e3f2c"); dirtDark = Hex("#4a3022"); stone = Hex("#6f7a4a");
                grass = Hex("#2f7a3e"); grassDark = Hex("#1f5c30");
            }
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int fromTop = h - 1 - y;
                    Color c = ((x / 4 + y / 4) % 7 == 0) ? dirtDark : dirt;
                    if (rnd.NextDouble() < 0.03) c = stone;
                    if (fromTop < 3) c = grass;
                    else if (fromTop < 5) c = (x % 3 == 0) ? grass : grassDark;
                    tex.SetPixel(x, y, c);
                }
            return Make(key, tex, PPU, new Vector2(0.5f, 1f));
        }

        // Piedra tallada inca (repisas y bloques altos).
        public static Sprite StoneBlock(int wUnits, int hUnits)
        {
            string key = $"stoneblock_{wUnits}x{hUnits}";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = wUnits * PPU, h = hUnits * PPU;
            var tex = NewTexture(w, h);
            Color baseC = Hex("#9c8f7d"), dark = Hex("#6f6456"), light = Hex("#c2b6a3"), moss = Hex("#5fae4e");
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int row = y / 8;
                    int off = (row % 2) * 8;
                    bool mortar = (y % 8 == 0) || ((x + off) % 16 == 0);
                    Color c = mortar ? dark : baseC;
                    if (!mortar && (y % 8 == 7)) c = light;
                    if (h - 1 - y < 2) c = moss;
                    tex.SetPixel(x, y, c);
                }
            return Make(key, tex, PPU, new Vector2(0.5f, 1f));
        }

        // Agua de la laguna (de día) e hielo (de noche).
        public static Sprite Water(int wUnits, int hUnits, bool frozen)
        {
            string key = $"water_{wUnits}x{hUnits}_{frozen}";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = wUnits * PPU, h = hUnits * PPU;
            var tex = NewTexture(w, h);
            Color deep = Hex("#1f4f8f"), mid = Hex("#2f74b5"), crest = Hex("#8fd3ff");
            Color ice = Hex("#d9f3ff"), iceShade = Hex("#a9dcf2"), crack = Hex("#ffffff");
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    int fromTop = h - 1 - y;
                    Color c = Color.Lerp(mid, deep, Mathf.Clamp01(fromTop / (float)h * 1.5f));
                    if (!frozen)
                    {
                        int wave = (int)(Mathf.Sin(x * 0.4f) * 1.5f + 1.5f);
                        if (fromTop < wave) c = Color.clear;
                        else if (fromTop < wave + 2) c = crest;
                        c.a = c.a > 0 ? 0.92f : 0f;
                    }
                    else if (fromTop < 6)
                    {
                        c = (fromTop < 2) ? ice : iceShade;
                        if ((x * 7 + fromTop * 3) % 23 == 0) c = crack;
                    }
                    tex.SetPixel(x, y, c);
                }
            return Make(key, tex, PPU, new Vector2(0.5f, 1f));
        }

        // Kantuta: flor andina. Abierta sirve de plataforma, cerrada es un capullo.
        public static Sprite Flower(int stemUnits, bool open)
        {
            string key = $"flower_{stemUnits}_{open}";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = 2 * PPU, h = stemUnits * PPU + 8;
            var tex = NewTexture(w, h);
            Color stem = Hex("#3e8a3a"), leaf = Hex("#5fae4e");
            Color petal = Hex("#e0365a"), petalDark = Hex("#a81e45"), center = Hex("#f2c14e");
            FillRect(tex, w / 2 - 1, 0, 3, h - 6, stem);
            for (int i = 1; i < stemUnits; i++)
            {
                int ly = i * PPU - 4;
                int dir = (i % 2 == 0) ? 1 : -1;
                FillRect(tex, w / 2 + (dir > 0 ? 2 : -7), ly, 6, 3, leaf);
            }
            if (open)
            {
                int top = h - 1;
                for (int x = 0; x < w; x++)
                {
                    int depth = 4 - Mathf.Abs(x - w / 2) / 5;
                    for (int d = 0; d < Mathf.Max(2, depth + 3); d++)
                        tex.SetPixel(x, top - d, (x / 4) % 2 == 0 ? petal : petalDark);
                }
                FillRect(tex, w / 2 - 3, top - 2, 7, 3, center);
            }
            else
            {
                int top = h - 1;
                FillRect(tex, w / 2 - 3, top - 9, 7, 9, petalDark);
                FillRect(tex, w / 2 - 2, top - 8, 5, 7, petal);
            }
            return Make(key, tex, PPU, new Vector2(0.5f, 1f));
        }

        // Casa de adobe con techo de paja (decoración del Pueblo).
        public static Sprite House()
        {
            const string key = "house";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = 64, h = 52;
            var tex = NewTexture(w, h);
            Color adobe = Hex("#c9925f"), adobeDark = Hex("#a8744a"), roof = Hex("#d8b45a"), roofDark = Hex("#a8873a");
            Color door = Hex("#5a3a22"), win = Hex("#3a2a1a");
            FillRect(tex, 6, 0, 52, 32, adobe);
            for (int y = 4; y < 32; y += 6) FillRect(tex, 6, y, 52, 1, adobeDark);
            FillRect(tex, 27, 0, 11, 18, door);
            FillRect(tex, 12, 14, 8, 7, win);
            FillRect(tex, 45, 14, 8, 7, win);
            for (int y = 32; y < h; y++)
            {
                int inset = (y - 32);
                for (int x = inset; x < w - inset; x++)
                    tex.SetPixel(x, y, ((x + y) % 4 == 0) ? roofDark : roof);
            }
            return Make(key, tex, PPU, new Vector2(0.5f, 0f));
        }

        // Apacheta: montículo de piedras que sirve de punto de control.
        public static Sprite Apacheta(bool lit)
        {
            string key = "apacheta_" + lit;
            if (cache.TryGetValue(key, out var cached)) return cached;
            var tex = NewTexture(16, 20);
            Color s1 = Hex("#8d8478"), s2 = Hex("#b0a696"), flag = lit ? Hex("#f2c14e") : Hex("#7a7a7a");
            FillRect(tex, 1, 0, 14, 4, s1);
            FillRect(tex, 3, 4, 10, 3, s2);
            FillRect(tex, 4, 7, 8, 3, s1);
            FillRect(tex, 6, 10, 4, 2, s2);
            FillRect(tex, 7, 12, 1, 8, Hex("#5a3a22"));
            FillRect(tex, 8, 15, 6, 4, flag);
            return Make(key, tex, PPU, new Vector2(0.5f, 0f));
        }

        // Puerta de piedra inca con el símbolo del sol.
        public static Sprite Gate(int hUnits)
        {
            string key = "gate_" + hUnits;
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = 24, h = hUnits * PPU;
            var tex = NewTexture(w, h);
            Color st = Hex("#7d7264"), dk = Hex("#5b5248"), gold = Hex("#f2c14e");
            FillRect(tex, 0, 0, w, h, st);
            for (int y = 0; y < h; y += 10) FillRect(tex, 0, y, w, 1, dk);
            Disc(tex, w / 2f, h * 0.6f, 6f, gold);
            Disc(tex, w / 2f, h * 0.6f, 3.5f, st);
            Disc(tex, w / 2f, h * 0.6f, 2f, gold);
            return Make(key, tex, PPU, new Vector2(0.5f, 0f));
        }

        // Inti: sol con rostro.
        public static Sprite Sun()
        {
            const string key = "sun";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int n = 48;
            var tex = NewTexture(n, n);
            Color ray = Hex("#ffb627"), body = Hex("#ffd23f"), face = Hex("#c27a12");
            for (int i = 0; i < 12; i++)
            {
                float a = i * Mathf.PI / 6f;
                for (float r = 14; r < 23; r += 0.5f)
                    tex.SetPixel((int)(n / 2f + Mathf.Cos(a) * r), (int)(n / 2f + Mathf.Sin(a) * r), ray);
            }
            Disc(tex, n / 2f, n / 2f, 13f, body);
            FillRect(tex, 18, 26, 3, 3, face);
            FillRect(tex, 27, 26, 3, 3, face);
            FillRect(tex, 19, 18, 10, 2, face);
            return Make(key, tex, PPU, new Vector2(0.5f, 0.5f));
        }

        // Killa: luna creciente con rostro.
        public static Sprite Moon()
        {
            const string key = "moon";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int n = 40;
            var tex = NewTexture(n, n);
            Color body = Hex("#e8ecf7"), face = Hex("#8a93b8");
            Disc(tex, n / 2f, n / 2f, 15f, body);
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float dx = x + 0.5f - (n / 2f + 8), dy = y + 0.5f - (n / 2f + 4);
                    if (dx * dx + dy * dy <= 13f * 13f) tex.SetPixel(x, y, Color.clear);
                }
            FillRect(tex, 11, 22, 3, 2, face);
            FillRect(tex, 10, 14, 4, 1, face);
            return Make(key, tex, PPU, new Vector2(0.5f, 0.5f));
        }

        // Cielo en degradado vertical.
        public static Sprite Sky(bool night)
        {
            string key = "sky_" + night;
            if (cache.TryGetValue(key, out var cached)) return cached;
            var tex = NewTexture(4, 64);
            tex.filterMode = FilterMode.Bilinear;
            Color top = night ? Hex("#0b1030") : Hex("#5aa8e6");
            Color bottom = night ? Hex("#3a2a5e") : Hex("#ffd9a0");
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < 4; x++)
                    tex.SetPixel(x, y, Color.Lerp(bottom, top, y / 63f));
            return Make(key, tex, 4, new Vector2(0.5f, 0.5f));
        }

        public static Sprite Stars()
        {
            const string key = "stars";
            if (cache.TryGetValue(key, out var cached)) return cached;
            var tex = NewTexture(160, 90);
            var rnd = new System.Random(7);
            for (int i = 0; i < 120; i++)
            {
                int x = rnd.Next(160), y = rnd.Next(25, 90);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, (float)(0.4 + rnd.NextDouble() * 0.6)));
            }
            return Make(key, tex, 8, new Vector2(0.5f, 0.5f));
        }

        // Cordillera de los Andes con nieve en las cumbres.
        public static Sprite Mountains(int wUnits, int hUnits, int seed, Color rock, Color snow, float snowLine)
        {
            string key = $"mountains_{seed}";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int ppu = 4;
            int w = wUnits * ppu, h = hUnits * ppu;
            var tex = NewTexture(w, h);
            float p1 = 0.031f + seed * 0.004f, p2 = 0.083f + seed * 0.01f, p3 = 0.17f;
            for (int x = 0; x < w; x++)
            {
                float ridge = 0.55f + 0.25f * Mathf.Sin(x * p1 + seed) + 0.12f * Mathf.Sin(x * p2 + seed * 2f) + 0.05f * Mathf.Sin(x * p3);
                int top = Mathf.Clamp((int)(ridge * h), 2, h - 1);
                for (int y = 0; y < top; y++)
                {
                    Color c = rock;
                    if (y > h * snowLine && y > top - 6) c = snow;
                    if ((x + y) % 9 == 0) c = Color.Lerp(c, Color.black, 0.08f);
                    tex.SetPixel(x, y, c);
                }
            }
            return Make(key, tex, ppu, new Vector2(0.5f, 0f));
        }

        // Entrada de la cueva del Supay.
        public static Sprite Cave()
        {
            const string key = "cave";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = 160, h = 112;
            var tex = NewTexture(w, h);
            Color rock = Hex("#4a3b52"), rock2 = Hex("#5c4a66"), hole = Hex("#0a060c");
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    float nx = (x - w / 2f) / (w / 2f), ny = y / (float)h;
                    if (nx * nx + ny * ny * 0.9f < 1f)
                        tex.SetPixel(x, y, ((x / 6 + y / 5) % 3 == 0) ? rock2 : rock);
                    float hx = (x - w / 2f) / (w * 0.28f), hy = y / (h * 0.62f);
                    if (hx * hx + hy * hy < 1f) tex.SetPixel(x, y, hole);
                }
            return Make(key, tex, PPU, new Vector2(0.5f, 0f));
        }

        // Piedra de luna: camino oculto que Killa revela de noche.
        public static Sprite MoonStone(int wUnits)
        {
            string key = "moonstone_" + wUnits;
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = wUnits * PPU, h = 8;
            var tex = NewTexture(w, h);
            Color body = Hex("#b9c8ff"), edge = Hex("#ffffff"), dark = Hex("#7b8fd6");
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    bool corner = (y < 2 || y > h - 2) && (x < 2 || x > w - 3);
                    if (corner) continue;
                    Color c = y >= h - 2 ? edge : (y < 2 ? dark : body);
                    if ((x * 5 + y * 3) % 17 == 0) c = edge;
                    tex.SetPixel(x, y, c);
                }
            return Make(key, tex, PPU, new Vector2(0.5f, 1f));
        }

        // Nube redondeada en pixel art.
        public static Sprite Cloud(int variant)
        {
            string key = "cloud_" + variant;
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = 48, h = 20;
            var tex = NewTexture(w, h);
            Color c = Color.white, shade = Hex("#dfe8f5");
            float[,] puffs = variant == 0
                ? new float[,] { { 12, 7, 7 }, { 23, 10, 9 }, { 35, 7, 7 } }
                : new float[,] { { 10, 6, 6 }, { 20, 9, 8 }, { 30, 10, 8 }, { 39, 6, 6 } };
            for (int i = 0; i < puffs.GetLength(0); i++) Disc(tex, puffs[i, 0], puffs[i, 1], puffs[i, 2], c);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < 4; y++)
                    if (tex.GetPixel(x, y).a > 0f) tex.SetPixel(x, y, shade);
            return Make(key, tex, PPU, new Vector2(0.5f, 0.5f));
        }

        // Rama de luna: tronco caído con musgo que brilla; solo existe de noche.
        public static Sprite MoonBranch(int wUnits)
        {
            string key = "moonbranch_" + wUnits;
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = wUnits * PPU, h = 9;
            var tex = NewTexture(w, h);
            Color bark = Hex("#6a5a8a"), barkDark = Hex("#4a3f66"), moss = Hex("#9fe3ff");
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if ((x == 0 || x == w - 1) && (y == 0 || y == h - 1)) continue;
                    Color c = (y + x / 5) % 4 == 0 ? barkDark : bark;
                    if (y >= h - 2) c = (x % 3 == 0) ? Color.white : moss;
                    tex.SetPixel(x, y, c);
                }
            return Make(key, tex, PPU, new Vector2(0.5f, 1f));
        }

        // Enredadera: crecida (de día) o seca (de noche).
        public static Sprite Vine(int hUnits, bool grown)
        {
            string key = $"vine_{hUnits}_{grown}";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = 16, h = hUnits * PPU;
            var tex = NewTexture(w, h);
            if (grown)
            {
                Color stem = Hex("#2f7a3e"), leaf = Hex("#6ccf5a"), leafDark = Hex("#3e9a40"), flower = Hex("#f2c14e");
                for (int y = 0; y < h; y++)
                {
                    int x = 7 + (int)(Mathf.Sin(y * 0.25f) * 2.5f);
                    FillRect(tex, x, y, 2, 1, stem);
                    if (y % 7 == 0)
                    {
                        int side = (y / 7) % 2 == 0 ? 1 : -1;
                        FillRect(tex, x + (side > 0 ? 2 : -4), y, 4, 3, (y / 7) % 3 == 0 ? leafDark : leaf);
                    }
                    if (y % 29 == 13) FillRect(tex, x - 1, y, 3, 3, flower);
                }
            }
            else
            {
                Color dry = Hex("#7a5a32"), dryDark = Hex("#5a4022");
                for (int y = 0; y < Mathf.Min(h, 14); y++)
                {
                    int x = 7 + (int)(Mathf.Sin(y * 0.6f) * 2f);
                    tex.SetPixel(x, y, y % 3 == 0 ? dryDark : dry);
                    tex.SetPixel(x + 1, y, dry);
                }
            }
            return Make(key, tex, PPU, new Vector2(0.5f, 0f));
        }

        // Árbol grande del bosque (decoración).
        public static Sprite Tree(int variant)
        {
            string key = "tree_" + variant;
            if (cache.TryGetValue(key, out var cached)) return cached;
            int w = 64, h = 120;
            var tex = NewTexture(w, h);
            Color trunk = Hex("#5a3a22"), trunkDark = Hex("#40281a");
            Color[] leaves = variant == 0
                ? new[] { Hex("#1f5c30"), Hex("#2f7a3e"), Hex("#3e9a40") }
                : new[] { Hex("#24563a"), Hex("#2e6e48"), Hex("#4a8a4a") };
            FillRect(tex, 26, 0, 12, 70, trunk);
            for (int y = 0; y < 70; y += 6) FillRect(tex, 28 + (y / 6) % 3 * 2, y, 2, 4, trunkDark);
            FillRect(tex, 20, 0, 6, 4, trunk);
            FillRect(tex, 38, 0, 6, 4, trunk);
            Disc(tex, 32, 82, 24, leaves[0]);
            Disc(tex, 18, 76, 15, leaves[1]);
            Disc(tex, 46, 78, 15, leaves[1]);
            Disc(tex, 32, 98, 18, leaves[1]);
            Disc(tex, 26, 102, 9, leaves[2]);
            Disc(tex, 42, 88, 8, leaves[2]);
            return Make(key, tex, PPU, new Vector2(0.5f, 0f));
        }

        // Silueta de bosque para el fondo con parallax.
        public static Sprite ForestLayer(int wUnits, int hUnits, int seed, Color color)
        {
            string key = "forestlayer_" + seed;
            if (cache.TryGetValue(key, out var cached)) return cached;
            int ppu = 4;
            int w = wUnits * ppu, h = hUnits * ppu;
            var tex = NewTexture(w, h);
            var rnd = new System.Random(seed);
            FillRect(tex, 0, 0, w, h / 3, color);
            for (int x = 0; x < w; x += 5 + rnd.Next(6))
            {
                int th = h / 3 + rnd.Next(h / 2);
                int r = 4 + rnd.Next(6);
                if (rnd.Next(2) == 0)
                {
                    // Pino triangular
                    for (int y = 0; y < th; y++)
                    {
                        int half = (int)(r * (1f - y / (float)th) * 1.4f);
                        FillRect(tex, x - half, y, half * 2 + 1, 1, color);
                    }
                }
                else
                {
                    FillRect(tex, x - 1, 0, 3, th - r, color);
                    Disc(tex, x, th - r, r, color);
                }
            }
            return Make(key, tex, ppu, new Vector2(0.5f, 0f));
        }

        // Totora: juncos de la laguna (decoración).
        public static Sprite Totora()
        {
            const string key = "totora";
            if (cache.TryGetValue(key, out var cached)) return cached;
            var tex = NewTexture(20, 36);
            Color a = Hex("#8fae4a"), b = Hex("#6e8f3a"), tip = Hex("#c9a45a");
            var rnd = new System.Random(5);
            for (int i = 0; i < 9; i++)
            {
                int x = 1 + i * 2;
                int top = 18 + rnd.Next(17);
                for (int y = 0; y < top; y++) tex.SetPixel(x + (y > top * 0.7f ? (i % 2 == 0 ? 1 : -1) : 0), y, i % 2 == 0 ? a : b);
                FillRect(tex, x - 1, top - 4, 2, 4, tip);
            }
            return Make(key, tex, PPU, new Vector2(0.5f, 0f));
        }

        // Piedra brillante que Sisa lanza con la huaraca.
        public static Sprite SlingStoneSprite()
        {
            const string key = "slingstone";
            if (cache.TryGetValue(key, out var cached)) return cached;
            var tex = NewTexture(6, 6);
            Disc(tex, 3f, 3f, 3f, Hex("#f2c14e"));
            Disc(tex, 2.5f, 3.5f, 1.4f, Hex("#fff3c0"));
            return Make(key, tex, PPU, new Vector2(0.5f, 0.5f));
        }

        public static Sprite Glow()
        {
            const string key = "glow";
            if (cache.TryGetValue(key, out var cached)) return cached;
            int n = 32;
            var tex = NewTexture(n, n);
            tex.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(n / 2f, n / 2f)) / (n / 2f);
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(1f - d) * 0.6f));
                }
            return Make(key, tex, 16, new Vector2(0.5f, 0.5f));
        }

        // Pasto ichu (decoración).
        public static Sprite Ichu()
        {
            const string key = "ichu";
            if (cache.TryGetValue(key, out var cached)) return cached;
            var tex = NewTexture(12, 10);
            Color a = Hex("#d8b45a"), b = Hex("#b8943a");
            for (int i = 0; i < 6; i++)
            {
                int x0 = 1 + i * 2;
                int lean = i < 3 ? -1 : 1;
                for (int y = 0; y < 10 - Mathf.Abs(i - 2.5f) * 2; y++)
                    tex.SetPixel(Mathf.Clamp(x0 + lean * (y / 4), 0, 11), y, i % 2 == 0 ? a : b);
            }
            return Make(key, tex, PPU, new Vector2(0.5f, 0f));
        }

        public static Sprite Pixel()
        {
            const string key = "pixel";
            if (cache.TryGetValue(key, out var cached)) return cached;
            var tex = NewTexture(1, 1);
            tex.SetPixel(0, 0, Color.white);
            return Make(key, tex, 1, new Vector2(0.5f, 0.5f));
        }
    }
}
