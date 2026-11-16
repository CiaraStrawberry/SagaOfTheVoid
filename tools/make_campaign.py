"""Write the 2017 campaign as data: game/Assets/Data/campaign.json.

Source: MainMenuCampaignControlScript.Populatemaps (recovery/decompiled/Assembly-CSharp/MainMenuCampaignControlScript.cs:84-95).
Enemy ship numbers follow UnitMovementcommandcontroller.getshipbynumber (1 = Scout ... 13 = FlagShip); 0 spawns nothing.
Maps are scenery sets in the 2017 battle scene (level3): MapsPack1 asteroid field, MapsPack2 space station,
MapsPack3 ship graveyard. Allowed ships follow PositinonRelativeToHeadset.cs:65-84 (Fighters / Frigates / Special menu groups).
"""
import json
import os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SHIP_BY_NUMBER = {1: "scout", 2: "fighter", 3: "bomber", 4: "attack_craft", 5: "missile_frigate", 6: "flak_frigate",
                  7: "cannon_frigate", 8: "laser_frigate", 9: "support_frigate", 10: "repair_frigate",
                  11: "destroyer", 12: "capital", 13: "super_capital"}
MAPS = {"map1": "asteroid_field", "map2": "space_station", "map3": "ship_graveyard"}
SKIES = {"skybox1": "Skybox1", "skybox2": "skybox2", "skybox3": "skybox3", "skybox4": "skybox4", "skybox5": "skybox5"}
ALLOWED = {"fighers": "fighters", "frigates": "fighters_and_frigates", "all": "all"}
GOALS = {"Destroyall": "destroy_all", "Survive": "survive", "killTarget": "kill_target"}

# (name, ship numbers, map, skybox, start money, objective, story as written for the VR panel, ships allowed)
MISSIONS = [
    ("Basic Training", [1, 1, 1, 1], "map1", "skybox3", 2000, "Destroyall",
     "Practice your skills \nagainst drones in \npreperation for \ndefense of the  \nempire.", "fighers"),
    ("Core Outskirts", [3, 3, 3], "map2", "skybox1", 2000, "Destroyall",
     "Rebels have been \nsending sorties into \ncivilian shiping lanes. \n\nDestroy their ships \nand protect our \neconomic interests.", "fighers"),
    ("System Patrol", [7, 7, 7, 13], "map3", "skybox4", 4000, "killTarget",
     "The outskirts have \nalways been a pit \nof despair and piracy. \n\n A Famed Pirate boss \nhas been located \nconducting a raid. \nend him.", "fighers"),
    ("Pirate Base", [6, 2, 2, 2, 2], "map1", "skybox5", 5500, "Destroyall",
     "Our intelligence has \nlocated one of their \ndens. \nDestroy it.", "frigates"),
    ("Military Excursion", [7, 7, 0], "map1", "skybox3", 4000, "Survive",
     "We need you to make \na full excursion into \nrebel territory \nand recorver a high \nranking prisoner.", "frigates"),
    ("All out war", [3, 3, 3, 2, 2, 2, 11, 9], "map2", "skybox4", 8000, "Destroyall",
     "The rebels have \nlaunched an all out \nassault on the \nCapital. \n\nDestroy one of their \nen-route fleets.", "frigates"),
    ("Final Assault", [11, 11, 0], "map3", "skybox3", 6000, "Survive",
     "The Imperial palace \nis under siege! \n\nyou need to survive \nagainst the rebel \nfleets!", "all"),
    ("Last Hope", [12, 11, 11, 3, 3, 3, 3, 3, 2, 2, 13], "map1", "skybox4", 12000, "killTarget",
     "The rebel flagship is \nexposed. \n\nstrike now and \nend the rebellion.", "all"),
]


def tidy(story):
    """Undo the hard line breaks of the VR panel; keep paragraph breaks. Spelling left as written in 2017."""
    paras = [" ".join(line.strip() for line in p.split("\n") if line.strip()) for p in story.split("\n\n")]
    return "\n\n".join(p for p in paras if p)


missions = []
for i, (name, ships, mp, sky, money, goal, story, allowed) in enumerate(MISSIONS):
    missions.append({
        "index": i,
        "name": name,
        "enemy_fleet": [SHIP_BY_NUMBER[n] for n in ships if n in SHIP_BY_NUMBER],
        "map": MAPS[mp],
        "sky": SKIES[sky],
        "start_money": money,
        "objective": GOALS[goal],
        # Survive lasts 300 s instead of 1200 s, and is won when the time runs out (UnitMovementcommandcontroller.cs:213-217, 935-942).
        "time_limit": 300 if goal == "Survive" else 1200,
        "ships_allowed": ALLOWED[allowed],
        "story": tidy(story),
        "story_2017": story,
    })

out = os.path.join(ROOT, "game", "Assets", "Data", "campaign.json")
with open(out, "w", encoding="utf-8") as f:
    json.dump({"maps": [
        {"id": "asteroid_field", "scene_2017": "MapsPack1", "contents": "16 big asteroids"},
        {"id": "space_station", "scene_2017": "MapsPack2", "contents": "SpaceStation4"},
        {"id": "ship_graveyard", "scene_2017": "MapsPack3", "contents": "derelict starship and a debris field (86 pieces)"},
    ], "missions": missions}, f, indent=1, ensure_ascii=False)
for m in missions:
    print(f'{m["index"] + 1}. {m["name"]:<20} {m["objective"]:<12} map={m["map"]:<15} money={m["start_money"]:<6} allowed={m["ships_allowed"]:<22} enemies={", ".join(m["enemy_fleet"])}')
print("written", out)
