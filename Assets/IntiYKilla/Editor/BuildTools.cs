using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IntiYKilla.EditorTools
{
    // Herramientas del menú "Inti y Killa":
    // - Generar escenas editables: crea IntiYKilla_Nivel1/2/3 con todos los objetos del nivel
    //   (suelo, flores, enemigos, Sisa...) y guarda el arte como imágenes PNG en Assets/IntiYKilla/Art.
    // - Compilar ejecutable para Windows.
    [InitializeOnLoad]
    public static class BuildTools
    {
        public const string SceneFolder = "Assets/Scenes";
        public const string ArtFolder = "Assets/IntiYKilla/Art";
        public const string ExePath = "Builds/IntiYKilla/IntiYKilla.exe";

        public static string ScenePath(int level) => $"{SceneFolder}/IntiYKilla_Nivel{level}.unity";

        static BuildTools()
        {
            if (Application.isBatchMode) return;
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;
                if (!File.Exists(ScenePath(1)) || !File.Exists(ScenePath(2)) || !File.Exists(ScenePath(3)))
                {
                    if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
                    GenerateScenes(false);
                    EditorSceneManager.OpenScene(ScenePath(1));
                }
            };
        }

        [MenuItem("Inti y Killa/Generar escenas editables (Nivel 1, 2 y 3)")]
        public static void GenerateScenesMenu()
        {
            if (!EditorUtility.DisplayDialog("Inti y Killa",
                "Esto vuelve a crear las escenas IntiYKilla_Nivel1, 2 y 3 desde el código y reemplaza los cambios que hayas hecho en ellas. ¿Continuar?",
                "Sí, generarlas", "Cancelar")) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            GenerateScenes(true);
            EditorSceneManager.OpenScene(ScenePath(1));
        }

        [MenuItem("Inti y Killa/Abrir Nivel 1 · Pueblo")]
        public static void Open1() => OpenLevel(1);
        [MenuItem("Inti y Killa/Abrir Nivel 2 · Laguna")]
        public static void Open2() => OpenLevel(2);
        [MenuItem("Inti y Killa/Abrir Nivel 3 · Bosque")]
        public static void Open3() => OpenLevel(3);

        static void OpenLevel(int level)
        {
            if (!File.Exists(ScenePath(level))) GenerateScenes(false);
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(ScenePath(level));
        }

        public static void GenerateScenes(bool overwrite)
        {
            Directory.CreateDirectory(SceneFolder);
            Directory.CreateDirectory(ArtFolder);
            SpriteFactory.ClearCache();
            SpriteFactory.Baker = BakeSprite;
            try
            {
                for (int level = 1; level <= 3; level++)
                {
                    if (!overwrite && File.Exists(ScenePath(level))) continue;
                    var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    var builder = new GameObject("builder").AddComponent<Bootstrap>();
                    builder.Build(level);
                    Object.DestroyImmediate(builder.gameObject);
                    EditorSceneManager.SaveScene(scene, ScenePath(level));
                }
            }
            finally
            {
                SpriteFactory.Baker = null;
                SpriteFactory.ClearCache();
                Bootstrap.Level = 1;
            }
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath(1), true),
                new EditorBuildSettingsScene(ScenePath(2), true),
                new EditorBuildSettingsScene(ScenePath(3), true),
            };
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Inti y Killa: escenas editables listas en " + SceneFolder + " (arte en " + ArtFolder + ").");
        }

        // Guarda la textura como PNG y la importa como sprite en pixel art.
        static Sprite BakeSprite(string key, Texture2D tex, float ppu, Vector2 pivot)
        {
            string path = $"{ArtFolder}/{key}.png";
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, tex.EncodeToPNG());
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = ppu;
                importer.filterMode = tex.filterMode;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.npotScale = TextureImporterNPOTScale.None;
                importer.maxTextureSize = 2048;
                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)SpriteAlignment.Custom;
                settings.spritePivot = pivot;
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null) Debug.LogWarning("Inti y Killa: no se pudo importar " + path);
            return sprite;
        }

        [MenuItem("Inti y Killa/Compilar ejecutable para Windows")]
        public static void BuildWindowsMenu() => BuildWindows();

        // También se puede llamar desde la línea de comandos:
        // Unity.exe -batchmode -quit -projectPath <ruta> -executeMethod IntiYKilla.EditorTools.BuildTools.BuildWindows
        public static void BuildWindows()
        {
            if (!File.Exists(ScenePath(1)) || !File.Exists(ScenePath(2)) || !File.Exists(ScenePath(3)))
                GenerateScenes(false);
            var scenes = new[] { ScenePath(1), ScenePath(2), ScenePath(3) };
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(scenes[0], true),
                new EditorBuildSettingsScene(scenes[1], true),
                new EditorBuildSettingsScene(scenes[2], true),
            };

            PlayerSettings.productName = "Inti y Killa";
            PlayerSettings.companyName = "Sibri Taipe Camacho";
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.resizableWindow = true;
            PlayerSettings.runInBackground = true;

            Directory.CreateDirectory(Path.GetDirectoryName(ExePath));
            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = ExePath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(options);
            var result = report.summary.result;
            Debug.Log("Inti y Killa: resultado de la compilación = " + result + " -> " + Path.GetFullPath(ExePath));
            if (Application.isBatchMode)
                EditorApplication.Exit(result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
        }
    }
}
