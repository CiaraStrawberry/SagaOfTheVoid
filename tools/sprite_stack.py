"""Sprite-stacking prototype: pixel-art slice sheets for docs/sprite-stack.html.

    1. Unity -batchmode -quit -projectPath tools/bake -executeMethod StackBaker.Run   (renders, 4x size)
    2. python tools/sprite_stack.py

Each ship becomes one strip of slices, bottom slice first, in docs/img/stack/<ship>_<colour>.png, listed in
docs/img/stack/stack.json. A slice pixel is filled where the hull is at that height; its colour comes from the lit
top view where there is one (so the top slice looks like the ship from above), else from the hull texture, and
lower slices are darker.
"""
import json
import os
import numpy as np
from PIL import Image

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
IN = os.path.join(ROOT, "tools", "bake", "out_stack")
OUT = os.path.join(ROOT, "docs", "img", "stack")


def down(img, s):
    """4x render -> final pixels: coverage and the average colour of the covered sub-pixels."""
    a = np.asarray(img.convert("RGBA"), dtype=np.float32) / 255.0
    h, w = a.shape[0] // s, a.shape[1] // s
    a = a[:h * s, :w * s].reshape(h, s, w, s, 4)
    cover = a[..., 3].mean(axis=(1, 3))
    wsum = a[..., 3].sum(axis=(1, 3))[..., None]
    rgb = (a[..., :3] * a[..., 3:4]).sum(axis=(1, 3)) / np.maximum(wsum, 1e-6)
    return cover, rgb


def palette(sheet, colours=14):
    """Pixel-art look: the whole ship in a small shared palette, no dithering."""
    alpha = sheet[..., 3]
    q = Image.fromarray(sheet[..., :3]).quantize(colours, method=Image.Quantize.MEDIANCUT, dither=Image.Dither.NONE)
    rgb = np.asarray(q.convert("RGB"))
    return np.dstack([rgb, alpha]).astype(np.uint8)


def main():
    manifest = json.load(open(os.path.join(IN, "manifest.json")))
    os.makedirs(OUT, exist_ok=True)
    listing = {}
    for name, m in manifest.items():
        side, n, s = m["side"], m["slices"], m["super"]
        for colour in ("Blue", "Red"):
            top_cover, top_rgb = down(Image.open(os.path.join(IN, f"{name}_{colour}_top.png")), s)
            sheet = np.zeros((side, side * n, 4), dtype=np.uint8)
            filled = 0
            for k in range(n):
                cover, rgb = down(Image.open(os.path.join(IN, f"{name}_{colour}_s{k:02d}.png")), s)
                mask = cover > 0.4
                col = np.where((top_cover > 0.3)[..., None], top_rgb, rgb * 0.85)
                shade = 0.55 + 0.45 * (k / max(1, n - 1))
                col = np.clip(col * shade, 0, 1)
                sheet[:, k * side:(k + 1) * side, :3] = (col * 255).astype(np.uint8)
                sheet[:, k * side:(k + 1) * side, 3] = np.where(mask, 255, 0)
                filled += int(mask.sum())
            # The top view as an extra lid slice, so the stack always ends on the ship as seen from above.
            lid = np.zeros((side, side, 4), dtype=np.uint8)
            lid[..., :3] = (np.clip(top_rgb, 0, 1) * 255).astype(np.uint8)
            lid[..., 3] = np.where(top_cover > 0.5, 255, 0)
            sheet = np.concatenate([sheet, lid], axis=1)
            sheet = palette(sheet)
            Image.fromarray(sheet).save(os.path.join(OUT, f"{name}_{colour}.png"))
            print(f"{name} {colour}: {side}px, {n} slices + lid, {filled} slice pixels")
        listing[name] = {"side": side, "slices": n + 1, "per_pixel": m.get("per_pixel", 1)}
    json.dump(listing, open(os.path.join(OUT, "stack.json"), "w"), indent=1)


if __name__ == "__main__":
    main()
