"""Serve the browser build locally: http://127.0.0.1:8000

Browsers won't run Unity web builds from file://, so they need any static web server.
By default it only listens on this computer; --lan also lets other computers on the home network in.

    python tools/serve_web.py            (then open http://127.0.0.1:8000)
    python tools/serve_web.py 8765 --lan (then open http://<this PC's address>:8765 on the other computer)
"""
import functools
import http.server
import os
import sys

ROOT = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "builds", "Web")
args = [a for a in sys.argv[1:] if a != "--lan"]
PORT = int(args[0]) if args else 8000
HOST = "0.0.0.0" if "--lan" in sys.argv else "127.0.0.1"


class Handler(http.server.SimpleHTTPRequestHandler):
    extensions_map = {**http.server.SimpleHTTPRequestHandler.extensions_map,
                      ".wasm": "application/wasm", ".js": "application/javascript", ".data": "application/octet-stream",
                      ".unityweb": "application/octet-stream"}


if not os.path.exists(os.path.join(ROOT, "index.html")):
    raise SystemExit("No web build at " + ROOT + " - run BuildScript.BuildWeb first.")
print("Serving", ROOT, "at http://%s:%d  (Ctrl+C stops)" % (HOST, PORT))
http.server.ThreadingHTTPServer((HOST, PORT), functools.partial(Handler, directory=ROOT)).serve_forever()
