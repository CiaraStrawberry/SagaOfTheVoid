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
        ["Tests", "25 rules + 3 play-mode"],
    ],
    "status": [
        ["ok", "Recovery of the 2017 build: code, models, textures, sounds, stats, levels and campaign."],
        ["ok", "3D ships on a flat battlefield, angled strategy camera, effects (shots land on hulls or shield rims, vertical missile launch, engine glow)."],
        ["ok", "Playable skirmish against the computer: selection, move in formation, attack, waypoints, groups, HUD. Windows test build: <code>builds/Skirmish/SagaRemake.exe</code>."],
        ["ok", "Money and buying ships during the fight (2017 economy): build bar, queue, refunds, kill bounties; the computer buys too and falls back when beaten down."],
        ["ok", "The three 2017 battle maps (asteroid field, space station, ship graveyard) as scenery, and all 5 skies."],
        ["ok", "Start menu with skirmish options (full or quick, map, computer difficulty) and the campaign."],
        ["ok", "The 8 campaign missions are playable: objectives, time limits, build tiers, saved progress, next / retry."],
        ["next", "Fleet builder (2017: fit turrets to ships under a points limit); it will replace the campaign's stand-in starter fleets."],
        ["next", "Turrets that turn to aim, and the tutorial."],
        ["ok", "Sound from the 2017 game: menu and battle music, a sound per weapon, warp-ins, explosions, interface clicks and order cues. <kbd>M</kbd> mutes."],
        ["ok", "Three skirmish modes: Free for all, Capital ship (each side has a slow, heavily armed flagship) and Base defence (each side has a fixed home base with 4 gun platforms). Lose the flagship or base, lose the battle."],
        ["warn", "Size scale between big and small ships is a stand-in (big ships shrink, small grow); not yet decided."],
        ["ok", "Builds slimmed: Windows 882 MB to 255 MB; web download 65 MB to 23 MB with sound (two team paints shipped, smaller detail maps, crunch and Brotli for the web, mesh compression)."],
        ["ok", "Web page of our own: full-window game, loading screen, full-screen button, no browser menu on right-click."],
        ["ok", "Selected ships show a running tally: damage dealt, kills, and shields restored for repair ships."],
        ["ok", "Ships warp in through the 2017 warp portal; fighter craft fly individually with trails; built ships wait in a reserve and you choose where they warp in."],
        ["ok", "Browser version (WebGL 2): <code>builds/Web</code>, run with <code>python tools/serve_web.py</code>."],
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
        ["Select", "Left-click a ship, or drag a box. <kbd>Shift</kbd> adds. Double-click: that type on screen. <kbd>Ctrl</kbd>+<kbd>A</kbd>: all. <kbd>Esc</kbd> clears. In the panel at the bottom left, click one ship of a group to select just it; <kbd>Shift</kbd>-click drops it."],
        ["Fight zones", "Left-click anywhere in a dogfight ring or a strike ring selects your craft in it. If you have none there, it selects the enemy craft so you can look at them (no orders). Clicking right on your own big ship still selects the ship."],
        ["Move", "Right-click space. The group keeps its shape."],
        ["Waypoints", "<kbd>Shift</kbd>+right-click queues a point after the current orders."],
        ["Attack-move", "<kbd>T</kbd> then left-click a spot, or <kbd>Ctrl</kbd>+right-click it. Ships head there but stop to fight whatever they meet on the way (big ships hold at gun range, fighters join dogfights), then carry on. A plain move flies straight through."],
        ["Attack", "Right-click an enemy. Ships chase it and fire on it first; big ships stop at gun range."],
        ["Stop", "<kbd>X</kbd>"],
        ["Groups", "<kbd>Ctrl</kbd>+<kbd>1</kbd>–<kbd>9</kbd> saves, <kbd>1</kbd>–<kbd>9</kbd> selects, twice quickly jumps the camera."],
        ["Pan", "<kbd>W</kbd><kbd>A</kbd><kbd>S</kbd><kbd>D</kbd>, arrow keys, or the screen edge."],
        ["Zoom", "Mouse wheel, toward the cursor. The camera tilts lower as it gets closer."],
        ["Turn camera", "<kbd>Q</kbd> / <kbd>E</kbd>, or hold the middle button and drag."],
        ["Focus", "<kbd>F</kbd> centres on the selection."],
        ["Build", "Click a ship in the build bar (bottom right) to buy it; right-click cancels one and refunds it. Hover for details."],
        ["Dogfights", "Fighter fights show as a faint gold ring. Right-click it: your fighters join, other ships attack the enemies in it. Left-click it: select all your ships in that fight."],
        ["Strike zones", "Fighters and bombers attacking a big ship show a faint red ring around it. Click the ship itself as usual. Left-click the ring: select your craft attacking it. Right-click the ring: attack that ship (or, around your own ship, the craft attacking it)."],
        ["Reserve", "Built ships wait in the reserve (above the build bar). Click one, or press <kbd>R</kbd> for all, then click where it should warp in. <kbd>Shift</kbd> keeps placing; right-click or <kbd>Esc</kbd> cancels. Not within 1,000 of an enemy."],
        ["Transmissions", "Campaign missions pause for story calls: a portrait plate with the speaker's name and channel, the lines typing out beside it. Click, <kbd>Space</kbd> or <kbd>Enter</kbd> finishes the line, then the next. <kbd>Esc</kbd> skips the rest of the call (never a decision). Calls are not voiced yet. Each mission has an opening briefing, a call mid-battle and an aftermath; the last mission ends with a choice."],
        ["Pause menu", "<b>Menu</b> button (top right), or <kbd>Esc</kbd> with nothing selected: the battle pauses. Resume, restart, controls, main menu or quit. <kbd>Esc</kbd> resumes."],
        ["Tutorial", "Start screen, under Skirmish: a guided practice battle in open space, ten short steps (camera, selecting, moving, groups, attacking, fighters, building, warping in, a first fight). Each step ticks off what you've done and moves on; <b>Skip step</b> passes one. The menu ticks it once finished."],
        ["Menu", "Start screen: pick a skirmish (mode, map, computer difficulty, full 18 v 18, quick 8 v 8 or small 4 v 4) or a campaign mission. After a battle: next mission, retry or play again, or back to the menu."],
        ["Game", "<kbd>P</kbd> pause, <kbd>+</kbd> / <kbd>-</kbd> speed (¼× to 4×), <kbd>H</kbd> controls card, <kbd>M</kbd> mute, <kbd>F5</kbd> restart, <kbd>F10</kbd> quit."],
    ],
    "decisions": [
        ["2026-11-16", "Rebuild in a fresh project instead of upgrading the 2017 one.", "About 7,000 of the 330,000 decompiled lines are game rules; the rest is VR, networking and libraries a 2D-plane game doesn't need."],
        ["2026-11-16", "Unity 6000.3.28f1, the long-term-support release.", "Owner's choice: stability over newest features."],
        ["2026-11-16", "Single-player first; multiplayer later.", "TrueSync is gone and the old lockstep likely desynced. Rules are built to allow it later."],
        ["2026-11-16", "Keep using the art recovered from the build.", "Owner still holds the Asset Store licences; faster than re-downloading."],
        ["2026-11-16", "3D models on a flat play plane, non-VR angled camera (first tried: baked 2D sprites).", "Owner preferred real 3D ships. No 2017 rule used height, so the plane changes nothing in play. Sprites kept as icons."],
        ["2026-11-16", "Fix two 2017 bugs: lasers hit once (not twice), shots land after travel time.", "Clear bugs, not design."],
        ["2026-11-16", "Big ships hold just inside their shortest gun's range.", "In 2017 they flew into the enemy's position and fights became a heap. Approved after the preview."],
        ["2026-11-16", "No cloaking; the Support Frigate's cloak becomes a shield support (first a field, now a beam: see below).", "The cloak was visual only in 2017 and read as a lighting glitch."],
        ["2026-11-16", "Health bars Homeworld-style: thin, only while under fire or selected.", "Owner feedback: the first bars were too obtrusive."],
        ["2026-11-16", "Missiles launch vertically, then arc onto the target.", "Owner request: look like vertical launch cells."],
        ["2026-11-16", "Docs live in this repo as a generated site, not as hosted pages.", "A lasting project record."],
        ["2026-11-16", "Bigger click target for ships: anywhere in the ship's on-screen footprint, at least 30 px.", "Owner feedback: attack clicks were too fiddly."],
        ["2026-11-16", "Battle maps re-placed for the strategy camera: asteroids pulled in to 30% and 3.5× bigger; station half size and moved under the battle; graveyard pulled in to 30%. All scenery sits below the battle plane and is darkened.", "In 2017 the scenery surrounded a VR player and sat off-screen or far below an RTS camera."],
        ["2026-11-16", "Campaign keeps the 2017 rules: no income except Survive missions, no kill bounty for the player, computer holds 30 s before attacking (not in Survive), build tiers by mission. Destroy-all missions lost on time-out.", "Faithful to MainMenuCampaignControlScript / AIController / UnitMovementcommandcontroller."],
        ["2026-11-16", "Campaign starter fleets are a stand-in by tier (fighters / frigates / all).", "In 2017 the starting fleet came from the player's fleet builder, which is not rebuilt yet."],
        ["2026-11-16", "Built ships go to a reserve; the player chooses where they warp in (not within 1,000 of an enemy). The computer still deploys straight to its fleet.", "Owner request."],
        ["2026-11-16", "Warp-in rebuilt from the 2017 Forge3D WarpJump textures; starting fleets warp in too.", "Owner request: the 2017 arrival effect."],
        ["2026-11-16", "Fighter craft fly individually (formation, dogfight weave, wide circles around big ships), with engine trails; fighters turn on a circle at least 650 wide and never sit idle.", "Owner feedback: tight loops looked janky, idle fighters stood still, 2017 had trails."],
        ["2026-11-16", "Campaign enemies never retreat and hold for 15 s instead of 30 s.", "Owner feedback: mission enemies just stood still (the 3-ship retreat rule parked small mission fleets)."],
        ["2026-11-16", "Browser version as WebGL 2 (WebGPU is still experimental in Unity 6).", "Owner request: a version that runs in the browser."],
        ["2026-11-16", "Bigger battlefield (fleets start 13,000 apart, 9,000 in quick battles); fighters and corvettes 1.5× faster, Destroyers, Capitals and the Super-Capital 0.7× speed; fighters turn on a 380-wide circle and stay in a smaller area.", "Owner feedback after playing."],
        ["2026-11-16", "Gunfire in bursts: each gun has a fixed spot, each burst one aim point; smaller shots and hit flashes.", "Owner feedback: shots sprayed everywhere."],
        ["2026-11-16", "Cannon shells fly at 1,500/s with a 0.1 s launch delay (2017: 440/s + 0.5 s), so cannon hits land sooner.", "Owner feedback: cannon shots too slow."],
        ["2026-11-16", "Menu button during a battle (asks once before leaving).", "Owner request."],
        ["2026-11-16", "Dogfight areas (Battlefleet Gothic style): enemy fighters that meet lock into a fight around a fixed point; any number of squadrons from both sides can join; it ends when one side is left. Right-click the area: fighters join, other ships attack the enemies in it. Left-click: select your ships in it. A plain move order pulls fighters out.", "Owner request: fighters should tangle up in a small area, and the area should be clickable."],
        ["2026-11-16", "Lasers last 1.4 s with a shimmer; every shot has a barrel flash; fighter contrails longer and brighter.", "Owner request."],
        ["2026-11-16", "Narrower windows than 16:9 (towards 1:1) scale the UI to the width and widen the camera view.", "Owner feedback: the menu looked squished at 1:1."],
        ["2026-11-16", "Ship only the Blue and Red paints in builds; detail maps smaller; crunch + Brotli for the web.", "Build size: textures were 97% of the build."],
        ["2026-11-16", "Use the intended 2017 build times (Scout 5 s ... Capital 20 s); the computer uses the build queue too; hard difficulty gets its intended 1.4× income.", "2017 bugs: the queue used list position as seconds, and hard got 1.0×. The 2017 computer spawned instantly, which is unfair against a queued player."],
        ["2026-11-16", "Sound uses the 2017 clips. Battle sounds fade with distance from where the camera looks and pan left or right; each sound has a short minimum gap so big fights stay clear.", "Owner asked what happened to the original sound: it was recovered but not yet wired up."],
        ["2026-11-16", "Dogfights spread out: each craft flies its own lane across the fight area and drifts between it and an enemy's tail.", "Owner feedback: every craft flew in one tight ball at the centre."],
        ["2026-11-16", "Strike zones: craft attacking a big ship fly their own lanes around it and make attack runs. The big ship stays clickable and chooses its own targets.", "Owner request: bombers on a capital ship should look like the dogfights, without blocking the ship."],
        ["2026-11-16", "Missiles speed up through their flight and dive steeply onto the target.", "Owner feedback: missiles slowed down as they arrived."],
        ["2026-11-16", "Skirmish modes. Capital ship: the Super-Capital becomes each side's flagship (60,000 armour, 40,000 shield, speed 25, 2 main guns, 2 missile racks, 2 miniguns). Base defence: a home base from the 2017 space station model (90,000 armour, 60,000 shield, 3 main guns, 3 missile racks, 4 miniguns) with 4 platforms from the 2017 main-gun turret. These do not count toward the fleet limit.", "Owner request. New numbers, not from 2017; tune after playing."],
        ["2026-11-16", "Sound volume follows the camera: near silent at full zoom-out, full volume when zoomed in on the action.", "Owner feedback: every sound played at once."],
        ["2026-11-16", "Big ships turn with weight: turning speed builds up over 0.8 s (frigates) to 3.5 s (Super-Capital) and eases off before the new heading.", "Owner feedback: capital turns were jerky; they should feel slow and bulky."],
        ["2026-11-16", "Fighters warp in along a straight line, fly straight ahead in formation with contrails, then break off (later: about 2 s, see below).", "Owner request."],
        ["2026-11-16", "Fighters only fire forward: a shot is drawn from a craft whose nose points within 35 degrees of the target.", "Owner request. The damage rules are unchanged; only what is drawn."],
        ["2026-11-16", "Enemy ships can be selected to look at their stats (no orders).", "Needed for the fight-zone click rule."],
        ["2026-11-16", "Fighters fire volleys of 6 small orange homing missiles (80% to hit other fighters for 1 point of ordinary damage, where a craft is 6 of the squadron's 30 armour; 20 damage to bigger ships; 650/s) instead of 6-round gun bursts (10%, 20 damage); about the same kill rate, but the fire reads clearly. Missiles launch off the nose and curve onto the target. Bombers fire single heavy shots (600 damage every 3 s, was 3 x 200); they still never target fighters, as in 2017.", "Owner request: concrete firing effects for fighters and bombers."],
        ["2026-11-16", "Shots that hit fighter craft: missiles home in on the craft; gun shots fly straight to where the craft will be. Misses fly straight past. When a squadron loses a craft, the craft that was just hit goes, with a small explosion.", "Owner request: fighter losses should come from being hit."],
        ["2026-11-16", "Contrails are 1.2 s of flight at any frame rate (points by time, not per frame).", "At 60-144 fps they were only 0.2-0.5 s long."],
        ["2026-11-16", "Computer fleets close in on a beaten enemy (3 ships or fewer) instead of holding at gun range.", "Battles stalled with the last few craft out at the edge of range."],
        ["2026-11-16", "Squadron markers, selection rings and click targets follow the squadron's craft, not its rules position; warp portals stay where the ship arrived.", "Owner feedback: portals and selection rings spun while fighters flew straight out of the jump."],
        ["2026-11-16", "Every drawn shot ends exactly when its hit lands. Gun rounds (miniguns, fighter and scout guns, attack craft) fly at 1,600/s after 0.1 s and bombs at 900/s after 0.2 s (2017: 440/s after 0.5 s), so the tracer you see is the real flight.", "Owner feedback: craft exploded before the shot reached them (tracers were drawn for 0.6 s while hits took 2-3 s)."],
        ["2026-11-16", "Fighter missiles are smaller; misses fly on past the target and fade; missile hits end in a fireball with a shock ring and debris.", "Owner request."],
        ["2026-11-16", "Squadron damage: fighter missiles do ordinary damage to squadrons (a weapon setting, damage_vs_light) instead of the 2017 rule where every hit knocks out a whole craft; all other weapons keep the 2017 rule. Their hits slow a fighter's turning by 0.08 instead of 0.5.", "Owner asked for an 80% fighter hit rate with damage adjusted to match; under the 2017 rule damage could not be adjusted."],
        ["2026-11-16", "Misses of every kind (gun rounds, shells, bombs, missiles) fly on past the target and fade.", "Owner request."],
        ["2026-11-16", "Stronger hit effects (white-hot core, bigger fireball, shock ring, more sparks) and thinner fighter missiles with shorter trails.", "Owner feedback."],
        ["2026-11-16", "Fighters fly straight out of the warp for about 2 s, then peel off one by one over 2.5 s with gentle turns.", "Owner feedback: they spun round right after arriving."],
        ["2026-11-16", "Gun turrets on the Destroyer, frigates and defence platforms turn to face what they shoot at, and shots leave from their barrels.", "Owner feedback: turrets never moved."],
        ["2026-11-16", "Support Frigate: the shield field becomes a steady beam on one friendly ship within 1,500 (the one with the weakest shields, preferring one no other beam is on), recharging 4% of that ship's max shield per second. Not fighters; beams do not stack.", "Owner request: a beam on a single friendly that raises its shields."],
        ["2026-11-16", "Bomber shots are always drawn and show as a salvo, one bomb per craft, landing together (still one hit in the rules); bombs bigger and tapered.", "Owner feedback: bomber attacks had no visible projectile (they only drew when a craft pointed straight at the target)."],
        ["2026-11-16", "Attack-move order (T then click, or Ctrl+right-click).", "Owner request."],
        ["2026-11-16", "Support Frigate and Attack Craft set aside for now: not for sale, and wherever one would appear (starting fleets, computer buys, campaign fleets) a Repair Frigate or a Bomber stands in. The shield beam stays in the rules for later.", "Owner request: keep just the Repair Frigate (it already restores a friendly ship's shields with a beam)."],
        ["2026-11-18", "A tutorial: a scripted practice battle (Tutorial.cs) with a coach panel that waits for each action, rather than text pages or a campaign mission. Red is driven by the script (World.ScriptedTeams) and the script ends the battle (World.ScriptedEnd), so enemies arrive only when a step calls them.", "Owner request: a tutorial for the game. Doing each control once teaches it better than reading the controls card."],
        ["2026-11-17", "Dev switches in the start menu's top-right corner (StartMenu.DevPanel, on during development): a checkbox to unlock every campaign mission without counting any as won, and a reset for campaign progress. Set DevPanel to false for a release.", "Owner request: an admin panel with an unlock-every-mission option during development."],
        ["2026-11-17", "Squadrons jumping in: an order given during the straight run after the jump (or an order they arrived with) ends the run and sends the craft round onto it in an arc, instead of first pushing on ahead and circling back. Ships count their orders (Ship.Orders) for this; no rule reads it.", "Owner request: fighters should not circle if ordered; they fly forward a bit, then switch to the order."],
        ["2026-11-17", "Deaths play out (BattleView.Wrecks.cs): big ships break into 1 to 5 chunks (more for bigger ships, different every time) along jagged cracks placed anywhere along the hull and angled up to 35 degrees (Saga/Stack clip); small pieces are thrown harder and spin faster, which drift apart, tumble, burn at the breaks with secondary blasts, char and sink away over about 10 s. Fighters, bombers and scouts that are shot down keep flying for about a second, rolling with a fire trail, then explode. Stacked scenery is drawn at a per-map brightness measured against the lit 3D scenery (StackShots -calibrate), as the unlit stacks had come out 2 to 3 times darker.", "Owner request: ships breaking up and fighters crashing in flight; the backgrounds had got darker."],
        ["2026-11-17", "Ships and battle-map scenery drawn as sprite stacks in game (SpriteStacks.cs): pixel-art slices at their real heights, one mesh per model, coloured per team at load. Ships are cut in tools/bake (StackBaker), scenery in the game project by raycasting (StackScenery), both packed by tools/sprite_stack.py into 8-bit palette atlases (about 0.8 MB in all). The 3D models stay, hidden, for turrets, muzzles and hit tests. Pause menu: Look switches between pixel ships and 3D models. The home base keeps its 3D model for now.", "Owner liked the look test and asked for it in the game, scenery included."],
        ["2026-11-17", "Sprite-stacking look test (docs/sprite-stack.html): every ship cut into one-pixel-high pixel-art slices from its 3D model (tools/bake StackBaker.cs, then tools/sprite_stack.py) and shown spinning and in a small moving fleet. A prototype only; the game still draws 3D models.", "Owner request: see how sprite-stacked ships would look, without building it into the game."],
        ["2026-11-17", "Narrative editor (tools/narrative_editor.py, opens at http://127.0.0.1:8770): edit missions, beats, triggers, lines, characters and endings in a browser, with a preview of the in-game comms panel that plays lines with the game's typing speed. It saves straight to game/Assets/Data/narrative.json (backups in tools/narrative_backups/), flags lines too long for the text box, and can rebuild the web build. make_narrative.py no longer overwrites that file without --force.", "Owner request: an interactive interface for editing the dialogue."],
        ["2026-11-16", "Campaign story \"The Thessa Record\" (owner's narrative package, draft 2): four characters (Admiral Vey, Lt. Kest, Dr. Sar, Commander Sen), three transmissions per mission (briefing, a battle trigger, aftermath) and a two-way ending after mission 8. The battle pauses and dims for each call. A battle call whose moment never came plays at victory, so nothing is skipped. Sen is an unnamed signal until mission 6.", "Owner request: narrative sections with a portrait and text, at least at the start of each level."],
        ["2026-11-16", "Comms portraits redrawn in an illustrated style (ink outlines, flat colour, hard-edged shadows, Citizen Sleeper as the style reference) with OpenAI gpt-image-2.6-sunburst, from the concept portraits, on transparent backgrounds; the game adds the frame, faction colour and scanlines (tools/portraits.py).", "Owner request: character plates in a style like Citizen Sleeper, with removable backgrounds."],
        ["2026-11-16", "The Menu button opens a pause menu (the battle stops) instead of leaving after a second click; Escape steps back: attack-move, deploying, selection, then the menu.", "Owner request: the menu should work as a pause."],
        ["2026-11-16", "Comms portraits shown with nothing behind them, standing on the bottom edge of the screen.", "Owner request: blend in with the display."],
        ["2026-11-16", "Unit voice replies instead of the 2017 order beeps (Marco/Polo): selecting, moving and attacking get a short line from the crew of the biggest selected ship (pilots, a bridge officer or a captain), recorded with ElevenLabs and given a muffled, static radio sound (tools/split_voice.py). The story calls are not voiced yet.", "Owner request: voice clips for unit selection and orders, with a staticky, muffled radio effect."],
        ["2026-11-16", "Fighter craft hold still while the game is paused (they used to jump to their target points, dragging their contrails).", "Owner feedback: pausing messed up the contrails."],
        ["2026-11-16", "New music made with Stable Audio 2.5 (tools/music.py), in the spirit of Homeworld: vast, slow, choir pads and drones, sparse percussion, melancholy rather than heroic. Menu: \"Deep Space Calm\". Battles: \"Battle Lines\", \"Drift of the Fleet\" and \"Fleet Engagement\" in turn, each battle starting on the next track, crossfading; music dips to half volume during story calls. The 2017 loops stay as fallbacks.", "Owner request: Homeworld-inspired backing music."],
        ["2026-11-16", "Missile Frigates hold their heading while firing at range: their vertical launch cells fire in any direction, so they don't turn to point at the target. Ships with guns still turn.", "Owner feedback: missile frigates shouldn't need to point at their target."],
        ["2026-11-16", "Small 4 v 4 skirmish (a Destroyer, a Cannon and a Flak Frigate, and a fighter squadron each).", "Owner request."],
        ["2026-11-16", "Skies drawn as flat pictures behind the battle: each fills the screen at its own shape and slides a little as the camera turns and pans. The 2017 skies are flat 2:1 wallpapers, not seamless 360-degree panoramas, so the skybox stretched them and showed a seam.", "Owner feedback: the backgrounds looked super stretched."],
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
        ["fixed", "Losing a campaign mission did nothing; now a defeat screen offers a retry."],
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
        name = s.get("icon") or s["sprite"]
        src = os.path.join(icons, name + "_Blue.png")
        if os.path.exists(src):
            Image.open(src).resize((160, 160), Image.LANCZOS).save(os.path.join(DOCS, "img", "icons", name + ".png"))
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
        ("menu_1_start.png", "Start menu: skirmish options and the campaign, missions unlocking as you win."),
        ("mission_1_basic_training.png", "Mission 1, Basic Training, on the asteroid field: objective line, mission credits, fighters-only build bar."),
        ("skirmish_5_building.png", "Buying ships: credits, fleet size, the build queue and the build bar."),
        ("map_view_asteroid_field.jpg", "Map: asteroid field."),
        ("map_view_space_station.jpg", "Map: space station, under the battle, Earth in the sky."),
        ("map_view_ship_graveyard.jpg", "Map: ship graveyard, a derelict and a debris field flanking the fleets."),
    ]:
        src = os.path.join(prev, name)
        if not os.path.exists(src):
            continue
        dst_name = name if name.endswith((".mp4", ".jpg")) else name.replace(".png", ".jpg")
        if name.endswith(".png"):
            Image.open(src).convert("RGB").save(os.path.join(DOCS, "media", dst_name), quality=88)
        else:
            shutil.copy2(src, os.path.join(DOCS, "media", dst_name))
        item = {"src": "media/" + dst_name, "caption": caption}
        if name.endswith(".mp4") and os.path.exists(os.path.join(prev, "poster_3d.jpg")):
            shutil.copy2(os.path.join(prev, "poster_3d.jpg"), os.path.join(DOCS, "media", "poster_3d.jpg"))
            item["poster"] = "media/poster_3d.jpg"
        media.append(item)

    # The campaign story as the game has it (edited in tools/narrative_editor.py), with small comms portraits.
    story = json.load(open(os.path.join(DATA, "narrative.json"), encoding="utf-8"))
    os.makedirs(os.path.join(DOCS, "img", "portraits"), exist_ok=True)
    for c in story["characters"]:
        src = os.path.join(ROOT, "game", "Assets", "Art", "Portraits", c["id"] + ".png")
        if os.path.exists(src):
            im = Image.open(src).convert("RGBA")
            im.thumbnail((240, 360))
            im.save(os.path.join(DOCS, "img", "portraits", c["id"] + ".png"))

    data = dict(CONTENT, ships=ships, weapons=weapons, campaign=campaign, media=media, story=story)
    html = open(os.path.join(ROOT, "tools", "docs_template.html"), encoding="utf-8").read()
    html = html.replace("/*DATA*/null", json.dumps(data, ensure_ascii=False).replace("</", "<\\/"))
    html = html.replace("__BUILT__", datetime.date.today().isoformat())
    with open(os.path.join(DOCS, "index.html"), "w", encoding="utf-8") as f:
        f.write(html)
    print("docs written:", os.path.join(DOCS, "index.html"), "| ships", len(ships), "| weapons", len(weapons),
          "| missions", len(campaign["missions"]), "| media", len(media))


if __name__ == "__main__":
    main()
