"""Build the remake's data files from the recovery catalog.

Inputs:  recovery/catalog/ships.json (from the old prefabs), tools/bake/out/manifest.json (sprite sizes)
Outputs: game/Assets/Data/ships.json, game/Assets/Data/weapons.json

Weapon damage, hit chances and target preferences are copied from the old code:
_Weapon.Calculate_damage, _Weapon.getmodifyer, _Ship.getpreferance.
"""
import json
import os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
FP = 4294967296.0

BUYABLE = {  # old resource name -> (id, display name, sprite name)
    "scout": ("scout", "Scout", "Scout"),
    "fighter": ("fighter", "Fighter", "Fighter"),
    "bomber": ("bomber", "Bomber", "Bomber"),
    "attackcraft": ("attack_craft", "Attack Craft", "AttackCraft"),
    "missilefrigate": ("missile_frigate", "Missile Frigate", "MissileFrigate"),
    "flackfrigate": ("flak_frigate", "Flak Frigate", "FlackFrigate"),
    "cannonfrigate": ("cannon_frigate", "Cannon Frigate", "CannonFrigate"),
    "lazerfrigate": ("laser_frigate", "Laser Frigate", "LazerFrigate"),
    "supportfrigate": ("support_frigate", "Support Frigate", "SupportFrigate"),
    "repairfrigate": ("repair_frigate", "Repair Frigate", "RepairFrigate"),
    "destroyer": ("destroyer", "Destroyer", "Destroyer"),
    "capital": ("capital", "Capital", "Capital"),
    "super-capital": ("super_capital", "Super-Capital", "Super-Capital"),
}
GUN_IDS = {"Cannon": "cannon", "Missile": "missile", "MiniGun": "minigun", "ShieldGenerator": "shield_repair",
           "Cloak": "shield_field",  # the 2017 cloak, redesigned as a shield support field (decided 2026-11-16)
           "FighterGun": "fighter_gun", "BomberGun": "bomber_gun", "Dual_Role": "dual_role",
           "CapitalMainGun": "capital_main_gun", "Lazer": "laser"}
# Build times in seconds, as the 2017 build menu passed them (PositinonRelativeToHeadset.cs:258-316).
# The 2017 queue then ignored them and used the ship's list number instead (a bug); the remake uses these.
BUILD_TIME = {"scout": 5, "fighter": 7, "bomber": 7, "attack_craft": 8, "missile_frigate": 6, "flak_frigate": 6,
              "cannon_frigate": 7, "laser_frigate": 6, "support_frigate": 6, "repair_frigate": 8,
              "destroyer": 15, "capital": 20, "super_capital": 0}
SPEED_TUNE = {"light": 1.5, "corvette": 1.5, "capital": 0.7, "super_capital": 0.7}
HULLS = ["light", "corvette", "frigate", "cruiser", "destroyer", "capital", "super_capital"]


def fp(v):
    return round(v["_serializedValue"] / FP, 4) if isinstance(v, dict) else v


def hits(light, corvette, frigate, cruiser, destroyer, capital):
    return {"light": light, "corvette": corvette, "frigate": frigate, "cruiser": cruiser,
            "destroyer": destroyer, "capital": capital, "super_capital": capital}


weapons = {
    # Cannon shells: 1,500/s with a 0.1 s launch delay (2017: 440/s + 0.5 s; owner found them too slow, 2026-11-16).
    "cannon":           {"damage": 800, "damage_vs_capital_hull_owner": 275, "prefers": "capital", "hit": hits(.02, .05, .8, .8, .8, 1), "visual": "shell", "projectile_speed": 1500, "launch_delay": 0.1},
    "capital_main_gun": {"damage": 2000, "prefers": "frigate", "hit": hits(.02, .05, .8, .8, .8, 1), "visual": "shell", "projectile_speed": 1500, "launch_delay": 0.1},
    "missile":          {"damage": 300, "prefers": "frigate", "hit": hits(.05, .5, 1, 1, 1, 1), "always_hits_non_light": True, "never_targets_light": True, "visual": "missile"},
    "minigun":          {"damage": 20, "prefers": "light", "hit": hits(.1, .1, .8, .8, .8, 1), "visual": "tracer"},
    "fighter_gun":      {"damage": 20, "prefers": "light", "hit": hits(.1, .5, 1, 1, 1, 1), "visual": "tracer"},
    # Remake: fighters fire pairs of small homing missiles (fewer shots that hit far more often; about the same
    # kill rate as the 2017 6-round bursts at 10%).
    "fighter_missile":  {"damage": 60, "prefers": "light", "hit": hits(.35, .6, 1, 1, 1, 1), "visual": "seeker", "projectile_speed": 1100, "launch_delay": 0.15},
    # Remake: single heavy shots (was 3 x 200 per burst); same damage per salvo.
    "bomber_gun":       {"damage": 600, "prefers": "capital", "hit": hits(.05, .5, 1, 1, 1, 1), "never_targets_light": True, "visual": "bomb"},
    "dual_role":        {"damage": 50, "prefers": "frigate", "hit": hits(.01, .5, .5, .5, 1, 1), "visual": "tracer"},
    "laser":            {"damage": 3000, "prefers": "capital", "hit": hits(.01, .02, .6, .9, .9, .9), "visual": "beam", "travel_time": 3.0},
    "shield_repair":    {"damage": 0, "shield_restore": 300, "targets_friends": True, "hit": hits(1, 1, 1, 1, 1, 1), "visual": "repair_beam"},
    # Not in 2017 (it was a visual-only cloak): friendly non-fighter ships within aura_radius recharge
    # shield_regen_pct of their max shield per second, up to full, even from broken. Does not stack.
    "shield_field":     {"damage": 0, "module": True, "aura_radius": 1500, "shield_regen_pct": 1.0, "visual": "none"},
}
for w in weapons.values():
    w.setdefault("projectile_speed", 440)  # old _Weapon.hittime: distance / 440 + 0.5 s
    w.setdefault("launch_delay", 0.5)

src = json.load(open(os.path.join(ROOT, "recovery", "catalog", "ships.json")))
man = json.load(open(os.path.join(ROOT, "tools", "bake", "out", "manifest.json")))
ships = []
for old, (sid, name, sprite) in BUYABLE.items():
    s = src[old]
    turrets = []
    for t in s["turrets"]:
        turrets.append({
            "weapon": GUN_IDS[t["type"]],
            "range": t["range"],
            "time_between_shots": fp(t["time_between_shots"]),
            "shots_per_burst": t["shots_per_burst"],
            "burst_wait": fp(t["burst_wait"]),
        })
    hull = s["hull"].lower()
    if sid == "super_capital":
        hull = "super_capital"
    ships.append({
        "id": sid, "name": name, "sprite": sprite, "hull": hull,
        "cost": s["cost"], "buyable": sid != "super_capital", "build_time": BUILD_TIME[sid],
        "armor": s["armor"], "shield": s["armor"],
        # Speed: 2017 value x30, then tuned 2026-11-16: fighters/corvettes x1.5, capital-class x0.7 (owner request).
        "speed": round(s["max_speed"] * 30 * SPEED_TUNE.get(hull, 1.0), 2), "turn_rate": s["max_turn"],
        "sprite_world_size": man[sprite]["world_size"],
        "squadron": hull == "light",
        "turrets": turrets,
    })

# ---- Skirmish mode structures (designed 2026-11-16, not in 2017) ----
def gun(weapon, rng, tbs, spb=0, bw=0.0):
    return {"weapon": weapon, "range": rng, "time_between_shots": tbs, "shots_per_burst": spb, "burst_wait": bw}

for s in ships:
    s["icon"] = s["sprite"]
    if s["id"] == "fighter":
        s["turrets"] = [gun("fighter_missile", 1100, 0.35, 2, 3.0)]
    if s["id"] == "bomber":
        s["turrets"] = [gun("bomber_gun", 1300, 3.0, 1, 3.0)]
    if s["id"] == "super_capital":
        # Capital ship mode: the flagship. Heavy guns, very slow. (2017 prefab had no guns; fitted at match start.)
        s.update({"armor": 60000, "shield": 40000, "speed": 25, "turn_rate": 0.12, "turrets": [
            gun("capital_main_gun", 3500, 4.0), gun("capital_main_gun", 3500, 4.0),
            gun("missile", 4500, 0.4, 4, 5.0), gun("missile", 4500, 0.4, 4, 5.0),
            gun("minigun", 1400, 0.1, 10, 3.0), gun("minigun", 1400, 0.1, 10, 3.0)]})

ships.append({
    # Base defence mode: the home base, built from the 2017 space station model. Never moves.
    "id": "home_base", "name": "Home Base", "sprite": "SpaceStation", "icon": "HomeBase", "hull": "super_capital",
    "cost": 0, "buyable": False, "build_time": 0, "armor": 90000, "shield": 60000, "speed": 0, "turn_rate": 0,
    "sprite_world_size": 320, "squadron": False, "turrets": [
        gun("capital_main_gun", 3800, 3.5), gun("capital_main_gun", 3800, 3.5), gun("capital_main_gun", 3800, 3.5),
        gun("missile", 5000, 0.4, 4, 4.0), gun("missile", 5000, 0.4, 4, 4.0), gun("missile", 5000, 0.4, 4, 4.0),
        gun("minigun", 1500, 0.1, 10, 2.5), gun("minigun", 1500, 0.1, 10, 2.5),
        gun("minigun", 1500, 0.1, 10, 2.5), gun("minigun", 1500, 0.1, 10, 2.5)]})
ships.append({
    # Base defence mode: gun platforms around the base, made from the 2017 capital main-gun turret. Never move.
    "id": "defence_platform", "name": "Defence Platform", "sprite": "turrets/CapitalMainGunInGame", "icon": "DefencePlatform",
    "hull": "destroyer", "cost": 0, "buyable": False, "build_time": 0, "armor": 6000, "shield": 6000, "speed": 0,
    "turn_rate": 0, "sprite_world_size": 60, "squadron": False, "turrets": [
        gun("cannon", 2200, 2.5), gun("minigun", 1300, 0.1, 10, 3.0), gun("minigun", 1300, 0.1, 10, 3.0)]})

out = os.path.join(ROOT, "game", "Assets", "Data")
os.makedirs(out, exist_ok=True)
with open(os.path.join(out, "ships.json"), "w") as f:
    json.dump({"ships": ships}, f, indent=1)
with open(os.path.join(out, "weapons.json"), "w") as f:
    json.dump({"weapons": [dict(id=k, **v) for k, v in weapons.items()]}, f, indent=1)
print(len(ships), "ships,", len(weapons), "weapons written to", out)
