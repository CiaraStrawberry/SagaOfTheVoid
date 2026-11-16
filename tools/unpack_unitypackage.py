"""Unpack a .unitypackage into a Unity project folder (keeps GUIDs via the .meta files).

Unity's own ImportPackage runs asynchronously and does not complete in -batchmode -quit runs,
so packages made by our own tools are unpacked with this instead.

    python tools/unpack_unitypackage.py tools/mapbake/maps.unitypackage game
"""
import os
import sys
import tarfile

pkg, project = sys.argv[1], sys.argv[2]
entries = {}
with tarfile.open(pkg, "r:gz") as tar:
    for m in tar.getmembers():
        if not m.isfile():
            continue
        guid, _, kind = m.name.partition("/")
        guid = guid.lstrip("./")
        entries.setdefault(guid, {})[kind] = tar.extractfile(m).read()

written = 0
root = os.path.abspath(project)
for guid, parts in entries.items():
    if "pathname" not in parts:
        continue
    rel = parts["pathname"].decode("utf-8").splitlines()[0].strip()
    dest = os.path.abspath(os.path.join(project, rel))
    if not dest.startswith(root + os.sep) or not rel.startswith("Assets/"):
        raise SystemExit("refusing path outside the project: " + rel)
    if "asset" in parts:
        os.makedirs(os.path.dirname(dest), exist_ok=True)
        with open(dest, "wb") as f:
            f.write(parts["asset"])
        written += 1
    else:
        os.makedirs(dest, exist_ok=True)
    if "asset.meta" in parts:
        with open(dest + ".meta", "wb") as f:
            f.write(parts["asset.meta"])
print("unpacked", written, "files into", project)
