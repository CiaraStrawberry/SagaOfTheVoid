"""Backing music for battles and menus, made with Stability AI's Stable Audio API (text to audio).

  python tools/music.py --balance
  python tools/music.py <name> --prompt "..." [--seconds 180] [--model stable-audio-2.5] [--steps N]

Writes art_pack/music/<name>.mp3. The key is the STABLE_AUDIO line in .env.
"""
import argparse
import json
import os
import sys
import urllib.error
import urllib.request
import uuid

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "art_pack", "music")
API = "https://api.stability.ai"


def key():
    with open(os.path.join(ROOT, ".env"), encoding="utf-8") as f:
        for line in f:
            if line.strip().startswith("STABLE_AUDIO") and "=" in line:
                return line.split("=", 1)[1].strip().strip('"')
    sys.exit("no STABLE_AUDIO line in .env")


def request(path, fields=None, accept="application/json"):
    headers = {"Authorization": "Bearer " + key(), "Accept": accept}
    data = None
    if fields is not None:
        boundary = uuid.uuid4().hex
        body = bytearray()
        for k, v in fields.items():
            body += f'--{boundary}\r\nContent-Disposition: form-data; name="{k}"\r\n\r\n{v}\r\n'.encode()
        body += f"--{boundary}--\r\n".encode()
        data = bytes(body)
        headers["Content-Type"] = "multipart/form-data; boundary=" + boundary
    req = urllib.request.Request(API + path, data=data, headers=headers, method="POST" if data else "GET")
    try:
        with urllib.request.urlopen(req, timeout=600) as r:
            return r.read(), r.headers
    except urllib.error.HTTPError as e:
        sys.exit(f"HTTP {e.code}: {e.read().decode(errors='replace')[:600]}")


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("name", nargs="?")
    ap.add_argument("--prompt")
    ap.add_argument("--seconds", type=int, default=180)
    ap.add_argument("--model", default="stable-audio-2.5")
    ap.add_argument("--steps", type=int, default=0)
    ap.add_argument("--balance", action="store_true")
    a = ap.parse_args()
    if a.balance:
        body, _ = request("/v1/user/balance")
        print("credits:", json.loads(body).get("credits"))
        return
    fields = {"prompt": a.prompt, "duration": str(a.seconds), "model": a.model, "output_format": "mp3"}
    if a.steps:
        fields["steps"] = str(a.steps)
    body, headers = request("/v2beta/audio/stable-audio-2/text-to-audio", fields, accept="audio/*")
    os.makedirs(OUT, exist_ok=True)
    path = os.path.join(OUT, a.name + ".mp3")
    with open(path, "wb") as f:
        f.write(body)
    print(path, len(body) // 1024, "KB", "seed", headers.get("seed", "?"))


if __name__ == "__main__":
    main()
