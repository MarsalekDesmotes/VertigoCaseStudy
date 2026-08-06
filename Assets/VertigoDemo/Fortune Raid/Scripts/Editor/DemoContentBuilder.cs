using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

namespace VertigoDemo.Editor
{
    public static partial class DemoContentBuilder
    {
        private const string Root = "Assets/VertigoDemo/Fortune Raid";
        private const string Art = Root + "/Art/";
        private const string Data = Root + "/Data";
        private const string Prefabs = Root + "/Prefabs";
        private const string Scenes = Root + "/Scenes";
        private const string GameScenePath = Scenes + "/Game.unity";
        private const string GamePrefabPath = Prefabs + "/ui_screen_game.prefab";

        [MenuItem("Vertigo Demo/Rebuild Demo Content")]
        public static void BuildAll()
        {
            EnsureFolder(Data);
            EnsureFolder(Prefabs);
            EnsureFolder(Scenes);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetDatabase.ImportAsset(
                Art + "ui_card_frame_4px_zone.png",
                ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

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

            WheelCatalogModel catalog = BuildData();
            BuildCoreSpriteAtlas();
            GameObject prefab = BuildGamePrefab(catalog);
            BuildScene(prefab);
            ConfigureProject();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Vertigo Demo content rebuilt successfully.");
        }

        private static WheelCatalogModel BuildData()
        {
            RewardDefinitionModel bomb = Reward("reward_bomb", "Bomb", "ui_card_icon_death.png", 1);
            RewardDefinitionModel gold = Reward("reward_gold", "Gold", "UI_icon_gold.png", 100);
            RewardDefinitionModel cash = Reward("reward_cash", "Cash", "UI_icon_cash.png", 250);
            RewardDefinitionModel pistol = Reward("reward_pistol", "Pistol Points", "UI_Icons_Pistol_Points.png", 10);
            RewardDefinitionModel rifle = Reward("reward_rifle", "Rifle Points", "UI_Icons_Rifle_Points.png", 10);
            RewardDefinitionModel sniper = Reward("reward_sniper", "Sniper Points", "UI_Icons_Sniper_Points.png", 10);
            RewardDefinitionModel armor = Reward("reward_armor", "Armor Points", "UI_Icons_Armor_Points.png", 15);
            RewardDefinitionModel grenade = Reward("reward_grenade", "M67 Grenade", "ui_icon_render_cons_grenade_m67.png", 1);
            RewardDefinitionModel stimulant = Reward("reward_stimulant", "Neurostim", "ui_icon_render_cons_healthshot_2_neurostim.png", 1);
            RewardDefinitionModel shotgun = Reward(
                "reward_shotgun", "Shotgun", "UI_Icon_Renders_tier3_shotgun.png", 1, stackable: false);
            RewardDefinitionModel smg = Reward(
                "reward_smg", "SMG", "UI_Icon_Renders_tier3_smg.png", 1, stackable: false);
            RewardDefinitionModel chest = Reward(
                "reward_chest", "Super Chest", "UI_icon_chest_super_nolight.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel bronzeChest = Reward(
                "reward_special_chest_bronze", "Bronze Chest", "UI_icon_chest_Bronze_nolight.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel silverChest = Reward(
                "reward_special_chest_silver", "Silver Chest", "UI_icon_chest_silver_nolight.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel smallChest = Reward(
                "reward_special_chest_small", "Small Chest", "UI_icon_chest_small_noligt.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel standardChest = Reward(
                "reward_special_chest_standard", "Standard Chest", "UI_icon_chest_standart_nolight.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel bigChest = Reward(
                "reward_special_chest_big", "Big Chest", "UI_icon_chest_big_nolight.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel pumpkinHelmet = Reward(
                "reward_special_pumpkin_helmet", "Pumpkin Helmet", "ui_icon_helmet_pumpkin.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel aviatorGlasses = Reward(
                "reward_special_aviator_glasses", "Aviator Glasses", "ui_icon_aviator_glasses_easter.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel baseballCap = Reward(
                "reward_special_baseball_cap", "Baseball Cap", "ui_icon_baseball_cap_easter.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel easterBayonet = Reward(
                "reward_special_easter_bayonet", "Easter Bayonet", "ui_icon_mle_bayonet_easter_time.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel summerBayonet = Reward(
                "reward_special_summer_bayonet", "Summer Bayonet", "ui_icon_mle_bayonet_summer_vice.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel molotov = Reward(
                "reward_special_molotov", "Molotov", "ui_icon_render_t_cons_molotov.png", 1, true);
            RewardDefinitionModel grenadeM26 = Reward(
                "reward_special_grenade_m26", "M26 Grenade", "ui_icon_render_cons_grenade_m26.png", 1, true);
            RewardDefinitionModel regenerator = Reward(
                "reward_special_regenerator", "Regenerator", "ui_icon_render_cons_healthshot_2_regenerator.png", 1, true);
            RewardDefinitionModel tier1Shotgun = Reward(
                "reward_special_tier1_shotgun", "Tier 1 Shotgun", "UI_Icon_Renders_tier1_shotgun.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel tier2Mle = Reward(
                "reward_special_tier2_mle", "Tier 2 MLE", "UI_Icon_Renders_tier2_mle.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel tier2Rifle = Reward(
                "reward_special_tier2_rifle", "Tier 2 Rifle", "UI_Icon_Renders_tier2_rifle.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel tier3Sniper = Reward(
                "reward_special_tier3_sniper", "Tier 3 Sniper", "UI_Icon_Renders_tier3_sniper.png", 1,
                special: true, stackable: false);
            RewardDefinitionModel knifePoints = Reward(
                "reward_special_knife_points", "Knife Points", "UI_Icons_Knife_Points.png", 20, true);
            RewardDefinitionModel pistolPointsPlus = Reward(
                "reward_special_pistol_points", "Pistol Points+", "UI_Icons_Pistol_Points_.png", 20, true);
            RewardDefinitionModel shotgunPoints = Reward(
                "reward_special_shotgun_points", "Shotgun Points", "UI_Icons_Shotgun_Points.png", 20, true);
            RewardDefinitionModel smgPoints = Reward(
                "reward_special_smg_points", "SMG Points", "UI_Icons_SMG_Points.png", 20, true);
            RewardDefinitionModel submachinePoints = Reward(
                "reward_special_submachine_points", "Submachine Points", "UI_Icons_Submachine_Points.png", 20, true);
            RewardDefinitionModel vestPoints = Reward(
                "reward_special_vest_points", "Vest Points", "UI_Icons_Vest_Points.png", 20, true);

            WheelDefinitionModel normal = Wheel(
                "wheel_normal",
                ZoneType.Normal,
                "ui_spin_bronze_base.png",
                "ui_spin_bronze_indicator.png",
                new List<WheelSliceDefinitionModel>
                {
                    Slice(gold, 1), Slice(pistol, 1), Slice(rifle, 1), Slice(armor, 1),
                    Slice(cash, 1), Slice(grenade, 1), Slice(stimulant, 1), Bomb(bomb)
                });

            WheelDefinitionModel safe = Wheel(
                "wheel_safe",
                ZoneType.Safe,
                "ui_spin_silver_base.png",
                "ui_spin_silver_indicator.png",
                new List<WheelSliceDefinitionModel>
                {
                    Slice(gold, 2), Slice(pistol, 2), Slice(rifle, 2), Slice(sniper, 2),
                    Slice(cash, 2), Slice(grenade, 2), Slice(stimulant, 2), Slice(armor, 2)
                });

            WheelDefinitionModel superOne = Wheel(
                "wheel_super",
                ZoneType.Golden,
                "ui_spin_golden_base.png",
                "ui_spin_golden_indicator.png",
                new List<WheelSliceDefinitionModel>
                {
                    Slice(pumpkinHelmet, 1), Slice(aviatorGlasses, 1),
                    Slice(baseballCap, 1), Slice(easterBayonet, 1),
                    Slice(summerBayonet, 1), Slice(molotov, 1),
                    Slice(grenadeM26, 1), Slice(regenerator, 1)
                });
            WheelDefinitionModel superTwo = Wheel(
                "wheel_super_2",
                ZoneType.Golden,
                "ui_spin_golden_base.png",
                "ui_spin_golden_indicator.png",
                new List<WheelSliceDefinitionModel>
                {
                    Slice(tier1Shotgun, 1), Slice(tier2Mle, 1),
                    Slice(tier2Rifle, 1), Slice(tier3Sniper, 1),
                    Slice(knifePoints, 1), Slice(shotgunPoints, 1),
                    Slice(smgPoints, 1), Slice(vestPoints, 1)
                });
            WheelDefinitionModel superThree = Wheel(
                "wheel_super_3",
                ZoneType.Golden,
                "ui_spin_golden_base.png",
                "ui_spin_golden_indicator.png",
                new List<WheelSliceDefinitionModel>
                {
                    Slice(bronzeChest, 1), Slice(silverChest, 1),
                    Slice(smallChest, 1), Slice(standardChest, 1),
                    Slice(bigChest, 1), Slice(chest, 1),
                    Slice(pistolPointsPlus, 1), Slice(submachinePoints, 1)
                });

            string path = Data + "/wheel_catalog.asset";
            WheelCatalogModel catalog = AssetDatabase.LoadAssetAtPath<WheelCatalogModel>(path);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<WheelCatalogModel>();
                AssetDatabase.CreateAsset(catalog, path);
            }
            catalog.EditorConfigure(
                normal,
                safe,
                new List<WheelDefinitionModel> { superOne, superTwo, superThree });
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

            // keep the atlas aligned with the complete supplied art folder.
            string[] files = AssetDatabase.FindAssets("", new[] { Art })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(file =>
                    file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            for (int i = 0; i < files.Length; i++)
            {
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(files[i]);
                if (asset != null) objects.Add(asset);
            }

            SpriteAtlasExtensions.Add(atlas, objects.ToArray());
            AssetDatabase.CreateAsset(atlas, path);
            EditorUtility.SetDirty(atlas);
        }

        private static RewardDefinitionModel Reward(
            string id,
            string title,
            string iconFile,
            int amount,
            bool special = false,
            bool stackable = true)
        {
            string path = Data + "/" + id + ".asset";
            RewardDefinitionModel reward = AssetDatabase.LoadAssetAtPath<RewardDefinitionModel>(path);
            if (reward == null)
            {
                reward = ScriptableObject.CreateInstance<RewardDefinitionModel>();
                AssetDatabase.CreateAsset(reward, path);
            }
            reward.EditorConfigure(id, title, Sprite(iconFile), amount, special, stackable);
            EditorUtility.SetDirty(reward);
            return reward;
        }

        private static WheelDefinitionModel Wheel(
            string id,
            ZoneType type,
            string baseFile,
            string indicatorFile,
            List<WheelSliceDefinitionModel> slices)
        {
            string path = Data + "/" + id + ".asset";
            WheelDefinitionModel wheel = AssetDatabase.LoadAssetAtPath<WheelDefinitionModel>(path);
            if (wheel == null)
            {
                wheel = ScriptableObject.CreateInstance<WheelDefinitionModel>();
                AssetDatabase.CreateAsset(wheel, path);
            }
            wheel.EditorConfigure(type, Sprite(baseFile), Sprite(indicatorFile), slices);
            EditorUtility.SetDirty(wheel);
            return wheel;
        }

        private static WheelSliceDefinitionModel Slice(RewardDefinitionModel reward, int multiplier)
        {
            return new WheelSliceDefinitionModel(false, reward, multiplier);
        }

        private static WheelSliceDefinitionModel Bomb(RewardDefinitionModel reward)
        {
            return new WheelSliceDefinitionModel(true, reward, 1);
        }

        private static void BuildScene(GameObject prefab)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);
            Camera camera = cameraObject.GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.018f, 0.025f, 0.045f, 1f);
            camera.orthographic = true;

            GameObject lightObject = new GameObject("Directional Light", typeof(Light));
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Light light = lightObject.GetComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;

            PrefabUtility.InstantiatePrefab(prefab, scene);
            EditorSceneManager.SaveScene(scene, GameScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(GameScenePath, true) };
        }

        private static void ConfigureProject()
        {
            PlayerSettings.companyName = "VertigoDemoCandidate";
            PlayerSettings.productName = "Lucky Strike Wheel";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
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
