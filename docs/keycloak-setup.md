# Keycloak Setup for HotelDruid

This guide covers operational setup of Keycloak as the identity provider for HotelDruid.

## Prerequisites

- Keycloak container is running (via `deploy/compose/keycloak.yml`)
- Admin credentials (from deployment environment)
- HotelDruid API is available at a known URL

## 1. Access Keycloak Admin Console

1. Open `http://localhost:8081` (or your configured Keycloak URL)
2. Click **Administration Console**
3. Log in with admin credentials from `deploy/scenarios/public-acme-keycloak.env`

## 2. Create a Realm for HotelDruid

1. In the left sidebar, hover over **Realms** (top-left dropdown)
2. Click **Create Realm**
3. Name: `HotelDruid`
4. Click **Create**

You now have an isolated realm for all HotelDruid identity.

## 3. Create a Client (OIDC Application)

1. In the left sidebar, go to **Clients**
2. Click **Create client**
3. Fill in:
   - **Client type**: `OpenID Connect`
   - **Client ID**: `HotelDruid-api`
   - **Name**: `HotelDruid API`
4. Click **Next**
5. On **Capability config**, enable:
   - ✅ Client authentication
   - ✅ Authorization
6. Click **Next**
7. On **Login settings**, set **Valid redirect URIs**:
   - `https://hotel.example.com/*` (public)
   - `http://localhost:5001/*` (local development)
   - `http://localhost:8080/*` (container local)
8. Click **Save**

## 4. Retrieve Client Credentials

1. In the Clients list, click **HotelDruid-api**
2. Go to the **Credentials** tab
3. Copy the **Client Secret** (you'll need this for the proxy or app)

Example curl to test credentials:

```bash
curl -X POST http://localhost:8081/realms/HotelDruid/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=HotelDruid-api" \
  -d "client_secret=<CLIENT_SECRET>" \
  -d "grant_type=client_credentials"
```

## 5. Import the desk roles

The realm file `deploy/keycloak/hoteldruid-realm.json` creates the realm, the client `HotelDruid-api`, four groups, and the claim the hotel reads.

In the admin console: **Realm settings** is not required first. From the realm dropdown, **Create realm**, then **Browse** the file and **Create**. On an existing `HotelDruid` realm, use **Partial import** and choose the same file.

Each group carries one realm role. Put a person in the group, or assign the role directly. The client mapper writes that role into the token claim `hoteldruid_roles`.

| Group and role | What that person may do |
| --- | --- |
| `administrator` | The whole hotel, including setup and who may sign in |
| `clerk` | Reservations, guests, arrivals, stays, the cashier, and reports |
| `room-service` | Room status, who is staying, and who is leaving |
| `reports` | Reports only |
| `messages` | The batch inbox, the outbox, and the message cursor. Used by the service client in section 5a, not by a person at the desk |

A person in more than one group keeps every matching area. `reports` is the smallest grant. Without Keycloak in front of the hotel, HotelDruid does not apply these roles: the desk is open, and the user-management menus stay hidden.

## 5a. Service account for the message APIs

The same realm file creates a second client, `HotelDruid-messages`, and a fifth role, `messages`. This client has no browser sign-in. Service accounts are on, and the standard login flow is off. An integration tool asks Keycloak for a token with `grant_type=client_credentials`, then calls the hotel.

Every access token from this client carries `hoteldruid_roles` of `messages`. That role is the batch inbox, the outbox, and the message cursor: read what is ready, post an incoming message, write the time a message was relayed, and read the cursor. It may also call structured inbox processing. The hotel still runs that processing only when deploy or an administrator has turned automation on. The role does not open the desk.

Keycloak creates the client secret when the realm is imported. Copy it from the client's **Credentials** tab. It is not in the realm file and it is not stored by the hotel. The group `messages` exists so a person can be given the same role while testing. A PHP import never proposes this role for a member of staff.

```bash
curl -X POST http://localhost:8081/realms/HotelDruid/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=HotelDruid-messages" \
  -d "client_secret=<CLIENT_SECRET>" \
  -d "grant_type=client_credentials"
```

## 6. Create Users

1. In the left sidebar, go to **Users**
2. Click **Add user**
3. Fill in:
   - **Username**: `john.staff`
   - **Email**: `john@hotel.example.com`
   - **First Name**: `John`
   - **Last Name**: `Staff`
4. Click **Create**
5. Go to the **Credentials** tab
6. Click **Set password**
7. Enter password and toggle **Temporary** off
8. Click **Set password**

Repeat for additional users.

## 7. Assign Roles to Users

1. In the Users list, click the user (e.g., `john.staff`)
2. Go to the **Role mapping** tab
3. Under **Assign roles**, click **Assign role**
4. Filter and select `clerk` (or `administrator`, `room-service`, `reports`)
5. Click **Assign**

The user now has the selected role.

## 8. Configure Scope and Role Mappings (Client)

1. In the Clients list, click **HotelDruid-api**
2. Go to the **Client scopes** tab
3. Click **HotelDruid-api-dedicated**
4. Go to the **Mappers** tab
5. Click **Configure a new mapper** and select **User Roles**
6. Fill in:
   - **Name**: `hoteldruid-roles`
   - **Token Claim Name**: `hoteldruid_roles`
   - **Add to ID token**: ✅
   - **Add to access token**: ✅
   - **Add to userinfo**: ✅
7. Click **Save**

This ensures role information is included in ID tokens.

## 9. Test User Login

### Get a token for a user:

```bash
curl -X POST http://localhost:8081/realms/HotelDruid/protocol/openid-connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "client_id=HotelDruid-api" \
  -d "client_secret=<CLIENT_SECRET>" \
  -d "grant_type=password" \
  -d "username=john.staff" \
  -d "password=<USER_PASSWORD>" \
  -d "scope=openid profile roles"
```

Response includes:

```json
{
  "access_token": "eyJhbGc...",
  "token_type": "Bearer",
  "expires_in": 300,
  "refresh_token": "...",
  "id_token": "eyJhbGc..."
}
```

### Decode the ID token (to verify roles):

Paste the `id_token` at https://jwt.io (for inspection only, not production).

You should see:

```json
{
  "sub": "...",
  "preferred_username": "john.staff",
  "email": "john@hotel.example.com",
  "roles": ["HotelDruid-admin"],
  "iat": 1234567890,
  "exp": 1234567900
}
```

## 10. Configure Proxy Auth (Forward Auth Pattern)

If using a reverse proxy with forward-auth (e.g., Nginx + oauth2-proxy):

1. Deploy an oauth2-proxy sidecar or use your proxy's native OIDC module
2. Configure it with:
   - **Provider**: `oidc`
   - **Client ID**: `HotelDruid-api`
   - **Client Secret**: `<CLIENT_SECRET>` (from step 4)
   - **OIDC Issuer URL**: `http://keycloak:8080/realms/HotelDruid`
   - **Redirect URL**: `https://hotel.example.com/oauth2/callback`
3. Set proxy to forward auth checks for protected paths
4. Map required roles to allow/deny decisions

Example Nginx config with external auth:

```nginx
location / {
    auth_request /auth;
    proxy_pass http://api:8080;
}

location = /auth {
    internal;
    proxy_pass http://oauth2-proxy:4180;
    proxy_pass_request_body off;
    proxy_set_header Content-Length "";
    proxy_set_header X-Original-URL $scheme://$http_host$request_uri;
}
```

## 11. Key URLs Reference

| Purpose | URL |
|---------|-----|
| Admin Console | `http://keycloak:8080/admin` |
| Realm Discovery | `http://keycloak:8080/realms/HotelDruid/.well-known/openid-configuration` |
| Token Endpoint | `http://keycloak:8080/realms/HotelDruid/protocol/openid-connect/token` |
| Userinfo Endpoint | `http://keycloak:8080/realms/HotelDruid/protocol/openid-connect/userinfo` |
| Logout Endpoint | `http://keycloak:8080/realms/HotelDruid/protocol/openid-connect/logout` |

Use these URLs in your application or proxy configuration.

## 12. Troubleshooting

**User login fails:**
- Verify password is set and not temporary
- Check user is assigned at least one role
- Verify client credentials are correct

**Roles not in token:**
- Confirm role mapper is configured on client scope
- Verify user is assigned role in role mapping

**CORS issues:**
- Configure **Web Origins** in client settings to allow browser requests
- Or use a proxy that handles CORS

**Token expired:**
- Keycloak default token lifetime is 5 minutes
- Configure **Token Lifespan** in Realm > Sessions to adjust
- Use refresh tokens to get new access tokens without re-authenticating

## 13. HotelDruid detects this front

HotelDruid treats Keycloak as the sign-in front only when both of these are true:

1. The API process has `Identity:Provider=keycloak` (compose variable `IDENTITY_PROVIDER`).
2. The API can read `{Identity:Issuer}/.well-known/openid-configuration` (compose variable `IDENTITY_ISSUER`).

`GET /api/system/identity-front` returns `configured`, `reachable`, `isFront`, `realm`, `issuer`, and `adminConsoleUrl`. The Blazor menu adds **Identity** only when `isFront` is true. That page states the realm and opens `IDENTITY_ADMIN_CONSOLE_URL` in the browser. User and role changes stay in the Keycloak admin console. The hotel API does not call the Keycloak admin API and does not keep the admin password.

The probe only detects the front. It does not sign a desk user in. Requiring a token is still the reverse proxy's job (section 10).

Use two addresses:

| Variable | Who uses it | Example |
|---|---|---|
| `IDENTITY_ISSUER` | API container, on the compose network | `http://keycloak:8080/realms/HotelDruid` |
| `IDENTITY_ADMIN_CONSOLE_URL` | Browser | `http://localhost:8081/admin/HotelDruid/console` or `https://auth.example.com/admin/HotelDruid/console` |

If the issuer is set but the probe fails, `isFront` stays false and the Identity menu is hidden. Open `/identity` to see the issuer the API tried. A public hostname that the API container cannot reach will fail the probe even when a laptop can open the admin console. Point `IDENTITY_ISSUER` at the address the API can resolve.

The Users, Groups, Privileges, and Sessions pages remain the imported staff register. They name the person on a cash row. They are not the login directory while this front is active.

An internal deployment that does not run `deploy/compose/keycloak.yml` leaves `IDENTITY_PROVIDER=none`. Nothing in the menu offers Keycloak management.

## 14. Next Steps

- Point the reverse proxy at the realm discovery endpoint (section 10) so public traffic requires a token. HotelDruid already probes that document to decide whether to show Identity.
- Configure role-based access control (RBAC) in your reverse proxy
- Set up Keycloak backup/restore procedures
- Document team onboarding process for adding new users/roles

