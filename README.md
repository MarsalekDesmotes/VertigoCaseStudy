# Vertigo Wheel Demo

Mobile risk-and-reward wheel demo created for Unity 2021.3.45f2 LTS.

## Open and generate content

1. Install Unity 2021.3.45f2 with Android Build Support, Android SDK & NDK Tools, and OpenJDK.
2. Open this folder in Unity Hub.
3. If the scene or prefab is missing, run `Vertigo Demo > Rebuild Demo Content` once from the Unity menu.
4. Open `Assets/VertigoDemo/Fortune Raid/Scenes/Game.unity` and press Play.

Content rebuild is menu-driven only; it does not run automatically on script reload. UI screens and the result popup are authored as prefab content in edit mode.

Runtime uses MVP: `GameRoot` injects `IZoneRules`, `IRewardRules`, and `ILocalization` into `GameScreenView` + `GamePresenter`. Presenter depends on `IWheelScreenView` / `IPopup` (ISP). UI copy uses `LocalizationKeys` (e.g. `popup.bomb.title`).

## Gameplay

- Normal zones contain seven rewards and one bomb.
- Every fifth zone is a bomb-free silver Safe Zone.
- Every thirtieth zone overrides the Safe Zone rule with a bomb-free Golden Zone.
- Golden pools rotate between seasonal gear, consumables, weapon renders, alternative point rewards, and chest tiers.
- Unique cosmetics, weapon skins/renders, and chests always award one copy and do not stack within a run.
- Special rewards use the supplied offer-shine and star-glow sprites in both the wheel slot and result popup.
- Golden Zones clearly communicate `SPECIAL REWARDS. NO BOMB.` before interaction unlocks.
- Stackable rewards scale with the current zone; unique rewards remain at one copy.
- Gold collected from the wheel credits the revive currency wallet.
- Bomb continue uses currency only (no free ad revive). Give up discards the run loot.
- Leaving is enabled only while idle in Safe and Golden Zones.
- Wheel contents are editable in ScriptableObject assets under `Assets/VertigoDemo/Fortune Raid/Data`.

## Delivery captures

Aspect-ratio frames live under `Delivery/Screenshots` (16:9, 20:9, 4:3).

Capture them in **Play Mode** on Zone 1 after the wheel binds (distinct reward amounts and one bomb icon). Edit-mode prefab defaults are not gameplay captures.

Reference paths:

- [16:9](Delivery/Screenshots/current_game_16x9.png)
- [20:9](Delivery/Screenshots/current_game_20x9.png)
- [4:3](Delivery/Screenshots/current_game_4x3.png)

## AI usage

AI was used as a design and documentation partner in two areas:

1. **README creation** — organizing setup, gameplay rules, architecture notes,
   and delivery references into a reviewer-friendly document.
2. **UX Strategy Ideas** — suggesting and evaluating interaction flow, reward
   communication, safe/super-zone readability, bomb feedback, popup sequencing,
   responsive composition, and visual-polish opportunities.

The final Unity implementation, assets, prefabs, ScriptableObject data, and
runtime behavior were reviewed and integrated in the project codebase.

## Unity package

`VertigoWheelDemo_NoDOTween.unitypackage` contains the demo assets, scripts,
prefabs, scene, data, art, atlas, and TMP resources. DOTween is intentionally
excluded; install/import the DOTween package separately first, then import this
package into a Unity 2021.3.45f2 project. Open the Game scene and press Play.
If content is missing after import, run `Vertigo Demo > Rebuild Demo Content`.
