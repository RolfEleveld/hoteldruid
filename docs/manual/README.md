# Hotel manual

The chapters in `chapters` are the manual. The pictures are in `images`. A hotel can replace a picture with its own screen and list that file in `images/hotel.txt`. The next capture leaves it.

```powershell
./deploy/build/manual.ps1
```

That refreshes the product pictures from the desk at `https://localhost:5001`, then writes `docs/manual/HotelDruid.html` and `docs/manual/out/HotelDruid.epub`. Open the HTML file in a browser. The epub is the same book for a reader.

```powershell
./deploy/build/manual.ps1 -Build
./deploy/build/manual.ps1 -Capture -BaseUrl https://localhost:5001
```

`-Build` only binds the pictures already on disk. `-Capture` only takes pictures. Add `-Force` to replace a picture listed in `hotel.txt`.

The desk must already be running. Capture does not install the hotel and does not change a stay.
