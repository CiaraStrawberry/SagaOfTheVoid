# Saga of the Void: recovery and 2D remake plan

Written 2026-11-16, after the first feasibility tests.

## Short answer

**✅ The remake is doable.** The code and nearly all the art, sound and game numbers can be pulled back out of the old build. Six tests passed; one was blocked and needs your OK (see "Tests").

**🧭 Recommended route: rebuild, do not resurrect.** Start a clean project on a current Unity, bring the recovered art and numbers into it, and write the game rules fresh for 2D, using the old code as the rule book. Do not try to reopen the old project and upgrade it in place.

**📐 Why:** the build holds about 303,000 lines of code, but only about 7,000 of them are your game rules. The rest is VR, networking and bought libraries, most of which no longer exist for current Unity and none of which a 2D mouse-driven game needs.

## What the build is

| Item | Finding |
|---|---|
| Engine | Unity 5.6.1f1, Mono build (the kind that decompiles well) |
| Product | "Saga of the Void", company RowlesCorp, Steam app 535410 |
| Game type | VR space real-time strategy, fleets of ships, up to 3v3 |
| Scenes | 7: main menu, lobby, tutorial, battle (`mptest`), test area, start area, find-a-game |
| Data size | 3.5 GB, 35,280 objects |
| Bought libraries found | VRTK, SteamVR, Oculus, Photon PUN and Voice, TrueSync, Apex Path, Forge3D, Space Graphics Toolkit, Devdog Sci-Fi UI, Particle Playground, HBAO, Volumetric Fog, Galaxia, Steamworks.NET |

## Tests

| # | Test | Result |
|---|---|---|
| 1 | Turn the code libraries back into C# | ✅ 3,268 files, 330,947 lines, in 11 seconds. No leftover compiler junk in your own files. |
| 2 | Read every object in the data files | ✅ 35,280 objects, 0 errors |
| 3 | Export sample art and sound | ✅ Textures up to 8192x8192, a ship model, sprites and two sound clips all came out |
| 4 | Read the numbers saved on ships and weapons | ✅ 6,010 of 6,022 script objects read. The 12 misses are old UnityScript demo objects. |
| 5 | Unity runs without its window, license OK | ✅ Unity 6000.3.6f1, Personal license, 24 seconds for a fresh project |
| 6 | Recovered ship model to 2D sprite | ✅ Frigate imported into Unity 6 and baked to top, side and front sprites with its texture |
| 7 | Rebuild a whole Unity project with AssetRipper | ⛔ Blocked. The tool downloaded, but running a downloaded program needs your approval. |

**📁 Where the results are:**
- `recovery\decompiled\` holds the C# for the three game code libraries.
- `recovery\catalog\catalog.json` lists every texture, model, sound and script.
- `recovery\catalog\ships.csv` and `weapons.csv` hold the saved ship and weapon numbers.
- `recovery\catalog\samples\` holds the sample exports, including `bake_top.png`, the 2D frigate.
- `saga_original\` was only read, never changed.

## What can be recovered

| Kind | Count | Notes |
|---|---|---|
| Textures | 677 | 244 at 2048, 107 at 4096, 11 at 8192. Each ship has 7 team colours plus normal, glow and shine maps. |
| Models | 224 | 860,941 points in total. About 20 ship hulls, plus turrets and scenery. |
| Sprites and fonts | 168 and 13 | UI art |
| Sound clips | 20 | 416 seconds in total. Small; expect to add music and more effects. |
| Animation clips | 57 | Mostly UI and VR hands |
| Particle effects | 484 | Readable, but 3D and tied to Forge3D. Plan to remake the ones you keep. |
| Ship setups | 37 | 13 of them are buyable in a match |
| Weapon and module types | 16 | Cannon, missile, minigun, laser, shield, cloak, upgrades and so on |
| Intro video | 1 | `HenryShort.mp4`, a plain file, already usable |

**❌ What cannot be recovered:** original layered source files for art, code comments, and editor-only tools. Scene and prefab layouts are in the data but need AssetRipper (test 7) or a custom dump to get out in a form Unity can open.

## How the old game works

This comes from a full read of your gameplay code. File and line references are in the code digest I can add to the repo on request.

**🚀 Ships:** 7 hull sizes (light craft to super-capital) and 4 roles. The role is never used by any rule. A ship has armour, shield, speed, turn rate and a points cost. Example: Capital costs 5,000, with 17,000 armour and 17,000 shield.

**🔫 Weapons:** each gun type has a fixed damage (cannon 800, missile 300, laser 3,000, capital main gun 2,000). Whether a shot hits is a dice roll from a table of gun type against target hull size. Shots on screen are only for show.

**🧭 Movement:** ships turn toward a target point and fly forward. There is no route finding and no obstacle dodging. Positions use all three directions, but **no rule depends on height**, so the game flattens to a plane without changing how it plays.

**💰 Economy:** start with 1,500, earn 30 a second up to 7,000, and get a third of a ship's cost for killing it. Ship limit is 23 in 1v1, 14 in 2v2, 9 in 3v3.

**🏆 Modes:** team deathmatch, free-for-all, and "capital ship" (kill the enemy flagship). 20-minute timer. The "capture and hold" scene name has no matching rules in the code.

**🤖 Computer player:** buys from a weighted list that favours frigates, attacks with more than 4 ships, falls back with 3 or fewer. The whole fleet charges the nearest enemy.

**📖 Campaign:** 8 missions with story text, all on the same battle scene with 3 maps. Three goal types: destroy all, survive, kill target.

**🎮 Controls:** every player command comes through VR hand controllers. This all gets replaced by mouse and keyboard.

**🌐 Multiplayer:** Photon for rooms, TrueSync for keeping players in step. Ship logic ran once per drawn frame as well as once per network step, so players very likely drifted out of step.

## Bugs found in the old rules

You decide for each: fix it, or keep it because the balance was tuned around it.

1. Lasers deal their damage twice on a hit.
2. The "weapon upgrade" module makes guns fire slower, not faster.
3. Hard difficulty gives the computer 1.0x income; it was meant to be 1.4x.
4. Build time uses the ship's list position as seconds (scout 1 second, capital 12).
5. Shot travel time is ignored; damage lands at once.
6. Shield overflow damage is thrown away instead of carrying into armour.
7. Cloak and fog of war only hide ships on screen. Hidden ships can still be shot at.
8. Losing a campaign mission does nothing; the loss code can never run.
9. The 13-step tutorial never advances past step 1.
10. The world edge check tests the same direction three times.

## Decided (16 Nov 2026)

- Unity 6000.3.28f1, the newest long-term-support release. Installed; the game project is in `game\`.
- Look: 3D models on a flat play plane, angled RTS camera (changed the same day from baked sprites). Game project uses URP. Ship prefabs in `game\Assets\Ships\`, built by `ShipModelBuilder.cs`. The baked top-down renders are kept as build-menu icons in `game\Assets\Art\Icons\`.
- Single-player first; multiplayer later.
- Keep using the art recovered from the build; flag anything missing. Nothing was missing for the ships.
- AssetRipper: run. Its output is in `recovery\assetripper\` (kept out of git).
- No reference footage; a recorded battle (`tools\preview\battle_preview_3d.mp4`) stands in for it.
- Steam app: decide at release.

- Big ships hold just inside their shortest gun's range (approved; `World.HoldAtRange`, on by default).
- No cloaking for now; the Support Frigate's cloak became a shield field (1,500 range, 1%/s, not fighters, no stacking).
- Missiles launch straight up (vertical launch cells), then arc to the target.

Still open: the scale between big and small ships.

## Status (16 Nov 2026)

- Playable skirmish: `game\Assets\Scenes\Skirmish.unity`. You command Blue against the computer. Selection, move with formation, attack, waypoints, groups, RTS camera, HUD.
- Tests: `game\Assets\Tests\` — 8 rules tests (no Play mode) and 1 Play-mode skirmish test, all passing. Run with `-runTests -testPlatform EditMode` or `PlayMode`.
- Battle recorder: `BattlePreview.Record` (`-quick` for an 8 v 8, 75 s run) writes frames to `tools\preview\frames`.
- Recovered levels: all 7 2017 scenes in `recovery\assetripper\ExportedProject\Assets\_Scenes\`; not yet brought into the new game.
- Next: money and buying ships mid-fight (2017 economy), computer buying, turrets aiming, the 3 battle maps.

## Decisions needed from you (original list, kept for reference)

**1. 🎯 Unity version.** Unity lists these today:

| Version | Kind | My view |
|---|---|---|
| 6000.6.9f1 | Newest full release | **My pick** for "most recent" |
| 6000.3.28f1 | Newest long-term-support release | Safer if you want two years of fixes with no upgrades |
| 6000.7.0b9 | Beta | Not for a release |
| 7000.0.0a16 | Alpha | Not for a release |

You have 6000.3.6f1 installed. Either pick needs an install of a few GB.

**2. 🖼️ How "2D" looks.** Two ways:
- **Baked sprites (my pick):** render each ship from above into flat pictures, as in test 6. True 2D, light on the computer, easy to re-colour.
- **Flat play, 3D models:** keep the models and look straight down with a flat camera. Less work and keeps live lighting, but it is not really a 2D game.

**3. 🌐 Multiplayer.** My pick: ship single-player first (skirmish and campaign), and add multiplayer later on a current networking library. TrueSync is no longer sold as far as I know, and the old step-keeping needs a rewrite either way. The new rules will be written so multiplayer can be added without redoing them.

**4. 📜 Bought art.** The ship models, effects and UI look like Asset Store packs. Before a re-release, check that your Asset Store account still lists them. If it does, download the originals from there; they are cleaner than what I can pull from the build. I have not checked what each pack's licence says about baked sprites.

**5. 🛠️ AssetRipper.** Say yes and I will run it to rebuild scenes and prefabs as Unity files. This gives turret positions on each ship, UI layouts and effect settings. Without it I can still dump positions with a custom script, which is slower and gives less.

**6. 🎥 Reference footage.** Do you still have a VR headset? The old game cannot be played without one, so footage of the original would have to come from you, or from old videos.

**7. 🏪 Steam.** Update the existing app 535410, or release as a new app.

## The plan

### Phase 0: Set up
- Start a git repository with large-file storage. Keep `saga_original\` out of it or read-only.
- Install the chosen Unity version.
- Settle decisions 1 to 5 above.

### Phase 1: Full recovery
- Export every texture, model, sprite, font, sound and animation into sorted folders, with a list that maps each file to where it came from.
- Dump every ship, turret and scene layout: parts, positions, which turret sits where.
- Dump every script's saved numbers, not just ships and weapons.
- Run AssetRipper if approved, and keep its output as a second reference copy.
- **Done when:** every one of the 37 ship setups can be rebuilt from the dump alone.

### Phase 2: Rule book
- Write the game rules as one document, taken from the code: ship table, weapon table, the hit-chance table, economy, computer player, the 8 missions and their text.
- Mark each bug above as "fix" or "keep".
- Mark what gets cut: VR hands, voice chat, Oculus, the galaxy backdrop demo code.
- **Done when:** you have read it and agree it describes the game you want to ship.

### Phase 3: New project and art pipeline
- New Unity project on URP. (Done.)
- A tool that turns each recovered ship into a clean prefab with all 7 team paints. (Done.)
- Turrets turning to aim (they are already separate parts in the prefabs).
- Rebuild the effects you keep (shots, missiles, lasers, shields, explosions, engine glow, warp-in) as 2D effects, reusing recovered textures.
- Backgrounds from the 5 recovered skyboxes.
- **Done when:** one ship of each hull size flies around a test scene with turrets and engine glow.

### Phase 4: Game rules, written fresh
- A fixed-step simulation on a flat plane, kept apart from drawing, so it gives the same result every run.
- Ship and weapon numbers loaded from data files made from `ships.csv` and `weapons.csv`, not typed into code.
- Orders as small messages (spawn; move, attack or waypoint for a list of ships), the same shape the old game used.
- Movement, targeting, the hit table, damage, shields, cloak, fog of war, economy, build queue, win checks.
- Automatic tests that check the numbers against the rule book.
- **Done when:** two computer players can finish a match with no screen attached, and the tests pass.

### Phase 5: Controls and screen layout
- Mouse and keyboard: drag-box select, right-click move and attack, shift for waypoints, control groups, camera pan and zoom.
- In-match screen: money, ship count, build menu and queue, timer, health bars, selection outlines, minimap.
- **Done when:** a full skirmish against the computer is playable start to finish.

### Phase 6: Modes and content
- Skirmish with the three modes, 3 maps, difficulty levels.
- The 8 campaign missions with their story text, a working loss screen, and saved progress.
- A new tutorial written for mouse controls.
- Fleet builder as a 2D screen: pick ships under a points limit and fit turrets to slots.
- **Done when:** the campaign can be played from mission 1 to 8.

### Phase 7: Menus, sound, saves
- Main menu, settings, credits.
- Sound hooked up; music and extra effects sourced, since only 20 clips exist.
- Save files in a plain format.

### Phase 8: Multiplayer (later, if wanted)
- Lobby and matchmaking on a current networking library.
- Players kept in step by sending orders only, which Phase 4 is built to allow.

### Phase 9: Release
- Current Steamworks library, achievements if wanted, store page art.
- Builds, play-testing, balance pass.

## Risks

| Risk | How likely | What limits it |
|---|---|---|
| Licence for a bought pack does not cover re-release, or the pack is gone | Unknown | Decision 4, checked before Phase 3 |
| The flat game feels different from the VR one | Likely | The rules stay; Phase 5 is where feel gets tuned, with you playing it |
| Balance shifts when bugs are fixed | Likely | Decide per bug in Phase 2; keep numbers in data files so tuning is cheap |
| Scene layouts are hard to dump without AssetRipper | Medium | Decision 5 |
| Too little sound and no music | Certain | Budgeted in Phase 7 |
| Newest Unity release has fresh bugs | Low | The long-term-support version is a drop-in fallback |

## Not yet checked

- Whether the old game still starts on this machine.
- Whether Photon and Steam still accept the old app keys.
- What each bought pack's licence allows.
- Scene and prefab layout recovery (test 7).
