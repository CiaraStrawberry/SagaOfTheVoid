"""Narrative editor: edit the campaign dialogue in a browser, with a preview of the in-game comms panel.

    python tools/narrative_editor.py            (then open http://127.0.0.1:8770)
    python tools/narrative_editor.py 8800       (another port)

Edits game/Assets/Data/narrative.json, the file the game reads. Each save keeps the previous version in
tools/narrative_backups/ (last 30, not in git). The page can also rebuild the web build so the change shows up in the
game served by tools/serve_web.py; that needs the Unity editor closed on this project.

Only listens on this computer.
"""
import datetime
import json
import os
import shutil
import subprocess
import sys
import threading
import http.server
from urllib.parse import urlparse

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DATA = os.path.join(ROOT, "game", "Assets", "Data", "narrative.json")
PORTRAITS = os.path.join(ROOT, "game", "Assets", "Art", "Portraits")
PAGE = os.path.join(ROOT, "tools", "narrative_editor.html")
BACKUPS = os.path.join(ROOT, "tools", "narrative_backups")
KEEP_BACKUPS = 30
UNITY = r"C:\Program Files\Unity\Hub\Editor\6000.3.28f1\Editor\Unity.exe"
PROJECT = os.path.join(ROOT, "game")
BUILD_LOG = os.path.join(ROOT, "tools", "buildweb.log")
PORT = int(sys.argv[1]) if len(sys.argv) > 1 else 8770

build = {"state": "idle", "started": None, "finished": None, "code": None}
lock = threading.Lock()


def check(d):
    """The shape Unity's JsonUtility needs (Story.cs). Returns a problem, or None."""
    if not isinstance(d, dict):
        return "not an object"
    for key in ("title", "characters", "missions", "endings"):
        if key not in d:
            return "missing " + key
    for c in d["characters"]:
        if not all(isinstance(c.get(k), str) for k in ("id", "name", "role", "faction", "color")):
            return "a character is missing a field"
    for m in d["missions"]:
        if not isinstance(m.get("index"), int):
            return "a mission has no index"
        for b in m.get("beats", []):
            if not isinstance(b.get("id"), str) or not isinstance(b.get("trigger"), str):
                return "a beat in mission %d has no id or trigger" % m["index"]
            for l in b.get("lines", []):
                if not isinstance(l.get("speaker"), str) or not isinstance(l.get("text"), str):
                    return "a line in %d/%s has no speaker or text" % (m["index"], b["id"])
    for e in d["endings"]:
        if not all(isinstance(e.get(k), str) for k in ("id", "choice", "title", "text")):
            return "an ending is missing a field"
    return None


def save(d):
    os.makedirs(BACKUPS, exist_ok=True)
    if os.path.exists(DATA):
        stamp = datetime.datetime.now().strftime("%Y%m%d-%H%M%S")
        shutil.copyfile(DATA, os.path.join(BACKUPS, "narrative-%s.json" % stamp))
        old = sorted(f for f in os.listdir(BACKUPS) if f.endswith(".json"))
        for f in old[:-KEEP_BACKUPS]:
            os.remove(os.path.join(BACKUPS, f))
    tmp = DATA + ".tmp"
    with open(tmp, "w", encoding="utf-8", newline="\n") as f:
        json.dump(d, f, ensure_ascii=False, indent=1)
    os.replace(tmp, DATA)


def run_build():
    try:
        code = subprocess.call([UNITY, "-batchmode", "-quit", "-projectPath", PROJECT,
                                "-executeMethod", "BuildScript.BuildWeb", "-logFile", BUILD_LOG])
    except OSError as e:
        code = -1
        with open(BUILD_LOG, "a", encoding="utf-8") as f:
            f.write("\ncould not start Unity: %s\n" % e)
    with lock:
        build.update(state="done" if code == 0 else "failed", code=code,
                     finished=datetime.datetime.now().isoformat(timespec="seconds"))


def log_tail(n=12):
    if not os.path.exists(BUILD_LOG):
        return ""
    with open(BUILD_LOG, encoding="utf-8", errors="replace") as f:
        lines = f.read().splitlines()
    # Unity writes a lot; keep the lines that say what went wrong or how it ended.
    keep = [l for l in lines if any(k in l for k in ("error", "Error", "Build", "another Unity instance", "Exiting"))]
    return "\n".join((keep or lines)[-n:])


class Handler(http.server.BaseHTTPRequestHandler):
    def log_message(self, fmt, *args):
        pass

    def reply(self, code, body, kind="application/json"):
        data = body if isinstance(body, bytes) else body.encode("utf-8")
        self.send_response(code)
        self.send_header("Content-Type", kind)
        self.send_header("Content-Length", str(len(data)))
        self.send_header("Cache-Control", "no-store")
        self.end_headers()
        self.wfile.write(data)

    def do_GET(self):
        path = urlparse(self.path).path
        if path in ("/", "/index.html"):
            with open(PAGE, "rb") as f:
                return self.reply(200, f.read(), "text/html; charset=utf-8")
        if path == "/api/narrative":
            with open(DATA, "rb") as f:
                return self.reply(200, f.read())
        if path == "/api/build":
            with lock:
                return self.reply(200, json.dumps({**build, "log": log_tail()}))
        if path.startswith("/portraits/"):
            name = os.path.basename(path)
            p = os.path.join(PORTRAITS, name)
            if name.endswith(".png") and os.path.exists(p):
                with open(p, "rb") as f:
                    return self.reply(200, f.read(), "image/png")
        self.reply(404, '{"error":"not found"}')

    def do_PUT(self):
        if urlparse(self.path).path != "/api/narrative":
            return self.reply(404, '{"error":"not found"}')
        try:
            d = json.loads(self.rfile.read(int(self.headers.get("Content-Length", 0))).decode("utf-8"))
        except (ValueError, UnicodeDecodeError) as e:
            return self.reply(400, json.dumps({"error": "bad JSON: %s" % e}))
        problem = check(d)
        if problem:
            return self.reply(400, json.dumps({"error": problem}))
        save(d)
        self.reply(200, json.dumps({"saved": datetime.datetime.now().isoformat(timespec="seconds")}))

    def do_POST(self):
        if urlparse(self.path).path != "/api/build":
            return self.reply(404, '{"error":"not found"}')
        with lock:
            if build["state"] == "running":
                return self.reply(409, json.dumps({"error": "a build is already running"}))
            build.update(state="running", code=None, finished=None,
                         started=datetime.datetime.now().isoformat(timespec="seconds"))
        threading.Thread(target=run_build, daemon=True).start()
        self.reply(202, json.dumps(build))


if __name__ == "__main__":
    print("Narrative editor at http://127.0.0.1:%d  (editing %s; Ctrl+C stops)" % (PORT, DATA))
    http.server.ThreadingHTTPServer(("127.0.0.1", PORT), Handler).serve_forever()
