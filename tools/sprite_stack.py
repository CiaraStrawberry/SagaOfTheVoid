"""Sprite-stacked ships: pixel-art slices of every ship, for the game and the demo page.

    1. Unity -batchmode -quit -projectPath tools/bake -executeMethod StackBaker.Run   (renders, 4x size)
    2. python tools/sprite_stack.py

A slice pixel is filled where the hull is at that height. Its colour comes from the lit top view where there is one,
else from the hull texture. The top view goes into the slice at the height of the top surface (from the baked height
map), so it sits on the hull with no gap; anything above the top surface is dropped.

Writes:
  docs/img/stack/<ship>_<colour>.png, stack.json   demo page sheets (docs/sprite-stack.html), shading baked in
  game/Assets/Art/Stacks/<ship>.png                 game atlas: one 8-bit palette index per pixel, slices in a grid,
                                                    shared by both team colours (bit 7 marks the top surface)
  game/Assets/Data/stacks.json                      placement, atlas layout and the Blue and Red palettes
The game (SpriteStacks.cs) turns the atlas into colour per team and shades the lower slices.
"""
import json
import os
import sys
import numpy as np
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
IN = os.path.join(ROOT, "tools", "bake", "out_stack")
DEMO = os.path.join(ROOT, "docs", "img", "stack")
GAME_ART = os.path.join(ROOT, "game", "Assets", "Art", "Stacks")
GAME_DATA = os.path.join(ROOT, "game", "Assets", "Data", "stacks.json")
COLOURS = ("Blue", "Red")
DEMO_SHIPS = ["Scout", "Fighter", "Bomber", "MissileFrigate", "FlackFrigate", "CannonFrigate", "LazerFrigate",
              "RepairFrigate", "Destroyer", "Capital", "Super-Capital"]
GAME_COLOURS = 48  # palette entries per team colour (the demo uses 14 after shading)


def down(img, s):
    """4x render -> final pixels: coverage and the average colour of the covered sub-pixels."""
    a = np.asarray(img.convert("RGBA"), dtype=np.float32) / 255.0
    h, w = a.shape[0] // s, a.shape[1] // s
    a = a[:h * s, :w * s].reshape(h, s, w, s, 4)
    cover = a[..., 3].mean(axis=(1, 3))
    wsum = a[..., 3].sum(axis=(1, 3))[..., None]
    rgb = (a[..., :3] * a[..., 3:4]).sum(axis=(1, 3)) / np.maximum(wsum, 1e-6)
    return cover, rgb


def shade_of(k, n):
    return 0.55 + 0.45 * (k / max(1, n - 1))


def slices(name, m):
    """Per team colour: unshaded colour of each slice (n, side, side, 3); shared masks and top-surface flags."""
    side, n, s = m["side"], m["slices"], m["super"]
    hm = np.asarray(Image.open(os.path.join(IN, f"{name}_height.png")).convert("RGBA"), dtype=np.float32)
    hv = (hm[..., 0] + hm[..., 1] / 255.0) / 255.0
    hv = np.where(hm[..., 3] > 0, hv, -1.0)
    hh, ww = hv.shape[0] // s, hv.shape[1] // s
    hv = hv[:hh * s, :ww * s].reshape(hh, s, ww, s).max(axis=(1, 3))
    top_k = np.clip((hv * n).astype(int), 0, n - 1)
    colours, masks, tops = {}, None, None
    for colour in COLOURS:
        top_cover, top_rgb = down(Image.open(os.path.join(IN, f"{name}_{colour}_top.png")), s)
        top = top_cover > 0.5
        cols = np.zeros((n, side, side, 3), dtype=np.float32)
        mk = np.zeros((n, side, side), dtype=bool)
        tp = np.zeros((n, side, side), dtype=bool)
        for k in range(n):
            cover, rgb = down(Image.open(os.path.join(IN, f"{name}_{colour}_s{k:02d}.png")), s)
            mk[k] = ((cover > 0.4) & ~(top & (k > top_k + 1))) | (top & (top_k == k))
            tp[k] = top & (top_k == k)
            cols[k] = np.where((top_cover > 0.3)[..., None], top_rgb, rgb * 0.85)
        # Thin parts (fins, antennae) the slicing misses exist only as top-surface pixels: give them a pixel of
        # thickness (two slices) so they don't float as a lone layer.
        for k in range(n - 1, 0, -1):
            for d in (1, 2):
                if k - d >= 0:
                    mk[k - d] |= tp[k]
        colours[colour] = np.clip(cols, 0, 1)
        if masks is None:
            masks, tops = mk, tp  # the same hull for every paint
    return colours, masks, tops


def demo_sheet(cols, masks, tops):
    n, side = masks.shape[0], masks.shape[1]
    sheet = np.zeros((side, side * n, 4), dtype=np.uint8)
    for k in range(n):
        shade = np.where(tops[k], max(shade_of(k, n), 0.9), shade_of(k, n))[..., None]
        sheet[:, k * side:(k + 1) * side, :3] = (np.clip(cols[k] * shade, 0, 1) * 255).astype(np.uint8)
        sheet[:, k * side:(k + 1) * side, 3] = np.where(masks[k], 255, 0)
    alpha = sheet[..., 3]
    q = Image.fromarray(sheet[..., :3]).quantize(14, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE)
    return np.dstack([np.asarray(q.convert("RGB")), alpha]).astype(np.uint8)


def kmeans(x, k, iters=14, seed=1):
    rng = np.random.default_rng(seed)
    sample = x[rng.choice(len(x), size=min(len(x), 40000), replace=False)]
    centres = sample[rng.choice(len(sample), size=min(k, len(sample)), replace=False)].copy()
    for _ in range(iters):
        d = ((sample[:, None, :] - centres[None, :, :]) ** 2).sum(-1)
        lab = d.argmin(1)
        for j in range(len(centres)):
            pts = sample[lab == j]
            if len(pts):
                centres[j] = pts.mean(0)
    labels = np.empty(len(x), dtype=np.int32)
    for i in range(0, len(x), 200000):
        d = ((x[i:i + 200000, None, :] - centres[None, :, :]) ** 2).sum(-1)
        labels[i:i + 200000] = d.argmin(1)
    return centres, labels


def game_atlas(name, m, colours, masks, tops):
    """Crop to the hull, share one palette index between the team colours, lay the slices out in a grid."""
    n, side = masks.shape[0], masks.shape[1]
    any_k = masks.any(axis=0)
    ys, xs = np.nonzero(any_k)
    y0, y1, x0, x1 = ys.min(), ys.max() + 1, xs.min(), xs.max() + 1
    w, h = x1 - x0, y1 - y0
    feats = np.concatenate([colours[c][masks] for c in COLOURS], axis=1)  # (pixels, 6)
    centres, labels = kmeans(feats, GAME_COLOURS)
    index = np.zeros(masks.shape, dtype=np.uint8)
    index[masks] = labels + 1
    index[tops & masks] |= 128
    # Grid as square as possible.
    best = None
    for cols in range(1, n + 1):
        rows = -(-n // cols)
        size = max(cols * w, rows * h)
        if best is None or size < best[0]:
            best = (size, cols, rows)
    _, cols, rows = best
    atlas = np.zeros((rows * h, cols * w), dtype=np.uint8)
    for k in range(n):
        r, c = divmod(k, cols)
        atlas[r * h:(r + 1) * h, c * w:(c + 1) * w] = index[k, y0:y1, x0:x1]
    Image.fromarray(atlas).save(os.path.join(GAME_ART, name + ".png"))
    pal = {c: [int(round(v * 255)) for row in centres[:, 3 * i:3 * i + 3] for v in row] for i, c in enumerate(COLOURS)}
    unit = m["unit"]
    return {"sprite": name, "slices": n, "w": int(w), "h": int(h), "cols": cols, "rows": rows, "unit": unit,
            "left": m["cx"] - side * unit / 2 + int(x0) * unit, "top": m["cz"] + side * unit / 2 - int(y0) * unit,
            "miny": m["miny"], "sizey": m["sizey"], "blue": pal["Blue"], "red": pal["Red"]}


SCENERY_IN = os.path.join(ROOT, "tools", "scenery_out")
SCENERY_DATA = os.path.join(ROOT, "game", "Assets", "Data", "scenery_stacks.json")
SCENERY_COLOURS = 32


def scenery():
    """Battle-map scenery cut by StackScenery.cs (game project): colour atlases -> palette atlases for the game."""
    src = os.path.join(SCENERY_IN, "scenery.json")
    if not os.path.exists(src):
        print("no scenery cut yet (Unity -executeMethod StackScenery.Run)")
        return
    data = json.load(open(src))
    for m in data["maps"]:
        pixels = 0
        for part in m["parts"]:
            img = np.asarray(Image.open(os.path.join(SCENERY_IN, part["sprite"] + ".png")).convert("RGBA"))
            mask = img[..., 3] > 0
            top = img[..., 3] == 200
            feats = img[..., :3][mask].astype(np.float32) / 255.0
            centres, labels = kmeans(feats, SCENERY_COLOURS)
            index = np.zeros(mask.shape, dtype=np.uint8)
            index[mask] = labels + 1
            index[top] |= 128
            Image.fromarray(index).save(os.path.join(GAME_ART, part["sprite"] + ".png"))
            part["pal"] = [int(round(v * 255)) for row in centres for v in row]
            pixels += int(mask.sum())
        print(f"{m['id']}: {len(m['parts'])} parts, {pixels:,} filled pixels")
    json.dump(data, open(SCENERY_DATA, "w"), separators=(",", ":"))


def main():
    manifest = json.load(open(os.path.join(IN, "manifest.json")))
    os.makedirs(DEMO, exist_ok=True)
    os.makedirs(GAME_ART, exist_ok=True)
    listing, game = {}, []
    for name, m in manifest.items():
        colours, masks, tops = slices(name, m)
        if name in DEMO_SHIPS and "--demo" in sys.argv:  # the look-test page keeps its first-cut sheets unless asked
            for colour in COLOURS:
                Image.fromarray(demo_sheet(colours[colour], masks, tops)).save(os.path.join(DEMO, f"{name}_{colour}.png"))
            listing[name] = {"side": m["side"], "slices": m["slices"], "per_pixel": m.get("per_pixel", 1)}
        g = game_atlas(name, m, colours, masks, tops)
        game.append(g)
        print(f"{name}: {m['slices']} slices, atlas {g['cols'] * g['w']}x{g['rows'] * g['h']}")
    if listing:
        json.dump(listing, open(os.path.join(DEMO, "stack.json"), "w"), indent=1)
    json.dump({"ships": game}, open(GAME_DATA, "w"), separators=(",", ":"))
    scenery()


if __name__ == "__main__":
    main()
