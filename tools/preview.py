"""Open the complete local Unity build over HTTP, without a terminal window."""
import functools
import http.server
import json
from pathlib import Path
import threading
import urllib.request
import webbrowser

ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / "docs"
PORT = 8765
HEALTH = "/__pixelgrid_health__"


class Handler(http.server.SimpleHTTPRequestHandler):
    def do_GET(self):
        if self.path == HEALTH:
            data = json.dumps({"app": "EndlessPixelgrid", "root": str(ROOT)}).encode()
            self.send_response(200)
            self.send_header("Content-Type", "application/json")
            self.send_header("Content-Length", str(len(data)))
            self.end_headers()
            self.wfile.write(data)
            return
        super().do_GET()

    def end_headers(self):
        # A freshly exported build must not mix with cached files from an older build.
        self.send_header("Cache-Control", "no-store")
        super().end_headers()

    def log_message(self, *_args):
        pass


def main():
    if not (DOCS / "index.html").is_file():
        raise SystemExit("WebGL-Build fehlt. In Unity: Schoolmakerdays > Build WebGL for GitHub Pages.")
    try:
        with urllib.request.urlopen(f"http://127.0.0.1:{PORT}{HEALTH}", timeout=1) as reply:
            health = json.load(reply)
        if health == {"app": "EndlessPixelgrid", "root": str(ROOT)}:
            webbrowser.open(f"http://127.0.0.1:{PORT}/")
            return
    except (OSError, ValueError):
        pass
    factory = functools.partial(Handler, directory=str(DOCS))
    try:
        server = http.server.ThreadingHTTPServer(("127.0.0.1", PORT), factory)
    except OSError:
        server = http.server.ThreadingHTTPServer(("127.0.0.1", 0), factory)
    threading.Timer(0.3, lambda: webbrowser.open(f"http://127.0.0.1:{server.server_port}/")).start()
    server.serve_forever()


if __name__ == "__main__":
    main()
