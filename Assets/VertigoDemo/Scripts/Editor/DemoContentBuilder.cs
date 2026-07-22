using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEditor.U2D;

namespace VertigoDemo.Editor
{
    [InitializeOnLoad]
    public static partial class DemoContentBuilder
    {
        private const string Root = "Assets/VertigoDemo";
        private const string Art = Root + "/Art/";
        private const string Data = Root + "/Data";
        private const string Prefabs = Root + "/Prefabs";
        private const string Scenes = Root + "/Scenes";
        private const string GameScenePath = Scenes + "/Game.unity";
        private const string GamePrefabPath = Prefabs + "/ui_screen_game.prefab";

        static DemoContentBuilder()
        {
            EditorApplication.delayCall += BuildOnFirstImport;
        }

        private static void BuildOnFirstImport()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !File.Exists(GameScenePath))
            {
                BuildAll();
            }
        }

        [MenuItem("Vertigo Demo/Rebuild Demo Content")]
        public static void BuildAll()
        {
            EnsureFolder(Data);
            EnsureFolder(Prefabs);
            EnsureFolder(Scenes);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            DemoUiFactory.Font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            if (DemoUiFactory.Font == null)
            {
                UnityEditor.PackageManager.PackageInfo package =
                    UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.textmeshpro");
                if (package == null)
                {
                    throw new System.InvalidOperationException("TextMeshPro package could not be resolved.");
                }
                AssetDatabase.ImportPackage(
                    Path.Combine(package.resolvedPath, "Package Resources", "TMP Essential Resources.unitypackage"),
                    false);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                DemoUiFactory.Font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                    "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
            }
            if (DemoUiFactory.Font == null)
            {
                throw new System.InvalidOperationException("TextMeshPro essentials could not be imported.");
            }

            WheelCatalog catalog = BuildData();
            BuildCoreSpriteAtlas();
            GameObject prefab = BuildGamePrefab(catalog);
            BuildScene(prefab);
            ConfigureProject();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Vertigo Demo content rebuilt successfully.");
        }

        private static WheelCatalog BuildData()
        {
            RewardDefinition bomb = Reward("reward_bomb", "Bomb", "ui_card_icon_death.png", 1);
            RewardDefinition gold = Reward("reward_gold", "Gold", "UI_icon_gold.png", 100);
            RewardDefinition cash = Reward("reward_cash", "Cash", "UI_icon_cash.png", 250);
            RewardDefinition pistol = Reward("reward_pistol", "Pistol Points", "UI_Icons_Pistol_Points.png", 10);
            RewardDefinition rifle = Reward("reward_rifle", "Rifle Points", "UI_Icons_Rifle_Points.png", 10);
            RewardDefinition sniper = Reward("reward_sniper", "Sniper Points", "UI_Icons_Sniper_Points.png", 10);
            RewardDefinition armor = Reward("reward_armor", "Armor Points", "UI_Icons_Armor_Points.png", 15);
            RewardDefinition grenade = Reward("reward_grenade", "M67 Grenade", "ui_icon_render_cons_grenade_m67.png", 1);
            RewardDefinition stimulant = Reward("reward_stimulant", "Neurostim", "ui_icon_render_cons_healthshot_2_neurostim.png", 1);
            RewardDefinition shotgun = Reward("reward_shotgun", "Shotgun", "UI_Icon_Renders_tier3_shotgun.png", 1);
            RewardDefinition smg = Reward("reward_smg", "SMG", "UI_Icon_Renders_tier3_smg.png", 1);
            RewardDefinition chest = Reward("reward_chest", "Super Chest", "UI_icon_chest_super_nolight.png", 1);

            WheelDefinition normal = Wheel(
                "wheel_normal",
                ZoneType.Normal,
                "ui_spin_bronze_base.png",
                "ui_spin_bronze_indicator.png",
                new List<WheelSliceDefinition>
                {
                    Slice(gold, 1), Slice(pistol, 1), Slice(rifle, 1), Slice(armor, 1),
                    Slice(cash, 1), Slice(grenade, 1), Slice(stimulant, 1), Bomb(bomb)
                });

            WheelDefinition safe = Wheel(
                "wheel_safe",
                ZoneType.Safe,
                "ui_spin_silver_base.png",
                "ui_spin_silver_indicator.png",
                new List<WheelSliceDefinition>
                {
                    Slice(gold, 2), Slice(pistol, 2), Slice(rifle, 2), Slice(sniper, 2),
                    Slice(cash, 2), Slice(grenade, 2), Slice(stimulant, 2), Slice(armor, 2)
                });

            WheelDefinition super = Wheel(
                "wheel_super",
                ZoneType.Super,
                "ui_spin_golden_base.png",
                "ui_spin_golden_indicator.png",
                new List<WheelSliceDefinition>
                {
                    Slice(gold, 10), Slice(cash, 10), Slice(shotgun, 1), Slice(smg, 1),
                    Slice(chest, 1), Slice(sniper, 10), Slice(armor, 10), Slice(stimulant, 5)
                });

            string path = Data + "/wheel_catalog.asset";
            WheelCatalog catalog = AssetDatabase.LoadAssetAtPath<WheelCatalog>(path);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<WheelCatalog>();
                AssetDatabase.CreateAsset(catalog, path);
            }
            catalog.EditorConfigure(normal, safe, super);
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static void BuildCoreSpriteAtlas()
        {
            string path = Data + "/ui_core.spriteatlas";
            if (AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path) != null)
            {
                AssetDatabase.DeleteAsset(path);
            }

            SpriteAtlas atlas = new SpriteAtlas();
            SpriteAtlasPackingSettings packing = atlas.GetPackingSettings();
            packing.enableRotation = false;
            packing.enableTightPacking = false;
            packing.padding = 4;
            atlas.SetPackingSettings(packing);

            SpriteAtlasTextureSettings texture = atlas.GetTextureSettings();
            texture.generateMipMaps = false;
            texture.readable = false;
            texture.sRGB = true;
            atlas.SetTextureSettings(texture);

            string[] files =
            {
                "ui_spin_bronze_base.png", "ui_spin_silver_base.png", "ui_spin_golden_base.png",
                "ui_spin_bronze_indicator.png", "ui_spin_silver_indicator.png", "ui_spin_golden_indicator.png",
                "ui_spin_generic_button.png", "UI_button_orange_standard.png", "UI_button_grey_standard.png",
                "ui_card_frame_12px_neutral.png", "ui_card_frame_gardient.png",
                "ui_card_panel_zone_current_white.png", "ui_card_icon_death.png"
            };
            List<Object> objects = new List<Object>();
            for (int i = 0; i < files.Length; i++)
            {
                Object asset = AssetDatabase.LoadMainAssetAtPath(Art + files[i]);
                if (asset != null) objects.Add(asset);
            }

            SpriteAtlasExtensions.Add(atlas, objects.ToArray());
            AssetDatabase.CreateAsset(atlas, path);
            EditorUtility.SetDirty(atlas);
        }

        private static RewardDefinition Reward(string id, string title, string iconFile, int amount)
        {
            string path = Data + "/" + id + ".asset";
            RewardDefinition reward = AssetDatabase.LoadAssetAtPath<RewardDefinition>(path);
            if (reward == null)
            {
                reward = ScriptableObject.CreateInstance<RewardDefinition>();
                AssetDatabase.CreateAsset(reward, path);
            }
            reward.EditorConfigure(id, title, Sprite(iconFile), amount);
            EditorUtility.SetDirty(reward);
            return reward;
        }

        private static WheelDefinition Wheel(
            string id,
            ZoneType type,
            string baseFile,
            string indicatorFile,
            List<WheelSliceDefinition> slices)
        {
            string path = Data + "/" + id + ".asset";
            WheelDefinition wheel = AssetDatabase.LoadAssetAtPath<WheelDefinition>(path);
            if (wheel == null)
            {
                wheel = ScriptableObject.CreateInstance<WheelDefinition>();
                AssetDatabase.CreateAsset(wheel, path);
            }
            wheel.EditorConfigure(type, Sprite(baseFile), Sprite(indicatorFile), slices);
            EditorUtility.SetDirty(wheel);
            return wheel;
        }

        private static WheelSliceDefinition Slice(RewardDefinition reward, int multiplier)
        {
            return new WheelSliceDefinition(false, reward, multiplier);
        }

        private static WheelSliceDefinition Bomb(RewardDefinition reward)
        {
            return new WheelSliceDefinition(true, reward, 1);
        }

        private static void BuildScene(GameObject prefab)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PrefabUtility.InstantiatePrefab(prefab, scene);
            EditorSceneManager.SaveScene(scene, GameScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(GameScenePath, true) };
        }

        private static void ConfigureProject()
        {
            PlayerSettings.companyName = "VertigoDemoCandidate";
            PlayerSettings.productName = "Lucky Strike Wheel";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.candidate.vertigowheeldemo");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        }

        [MenuItem("Vertigo Demo/Build Android APK")]
        public static void BuildAndroidApk()
        {
            BuildAll();
            string buildDirectory = Path.GetFullPath("Builds/Android");
            Directory.CreateDirectory(buildDirectory);
            string output = Path.Combine(buildDirectory, "VertigoWheelDemo.apk");
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { GameScenePath },
                locationPathName = output,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };
            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                throw new System.Exception("Android build failed: " + report.summary.result);
            }
            Debug.Log("APK created at " + output);
        }

        private static Sprite Sprite(string file)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(Art + file);
        }

        private static void EnsureFolder(string path)
        {
            string[] segments = path.Split('/');
            string current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                string next = current + "/" + segments[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[i]);
                }
                current = next;
            }
        }
    }
}
