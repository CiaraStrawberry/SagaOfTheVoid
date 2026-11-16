"""Character portraits for the comms panel, redrawn from the campaign concept art in an illustrated
style (ink lines, flat hard-edged shading), using OpenAI image edits with reference images.

  python tools/portraits.py <character|all> [--model gpt-image-2.6-flare] [--refs DIR] [--out DIR] [--tag name]

Style references are sent first (how to draw), the character's concept portrait last (who to draw).
The API key is read from .env (a bare key, or OPENAI_API_KEY=...). Outputs PNGs named <character>[_tag].png.
"""
import argparse
import base64
import json
import os
import sys
import urllib.request
import uuid

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
CONCEPTS = os.path.join(ROOT, "art_pack", "Saga_Campaign_Package", "art_assets")

CHARACTERS = {
    "vey": ("admiral_oren_vey.png",
            "Admiral Oren Vey, Imperial Navy, late fifties, swept-back grey hair, lined stern face. Formal charcoal "
            "uniform with muted brass buttons, epaulettes and braid, too formal for the worn ship interior around him. "
            "Cool blue rim light. Controlled, unreadable expression."),
    "kest": ("lieutenant_mara_kest.png",
             "Lieutenant Mara Kest, fighter wing leader, thirties. Scuffed navy flight gear with harness straps. "
             "Lighter face tones, a broad readable silhouette. Practical, dry, a little tired."),
    "sar": ("dr_leena_sar.png",
            "Dr. Leena Sar, former director of transit records, fifties. Pale work uniform, dark hair with grey "
            "streaks. An amber monitor glow is the only warm light on her; the rest is cool shadow. Exact, careful, "
            "exhausted."),
    "sen": ("commander_nara_sen.png",
            "Commander Nara Sen, rebel fleet commander, thirties. Short messy auburn hair, small scar on her brow, "
            "freckles. Maroon armour over dark plating with a damaged shoulder plate and harness. A red warning "
            "light makes her look more severe than her expression is. Direct, unsentimental."),
}

STYLE = (
    "Make a new character illustration in exactly the drawing style of the first images: bold hand-inked black "
    "outlines of varying weight, simplified stylised facial features (graphic-novel, lightly manga-influenced, not "
    "realistic, not painterly), flat colour fills with sharply divided hard-edged shadows, a limited palette of a "
    "few strong colour blocks, sparing neon or screen light, slight print texture. "
    "The LAST image shows who to draw: keep their face shape, age, hairstyle and hair colour, scars, outfit and "
    "colours, but redraw them completely in that style with a fresh pose. Do not copy any person, outfit or "
    "object from the style images; they only show how to draw. "
    "Framing: chest-up portrait, three-quarter view, the head in the upper third, shoulders reaching the bottom "
    "edge. Background: completely transparent, nothing behind the figure (the game puts its own backdrop "
    "behind the portrait); keep a clean, crisp silhouette edge with no glow or haze around it. Light the figure "
    "as if from cockpit screens and a cool rim light. "
    "No text, letters, names, logos, frame, border or interface elements anywhere in the image."
)


def api_key():
    with open(os.path.join(ROOT, ".env"), encoding="utf-8") as f:
        for line in f:
            line = line.strip()
            if not line or line.startswith("#"):
                continue
            return line.split("=", 1)[1].strip() if "=" in line else line
    sys.exit("no key in .env")


def multipart(fields, files):
    boundary = uuid.uuid4().hex
    out = bytearray()
    for k, v in fields.items():
        out += f'--{boundary}\r\nContent-Disposition: form-data; name="{k}"\r\n\r\n{v}\r\n'.encode()
    for k, path in files:
        mime = "image/png" if path.lower().endswith(".png") else "image/jpeg"
        out += (f'--{boundary}\r\nContent-Disposition: form-data; name="{k}"; filename="{os.path.basename(path)}"\r\n'
                f"Content-Type: {mime}\r\n\r\n").encode()
        out += open(path, "rb").read() + b"\r\n"
    out += f"--{boundary}--\r\n".encode()
    return bytes(out), boundary


def generate(who, model, refs, out_dir, tag, quality):
    concept, notes = CHARACTERS[who]
    files = [("image[]", r) for r in refs] + [("image[]", os.path.join(CONCEPTS, concept))]  # style first, character last
    body, boundary = multipart({"model": model, "prompt": STYLE + " Character notes: " + notes,
                                "size": "1024x1536", "quality": quality, "n": "1",
                                "background": "transparent", "output_format": "png"}, files)
    req = urllib.request.Request("https://api.openai.com/v1/images/edits", data=body, method="POST", headers={
        "Authorization": "Bearer " + api_key(), "Content-Type": "multipart/form-data; boundary=" + boundary})
    try:
        with urllib.request.urlopen(req, timeout=600) as r:
            data = json.load(r)
    except urllib.error.HTTPError as e:
        sys.exit(f"{who}: HTTP {e.code}: {e.read().decode(errors='replace')[:800]}")
    os.makedirs(out_dir, exist_ok=True)
    path = os.path.join(out_dir, who + (f"_{tag}" if tag else "") + ".png")
    with open(path, "wb") as f:
        f.write(base64.b64decode(data["data"][0]["b64_json"]))
    print(path, data.get("usage", ""))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("who")
    ap.add_argument("--model", default="gpt-image-2.6-sunburst")
    ap.add_argument("--refs", required=True, help="folder of style reference images")
    ap.add_argument("--out", default=os.path.join(ROOT, "art_pack", "portraits"))
    ap.add_argument("--tag", default="")
    ap.add_argument("--quality", default="high")
    a = ap.parse_args()
    refs = sorted(os.path.join(a.refs, f) for f in os.listdir(a.refs) if f.startswith("ref_"))
    for who in (CHARACTERS if a.who == "all" else [a.who]):
        generate(who, a.model, refs, a.out, a.tag, a.quality)


if __name__ == "__main__":
    main()
