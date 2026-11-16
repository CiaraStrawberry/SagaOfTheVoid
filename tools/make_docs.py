"""Build the project docs site: docs/index.html plus its images and media.

Data comes from the game's own files (game/Assets/Data/*.json), so the docs match the game.
Hand-written content (status, decisions, controls, recovery record) lives in CONTENT below.
The page opens straight from disk (no server): data is embedded, media are relative files.

    python tools/make_docs.py
"""
import datetime
import json
import os
import shutil

from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DATA = os.path.join(ROOT, "game", "Assets", "Data")
DOCS = os.path.join(ROOT, "docs")

CONTENT = {
    "overview": [
        ["Engine", "Unity 6000.3.28f1 LTS, URP"],
        ["Ships", "13 recovered, 7 paints each"],
        ["Campaign", "8 missions, 3 maps"],
        ["Tests", "14 rules + 1 play-mode"],
    ],
    "status": [
        ["ok", "Recovery of the 2017 build: code, models, textures, sounds, stats, levels and campaign."],
        ["ok", "3D ships on a flat battlefield, angled strategy camera, effects (shots land on hulls or shield rims, vertical missile launch, engine glow)."],
        ["ok", "Playable skirmish against the computer: selection, move in formation, attack, waypoints, groups, HUD. Windows test build: <code>builds/Skirmish/SagaRemake.exe</code>."],
        ["ok", "Money and buying ships during the fight (2017 economy): build bar, queue, refunds, kill bounties; the computer buys too and falls back when beaten down."],
        ["next", "The three battle maps as scenery, then the campaign missions on top of them."],
        ["warn", "Size scale between big and small ships is a stand-in (big ships shrink, small grow); not yet decided."],
        ["warn", "Test build is 814 MB because ship textures are stored uncompressed; to be compressed."],
    ],
    "repo": [
        ["PLAN.md", "The plan, decisions and status, in text"],
        ["docs/", "This site (generated)"],
        ["game/", "The Unity 6 project"],
        ["game/Assets/Scripts/Sim/", "Battle rules, no Unity dependency (Saga.Sim)"],
        ["game/Assets/Scripts/Game/", "Drawing, skirmish controller, HUD (Saga.Game)"],
        ["game/Assets/Data/", "ships.json, weapons.json, campaign.json"],
        ["game/Assets/Ships/", "Ship prefabs and their 7 paints (built)"],
        ["game/Assets/Imported/", "Art recovered from the 2017 build"],
        ["game/Assets/Editor/", "Ship builder, scene builder, battle recorder, build script"],
        ["game/Assets/Tests/", "Rules tests and the play-mode skirmish test"],
        ["recovery/decompiled/", "The 2017 code as C# (read-only reference)"],
        ["recovery/catalog/", "Asset catalog, stats tables, map contents"],
        ["recovery/assetripper/", "Full AssetRipper export of the build (not in git, 3.5 GB)"],
        ["saga_original/", "The 2017 build itself (not in git, never modified)"],
        ["tools/", "Data, campaign and docs builders; preview captures"],
    ],
    "controls": [
        ["Select", "Left-click a ship, or drag a box. <kbd>Shift</kbd> adds. Double-click: that type on screen. <kbd>Ctrl</kbd>+<kbd>A</kbd>: all. <kbd>Esc</kbd> clears."],
        ["Move", "Right-click space. The group keeps its shape."],
        ["Waypoints", "<kbd>Shift</kbd>+right-click queues a point after the current orders."],
        ["Attack", "Right-click an enemy. Ships chase it and fire on it first; big ships stop at gun range."],
        ["Stop", "<kbd>X</kbd>"],
        ["Groups", "<kbd>Ctrl</kbd>+<kbd>1</kbd>–<kbd>9</kbd> saves, <kbd>1</kbd>–<kbd>9</kbd> selects, twice quickly jumps the camera."],
        ["Pan", "<kbd>W</kbd><kbd>A</kbd><kbd>S</kbd><kbd>D</kbd>, arrow keys, or the screen edge."],
        ["Zoom", "Mouse wheel, toward the cursor. The camera tilts lower as it gets closer."],
        ["Turn camera", "<kbd>Q</kbd> / <kbd>E</kbd>, or hold the middle button and drag."],
        ["Focus", "<kbd>F</kbd> centres on the selection."],
        ["Build", "Click a ship in the build bar (bottom right) to buy it; right-click cancels one and refunds it. Hover for details."],
        ["Game", "<kbd>P</kbd> pause, <kbd>+</kbd> / <kbd>-</kbd> speed (¼× to 4×), <kbd>H</kbd> controls card, <kbd>F5</kbd> restart, <kbd>F10</kbd> quit."],
    ],
    "decisions": [
        ["2026-11-16", "Rebuild in a fresh project instead of upgrading the 2017 one.", "About 7,000 of the 330,000 decompiled lines are game rules; the rest is VR, networking and libraries a 2D-plane game doesn't need."],
        ["2026-11-16", "Unity 6000.3.28f1, the long-term-support release.", "Owner's choice: stability over newest features."],
        ["2026-11-16", "Single-player first; multiplayer later.", "TrueSync is gone and the old lockstep likely desynced. Rules are built to allow it later."],
        ["2026-11-16", "Keep using the art recovered from the build.", "Owner still holds the Asset Store licences; faster than re-downloading."],
        ["2026-11-16", "3D models on a flat play plane, non-VR angled camera (first tried: baked 2D sprites).", "Owner preferred real 3D ships. No 2017 rule used height, so the plane changes nothing in play. Sprites kept as icons."],
        ["2026-11-16", "Fix two 2017 bugs: lasers hit once (not twice), shots land after travel time.", "Clear bugs, not design."],
        ["2026-11-16", "Big ships hold just inside their shortest gun's range.", "In 2017 they flew into the enemy's position and fights became a heap. Approved after the preview."],
        ["2026-11-16", "No cloaking; the Support Frigate's cloak becomes a shield field (1,500 range, 1%/s, not fighters, no stacking).", "The cloak was visual only in 2017 and read as a lighting glitch."],
        ["2026-11-16", "Health bars Homeworld-style: thin, only while under fire or selected.", "Owner feedback: the first bars were too obtrusive."],
        ["2026-11-16", "Missiles launch vertically, then arc onto the target.", "Owner request: look like vertical launch cells."],
        ["2026-11-16", "Docs live in this repo as a generated site, not as hosted pages.", "A lasting project record."],
        ["2026-11-16", "Bigger click target for ships: anywhere in the ship's on-screen footprint, at least 30 px.", "Owner feedback: attack clicks were too fiddly."],
        ["2026-11-16", "Use the intended 2017 build times (Scout 5 s ... Capital 20 s); the computer uses the build queue too; hard difficulty gets its intended 1.4× income.", "2017 bugs: the queue used list position as seconds, and hard got 1.0×. The 2017 computer spawned instantly, which is unfair against a queued player."],
    ],
    "recovery_stats": [
        ["Build", "Unity 5.6.1f1, Mono"],
        ["Objects read", "35,280, 0 errors"],
        ["Code", "3,268 files, 330,947 lines"],
        ["Art", "677 textures, 224 models"],
    ],
    "recovery": [
        ["Code", "ilspycmd 11.2", "C# for the three game libraries in <code>recovery/decompiled/</code>"],
        ["Asset catalog and stats", "UnityPy 1.25 + TypeTreeGenerator", "Every object listed; saved script values (ship and weapon numbers) read"],
        ["Unity project", "AssetRipper 2.0", "Scenes, prefabs, meshes, materials, textures, audio; some script fields dropped"],
        ["Ship prefabs", "game/Assets/Editor/ShipModelBuilder.cs", "Clean prefabs with URP materials in 7 paints"],
        ["Campaign", "tools/make_campaign.py", "8 missions from MainMenuCampaignControlScript.cs:84-95"],
        ["Battle maps", "UnityPy on level3", "MapsPack1-3 contents in <code>recovery/catalog/battle_maps.json</code>"],
    ],
    "scenes": [
        ["level0", "_Scenes/Intro/MenuMain", "Main menu"],
        ["level1", "_Scenes/Intro/MainMenuIntroLobby", "Lobby: mode, map, sky, bots"],
        ["level2", "_Scenes/Tutorial/Tutorial", "Tutorial (13 steps written; it never advanced in 2017)"],
        ["level3", "_Scenes/CaptureAndHold/mptest", "The battle scene: holds MapsPack1-3 and the in-game VR menus"],
        ["level4", "_Scenes/Test/TestArea", "Test area"],
        ["level5", "_Scenes/Intro/MainMenuStartArea", "Start area"],
        ["level6", "_Scenes/Intro/MainMenuFindaGame", "Find a game (multiplayer)"],
    ],
    "bugs": [
        ["fixed", "Lasers dealt their damage twice on a hit."],
        ["fixed", "Shot travel time was ignored; damage landed at once."],
        ["open", "The \"weapon upgrade\" module made guns fire slower (modules not built yet)."],
        ["fixed", "Hard difficulty gave the computer 1.0× income instead of 1.4×."],
        ["fixed", "Build time used the ship's list position as seconds; the intended times are used now."],
        ["open", "Shield overflow damage is thrown away instead of reaching armour (kept as 2017 for now)."],
        ["fixed", "Cloak and fog only hid ships on screen (cloak removed)."],
        ["open", "Losing a campaign mission did nothing (campaign not built yet)."],
        ["open", "The 13-step tutorial never advanced past step 1 (tutorial not built yet)."],
        ["fixed", "The world edge check tested one direction three times (no world edge in the remake yet)."],
    ],
}


def main():
    os.makedirs(os.path.join(DOCS, "img", "icons"), exist_ok=True)
    os.makedirs(os.path.join(DOCS, "img", "skies"), exist_ok=True)
    os.makedirs(os.path.join(DOCS, "media"), exist_ok=True)

    ships = json.load(open(os.path.join(DATA, "ships.json")))["ships"]
    weapons = json.load(open(os.path.join(DATA, "weapons.json")))["weapons"]
    campaign = json.load(open(os.path.join(DATA, "campaign.json"), encoding="utf-8"))

    icons = os.path.join(ROOT, "game", "Assets", "Art", "Icons")
    for s in ships:
        Image.open(os.path.join(icons, s["sprite"] + "_Blue.png")).resize((160, 160), Image.LANCZOS).save(os.path.join(DOCS, "img", "icons", s["sprite"] + ".png"))
    skies = os.path.join(ROOT, "game", "Assets", "Art", "Backgrounds")
    for m in {m["sky"] for m in campaign["missions"]}:
        im = Image.open(os.path.join(skies, m + ".png")).convert("RGB")
        im.thumbnail((600, 300))
        im.save(os.path.join(DOCS, "img", "skies", m + ".jpg"), quality=85)

    media = []
    prev = os.path.join(ROOT, "tools", "preview")
    for name, caption in [
        ("battle_preview_3d.mp4", "Computer against computer, 18 ships a side, shown at 2× speed."),
        ("skirmish_3_attack.png", "Playable skirmish: the Blue fleet on an attack order against the Red Capital."),
        ("skirmish_4_single.png", "One ship selected: armour, shield, speed, shield-field status and guns."),
        ("skirmish_2_moved.png", "The Blue fleet moving in formation, with the controls card."),
    ]:
        src = os.path.join(prev, name)
        if not os.path.exists(src):
            continue
        dst_name = name if name.endswith(".mp4") else name.replace(".png", ".jpg")
        if name.endswith(".png"):
            Image.open(src).convert("RGB").save(os.path.join(DOCS, "media", dst_name), quality=88)
        else:
            shutil.copy2(src, os.path.join(DOCS, "media", dst_name))
        item = {"src": "media/" + dst_name, "caption": caption}
        if name.endswith(".mp4") and os.path.exists(os.path.join(prev, "poster_3d.jpg")):
            shutil.copy2(os.path.join(prev, "poster_3d.jpg"), os.path.join(DOCS, "media", "poster_3d.jpg"))
            item["poster"] = "media/poster_3d.jpg"
        media.append(item)

    data = dict(CONTENT, ships=ships, weapons=weapons, campaign=campaign, media=media)
    html = open(os.path.join(ROOT, "tools", "docs_template.html"), encoding="utf-8").read()
    html = html.replace("/*DATA*/null", json.dumps(data, ensure_ascii=False).replace("</", "<\\/"))
    html = html.replace("__BUILT__", datetime.date.today().isoformat())
    with open(os.path.join(DOCS, "index.html"), "w", encoding="utf-8") as f:
        f.write(html)
    print("docs written:", os.path.join(DOCS, "index.html"), "| ships", len(ships), "| weapons", len(weapons),
          "| missions", len(campaign["missions"]), "| media", len(media))


if __name__ == "__main__":
    main()
