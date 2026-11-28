# Several hotels

Each hotel keeps its own data folder. That folder is the hotel's records: the guests, the stays, and the ledger. A copy, a restore, or a disclosure then stays inside one hotel. Each hotel is its own process and its own port. One restart stops that hotel and leaves the others running.

`Start-HotelDruid.cmd` and `start-hoteldruid.sh` are for one hotel. They always listen on `http://127.0.0.1:8080/` and always use the `data` folder next to themselves. A second start of that command fails because the port is already taken. The scenarios below start `HotelDruid.Api` once per hotel.

The [deployment runbook](deployment-runbook.md) is one container stack: one hotel, and optionally a proxy and Keycloak on a public name. The scenarios here are several hotels on one machine.

## Choose a scenario

| Scenario | Who opens the desk | What this machine runs |
| --- | --- | --- |
| [Ports on this computer](#ports-on-this-computer) | A person at this computer, by port | One program, one process per hotel |
| [Names from a hosts file](#names-from-a-hosts-file) | A few other desks, by `https://` name | nginx, and one Linux process per hotel |
| [Names from local DNS](#names-from-local-dns) | Every desk that uses the DNS server, by the same names | The same nginx and the same processes |
| [One container per hotel](#one-container-per-hotel) | By port, or by the names above | Docker, one container per hotel |

Each process uses only its own data folder. The write lock is inside one process, so two processes writing one folder can corrupt it. A backup of a hotel is a copy of its data folder. The first visit to each hotel is that hotel's setup.

## Ports on this computer

Unzip the copy folder once. On Windows that is `HotelDruid-selfcontained-win-x64.zip`. On Linux it is `HotelDruid-selfcontained-linux-x64.zip`. Create a data folder and a log folder for each hotel, then start the program twice.

Windows:

```powershell
$program = 'C:\HotelDruid\HotelDruid.Api.exe'
New-Item -ItemType Directory -Force -Path 'C:\Hotels\Annunziata\data', 'C:\Hotels\Annunziata\logs', 'C:\Hotels\Second\data', 'C:\Hotels\Second\logs' | Out-Null

$env:HOTELDRUID_DATAROOT = 'C:\Hotels\Annunziata\data'
$env:HOTELDRUID_LOGDIR = 'C:\Hotels\Annunziata\logs'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:8080'
Start-Process $program

$env:HOTELDRUID_DATAROOT = 'C:\Hotels\Second\data'
$env:HOTELDRUID_LOGDIR = 'C:\Hotels\Second\logs'
$env:ASPNETCORE_URLS = 'http://127.0.0.1:8081'
Start-Process $program
```

Open `http://127.0.0.1:8080/` and `http://127.0.0.1:8081/`. Closing a window stops that hotel.

Linux, from the unzipped folder. The program file is `HotelDruid.Api`. That folder already contains the runtime. The .NET SDK stays on the machine that builds the program.

```sh
HOTELDRUID_DATAROOT=/var/hotels/annunziata/data \
HOTELDRUID_LOGDIR=/var/hotels/annunziata/logs \
ASPNETCORE_ENVIRONMENT=Production \
ASPNETCORE_URLS=http://127.0.0.1:8080 \
./HotelDruid.Api
```

The second hotel uses another data folder and `http://127.0.0.1:8081`. These ports answer only on this computer. Start each hotel again after a reboot.

The installed desk, from `install.ps1`, is already a separate hotel at `https://localhost:5001/` with its files under `C:\ProgramData\HotelDruid`. It can run beside these processes.

## How a name finds its hotel

The next two scenarios use one Linux machine. nginx listens on ports 80 and 443. Each hotel listens on `127.0.0.1` and its own port. Another computer cannot open a hotel by port.

A hosts file or a DNS server can only turn a name into an address. It cannot choose a port. Every hotel name points at this machine, and the browser connects to port 443. The browser sends the name twice: once in the TLS handshake, so nginx can pick the certificate, and once in the `Host` header, so nginx can pick the server block.

| What is asked | Where it is decided | Result |
| --- | --- | --- |
| `annunziata.internal` | Hosts file or DNS on the desk | This machine's IP address |
| The same name, on port 443 | nginx on this machine | `127.0.0.1:8080` and `/var/hotels/annunziata/data` |
| `second.internal` | The same hosts file or DNS | The same IP address |
| The same name, on port 443 | nginx on this machine | `127.0.0.1:8081` and `/var/hotels/second/data` |

Start the hotels as in the Linux commands above. nginx then accepts the names. The certificate lists every hotel name. The proxy file shipped for one container, `deploy/container/compose/nginx/default.conf`, has a single `server_name _`. Two hotels are two server names:

```nginx
server {
    listen 80;
    server_name annunziata.internal;
    return 301 https://$host$request_uri;
}

server {
    listen 443 ssl;
    server_name annunziata.internal;

    ssl_certificate     /etc/nginx/certs/fullchain.pem;
    ssl_certificate_key /etc/nginx/certs/privkey.pem;

    location / {
        proxy_pass http://127.0.0.1:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

The second hotel is the same pair of blocks with `server_name second.internal` and `proxy_pass http://127.0.0.1:8081`. Adding a hotel is a new data folder, the next port, a new pair of server blocks, and a name that resolves to this machine.

One container that started every hotel inside itself would still need this proxy, and one restart would stop every hotel.

## Names from a hosts file

Use this when a few desks open the hotels. On each desk, the hosts file maps the names to this machine. One line can carry every hotel. `192.168.1.10` stands for this machine's address.

Windows: `C:\Windows\System32\drivers\etc\hosts`. Linux: `/etc/hosts`.

```text
192.168.1.10 annunziata.internal second.internal
```

The desks then open `https://annunziata.internal/` and `https://second.internal/`. A new hotel is a new word on that line, on every desk. The hosts file has no CNAME. It is only names and addresses.

## Names from local DNS

Use this when every desk should learn a hotel without an edit on that desk. The DNS server aliases each hotel name to this machine's name, and gives that machine name an address. A CNAME is that alias. It points at a name. The A record points at the address.

```text
hotels.internal.        A      192.168.1.10
annunziata.internal.    CNAME  hotels.internal.
second.internal.        CNAME  hotels.internal.
```

The desks use this DNS server. They open the same `https://` names. A new hotel is a new CNAME on the server. nginx and the hotel processes are unchanged from the hosts-file scenario.

## One container per hotel

Docker keeps each hotel running, including after a reboot. One image, one container, one host port, and one folder. Inside the container the hotel is still `/data` and port 8080.

```powershell
docker run -d --restart unless-stopped --name hotel-annunziata -p 127.0.0.1:8080:8080 -v C:\Hotels\Annunziata:/data ghcr.io/<owner>/hoteldruid:blazor
docker run -d --restart unless-stopped --name hotel-second -p 127.0.0.1:8081:8080 -v C:\Hotels\Second:/data ghcr.io/<owner>/hoteldruid:blazor
```

`127.0.0.1` on the published port keeps the hotel port on this computer. Open the ports directly, or point the nginx blocks above at those same ports. The names from a hosts file or from local DNS work in front of these containers the same way they work in front of the Linux processes.

`deploy/container/compose/plain.yml` starts one hotel. It fixes the container name `HotelDruid-api`, so a second `docker compose up` of that file does not add a second hotel. Stop one hotel with `docker stop hotel-second`. The other container keeps running.
