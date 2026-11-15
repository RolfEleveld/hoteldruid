"""Copy plain Blazor names next to the fingerprinted publish files.

The browser asks for dotnet.js. The publish output stores dotnet.<hash>.js.
UseStaticFiles does not apply the import map, so the plain name has to exist.
"""
import json
import os
import re
import shutil
import sys

root = sys.argv[1]
www = os.path.join(root, "wwwroot")
index_path = os.path.join(www, "index.html")
manifest_path = os.path.join(root, "HotelDruid.Client.staticwebassets.endpoints.json")

if os.path.isfile(index_path):
    text = open(index_path, encoding="utf-8").read()
    framework = os.path.join(www, "_framework")
    if os.path.isdir(framework):
        for name in os.listdir(framework):
            match = re.match(r"blazor\.webassembly\.(.+)\.js$", name)
            if match and not name.endswith((".br", ".gz")):
                updated = text.replace("#[.fingerprint]", "." + match.group(1))
                if updated != text:
                    open(index_path, "w", encoding="utf-8", newline="\n").write(updated)
                break

count = 0
if os.path.isfile(manifest_path):
    with open(manifest_path, encoding="utf-8") as handle:
        data = json.load(handle)
    for endpoint in data.get("Endpoints", []):
        route = endpoint.get("Route") or ""
        asset = endpoint.get("AssetFile") or ""
        selectors = endpoint.get("Selectors") or []
        if not route or not asset or route == asset or selectors:
            continue
        if re.search(r"\.[a-z0-9]{8,12}\.", route):
            continue
        if asset.endswith(".br") or asset.endswith(".gz"):
            continue
        src = os.path.join(www, asset.replace("/", os.sep))
        dest = os.path.join(www, route.replace("/", os.sep))
        if os.path.isfile(src) and not os.path.exists(dest):
            os.makedirs(os.path.dirname(dest), exist_ok=True)
            shutil.copyfile(src, dest)
            count += 1

print(f"linked {count} plain asset names")
