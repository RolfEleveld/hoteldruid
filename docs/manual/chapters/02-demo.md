# Demo hotel

Use this to train the team. Do not load it over a hotel that is already in use unless the export path has been kept.

1. Install the desk. See [Deploy](01-deploy.md).
2. In an administrator PowerShell, in the unzipped folder:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Load-DemoHotel.ps1
```

3. The current hotel is exported first. The last line of the window is that path, under the current user's Temp folder.
4. Reload the browser. The open page still shows the previous hotel until it is reloaded.

The demo is Hotel Belvedere: 20 rooms, about two thirds occupied, history from five years ago through six months ahead. The same seed writes the same pattern. Stays that already left are checked out. Stays that began before today and are still open are in house. Today and later stay confirmed.

Practice [Reservations](06-reservations.md), [Arrivals](07-arrivals.md), [In house](08-inhouse.md), and [Check-out](09-checkout.md) on those stays. A stay you check in or pay on this copy is changed. Load the demo again only if the export from step 3 is still the hotel you want to keep.

Next: [House settings](03-house.md).
