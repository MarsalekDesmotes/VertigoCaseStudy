# Vertigo Wheel Demo

Mobile risk-and-reward wheel demo created for Unity 2021.3.45f2 LTS.

## Open and generate content

1. Install Unity 2021.3.45f2 with Android Build Support, Android SDK & NDK Tools, and OpenJDK.
2. Open this folder in Unity Hub.
3. If the generated scene is missing, run `Vertigo Demo > Rebuild Demo Content` once.
4. Open `Assets/VertigoDemo/Fortune Raid/Scenes/Game.unity` and press Play.

The project bootstrap runs automatically after the first clean import. UI screens and the result popup are authored as prefab content in edit mode. Runtime code only binds data, listeners, state, sprites, visibility, and animation.

## Gameplay

- Normal zones contain seven rewards and one bomb.
- Every fifth zone is a bomb-free silver Safe Zone.
- Every thirtieth zone overrides the Safe Zone rule with a bomb-free golden Super Zone.
- Golden pools rotate between seasonal gear, consumables, weapon renders, alternative point rewards, and chest tiers.
- Unique cosmetics, weapon skins/renders, and chests always award one copy and do not stack within a run.
- Special rewards use the supplied offer-shine and star-glow sprites in both the wheel slot and result popup.
- Golden Zones clearly communicate `SPECIAL REWARDS. NO BOMB.` before interaction unlocks.
- Stackable rewards scale with the current zone; unique rewards remain at one copy.
- Leaving is enabled only while idle in Safe and Super Zones.
- Wheel contents are editable in ScriptableObject assets under `Assets/VertigoDemo/Fortune Raid/Data`.

## Delivery captures

Responsive gameplay captures are included under `Delivery/Screenshots` for the
required 16:9, 20:9, and 4:3 aspect ratios.

`Delivery/GameplayDemo.mp4` is a 21.87-second 1600x900 gameplay capture showing
two spins, reward popups, accumulated loot, and zone progression.
