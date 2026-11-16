"""Serve the browser build locally: http://127.0.0.1:8000

Browsers won't run Unity web builds from file://, so they need any static web server.
This one only listens on this computer.

    python tools/serve_web.py            (then open http://127.0.0.1:8000)
"""
import functools
import http.server
import os
import sys

ROOT = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "builds", "Web")
PORT = int(sys.argv[1]) if len(sys.argv) > 1 else 8000


class Handler(http.server.SimpleHTTPRequestHandler):
    extensions_map = {**http.server.SimpleHTTPRequestHandler.extensions_map,
                      ".wasm": "application/wasm", ".js": "application/javascript", ".data": "application/octet-stream",
                      ".unityweb": "application/octet-stream"}


if not os.path.exists(os.path.join(ROOT, "index.html")):
    raise SystemExit("No web build at " + ROOT + " - run BuildScript.BuildWeb first.")
print("Serving", ROOT, "at http://127.0.0.1:%d  (Ctrl+C stops)" % PORT)
http.server.ThreadingHTTPServer(("127.0.0.1", PORT), functools.partial(Handler, directory=ROOT)).serve_forever()
