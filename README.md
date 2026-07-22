# Vertigo Wheel Demo

Mobile risk-and-reward wheel demo created for Unity 2021.3.45f1 LTS. This is the final Unity 2021.3 release before the paid Extended LTS entitlement check introduced in 2021.3.46f1.

## Open and generate content

1. Install Unity 2021.3.45f1 with Android Build Support, Android SDK & NDK Tools, and OpenJDK.
2. Open this folder in Unity Hub.
3. If the generated scene is missing, run `Vertigo Demo > Rebuild Demo Content` once.
4. Open `Assets/VertigoDemo/Scenes/Game.unity` and press Play.

The project bootstrap runs automatically after the first clean import. UI screens and the result popup are authored as prefab content in edit mode. Runtime code only binds data, listeners, state, sprites, visibility, and animation.

## Gameplay

- Normal zones contain seven rewards and one bomb.
- Every fifth zone is a bomb-free silver Safe Zone.
- Every thirtieth zone is a bomb-free golden Super Zone.
- Rewards scale every five zones.
- Leaving is enabled only while idle in Safe and Super Zones.
- Wheel contents are editable in ScriptableObject assets under `Assets/VertigoDemo/Data`.

## Android build

Use `Vertigo Demo > Build Android APK`. The APK is written to `Builds/Android/VertigoWheelDemo.apk`.

## Tests

Open `Window > General > Test Runner`, choose EditMode, and run all tests.

The included EditMode suite covers zone classification, bomb/reset behavior,
reward accumulation, and reward scaling. It has been verified with Unity
2021.3.45f1: 11 tests passed, 0 failed.

## Delivery captures

Responsive gameplay captures are included under `Delivery/Screenshots` for the
required 16:9, 20:9, and 4:3 aspect ratios.

`Delivery/GameplayDemo.mp4` is a 21.87-second 1600x900 gameplay capture showing
two spins, reward popups, accumulated loot, and zone progression.

The release-ready APK is `Delivery/VertigoWheelDemo.apk`. It contains both
`arm64-v8a` and `armeabi-v7a` native libraries. SHA-256:
`D2BBA662AC8FE2D0CAEB80227E93FAE5A90B9162AC9C05BEEEA08D34C0767683`.
