# Saga of the Void — remake

Saga of the Void was a VR space strategy game (Unity 5.6, 2017). Its project files were lost; this branch
rebuilds it from the shipped build as a mouse-and-keyboard game in Unity 6 LTS: the original 3D ships fighting
on a flat battlefield under an angled strategy camera, with the 2017 rules, campaign and maps.

The original 2017 code is on the `master` branch.

## What's here

| Folder | What |
|---|---|
| `game/` | The Unity 6000.3.28f1 project (URP). Open it in Unity Hub; the scene is `Assets/Scenes/Skirmish.unity`. |
| `game/Assets/Scripts/Sim/` | Battle rules, no Unity dependency. |
| `game/Assets/Scripts/Game/` | Drawing, controls, HUD, menus. |
| `game/Assets/Data/` | Ships, weapons and the 8 campaign missions as data. |
| `docs/index.html` | The full project record: ships, weapons, campaign, rules, controls, decisions, recovery. Open it from disk. |
| `recovery/` | The 2017 code decompiled, and catalogs of what was recovered. |
| `tools/` | Data, docs and campaign builders; web server; art pack. |
| `PLAN.md` | The original plan. |

## The recovered art is not in this repository

The ~2 GB of recovered textures in `game/Assets/Imported` are kept off GitHub (the free large-file allowance
is 1 GB). Their `.meta` files are here, so putting the files back restores everything:

```
python tools/art_pack.py restore path/to/saga_art.zip
```

Without a pack, the art can be recovered again from the 2017 build with AssetRipper (see the docs).

## Build and run

```
# tests
Unity.exe -batchmode -projectPath game -runTests -testPlatform EditMode
# Windows build -> builds/Skirmish/SagaRemake.exe
Unity.exe -batchmode -quit -projectPath game -executeMethod BuildScript.BuildWindows
# browser build -> builds/Web, then open http://127.0.0.1:8000
Unity.exe -batchmode -quit -projectPath game -buildTarget WebGL -executeMethod BuildScript.BuildWeb
python tools/serve_web.py
```
