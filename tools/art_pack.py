"""Pack or restore the recovered art that is kept out of GitHub.

The big recovered textures in game/Assets/Imported (about 2 GB) are not in the repository: GitHub's free
large-file storage is 1 GB. Their .meta files ARE in the repository, so restoring the files by path puts
everything back exactly (same Unity GUIDs, so every prefab and material reference still works).

    python tools/art_pack.py pack      -> art_pack/saga_art.zip   (back it up somewhere safe)
    python tools/art_pack.py restore   <- art_pack/saga_art.zip   (or pass another path)

Without a pack, the art can be recovered again from the 2017 build (saga_original/) with AssetRipper:
see the docs (Recovery record, How to run).
"""
import os
import sys
import zipfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
ART = os.path.join(ROOT, "game", "Assets", "Imported")
EXT = (".png", ".jpg", ".tga", ".psd", ".tif", ".exr", ".wav", ".ogg", ".mp3", ".ttf", ".otf")
DEFAULT = os.path.join(ROOT, "art_pack", "saga_art.zip")


def pack(path):
    os.makedirs(os.path.dirname(path), exist_ok=True)
    n = size = 0
    with zipfile.ZipFile(path, "w", zipfile.ZIP_STORED, allowZip64=True) as z:  # textures are already compressed
        for d, _, files in os.walk(ART):
            for f in files:
                if f.lower().endswith(EXT):
                    full = os.path.join(d, f)
                    z.write(full, os.path.relpath(full, ROOT))
                    n += 1
                    size += os.path.getsize(full)
    print("packed %d files, %.0f MB -> %s" % (n, size / 1048576, path))


def restore(path):
    root = os.path.abspath(ROOT)
    n = 0
    with zipfile.ZipFile(path) as z:
        for name in z.namelist():
            dest = os.path.abspath(os.path.join(ROOT, name))
            if not dest.startswith(os.path.join(root, "game", "Assets", "Imported") + os.sep):
                raise SystemExit("refusing path outside game/Assets/Imported: " + name)
            os.makedirs(os.path.dirname(dest), exist_ok=True)
            with z.open(name) as src, open(dest, "wb") as out:
                out.write(src.read())
            n += 1
    print("restored %d files into game/Assets/Imported" % n)


if __name__ == "__main__":
    if len(sys.argv) < 2 or sys.argv[1] not in ("pack", "restore"):
        raise SystemExit(__doc__)
    (pack if sys.argv[1] == "pack" else restore)(sys.argv[2] if len(sys.argv) > 2 else DEFAULT)
