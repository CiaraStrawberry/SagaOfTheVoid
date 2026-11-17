"""Campaign story data for the game: dialogue beats, characters and endings.

  python tools/make_narrative.py           (first import only)
  python tools/make_narrative.py --force   (start again from the package, losing edits)

Since 2026-11-17 the dialogue is edited in tools/narrative_editor.py, so game/Assets/Data/narrative.json is the
real copy and this script refuses to overwrite it unless given --force.

Reads art_pack/Saga_Campaign_Package/campaign_narrative.json (the owner's narrative package, draft 2) and writes
game/Assets/Data/narrative.json in a shape Unity's JsonUtility can read (lists, not dictionaries). Also copies the
chosen comms portraits (art_pack/portraits/<id>_<PORTRAIT_TAG>.png, made by tools/portraits.py) to
game/Assets/Art/Portraits/<id>.png.
"""
import json
import os
import shutil
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "art_pack", "Saga_Campaign_Package", "campaign_narrative.json")
OUT = os.path.join(ROOT, "game", "Assets", "Data", "narrative.json")
PORTRAITS_IN = os.path.join(ROOT, "art_pack", "portraits")
PORTRAITS_OUT = os.path.join(ROOT, "game", "Assets", "Art", "Portraits")
PORTRAIT_TAG = "v3"  # transparent background, illustrated style (2026-11-16)

# Button text for the final choice (from the campaign page's ending section).
CHOICE_TEXT = {"publish": "Transmit the Thessa record.", "seal": "Seal the archive pending an inquiry."}


def main():
    src = json.load(open(SRC, encoding="utf-8"))
    characters = [{"id": cid, "name": c["name"], "role": c["role"], "faction": c["faction"], "color": c["color"]}
                  for cid, c in src["characters"].items()]
    missions = []
    for m in src["missions"]:
        missions.append({
            "index": m["index"], "name": m["name"], "act": m["act"], "premise": m["premise"],
            "primary": m["primary"], "secondary": m["secondary"], "bonus": m["bonus"],
            "beats": [{"id": b["id"], "trigger": b["trigger"], "type": b.get("type", ""),
                       "lines": [{"speaker": l["speaker"], "text": l["text"]} for l in b["lines"]]}
                      for b in m["beats"]],
        })
    endings = [{"id": eid, "choice": CHOICE_TEXT.get(eid, e["title"]), "title": e["title"], "text": e["text"]}
               for eid, e in src["ending_choices"].items()]
    out = {"title": src["working_story_title"], "characters": characters, "missions": missions, "endings": endings}
    if os.path.exists(OUT) and "--force" not in sys.argv:
        print("Not overwriting " + OUT + ": it may hold edits from the narrative editor. Use --force to replace it.")
    else:
        with open(OUT, "w", encoding="utf-8") as f:
            json.dump(out, f, ensure_ascii=False, indent=1)

    os.makedirs(PORTRAITS_OUT, exist_ok=True)
    copied = 0
    for c in characters:
        p = os.path.join(PORTRAITS_IN, f"{c['id']}_{PORTRAIT_TAG}.png")
        if os.path.exists(p):
            shutil.copyfile(p, os.path.join(PORTRAITS_OUT, c["id"] + ".png"))
            copied += 1
    beats = sum(len(m["beats"]) for m in missions)
    print(f"{len(missions)} missions, {beats} beats, {len(characters)} characters, {len(endings)} endings; "
          f"{copied} portraits -> {PORTRAITS_OUT}")


if __name__ == "__main__":
    main()
