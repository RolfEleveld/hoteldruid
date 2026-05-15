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

## 5. Create Roles

1. In the left sidebar, go to **Roles**
2. Click **Create role**
3. Create the following roles:

   - **HotelDruid-admin**: Full access to all operations
   - **HotelDruid-staff**: Can view bookings, manage check-ins
   - **HotelDruid-guest**: Read-only access to own reservations

Example (repeat for each role):
- Name: `HotelDruid-admin`
- Description: `Full HotelDruid access`
- Click **Save**

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
4. Filter and select `HotelDruid-admin` (or appropriate role)
5. Click **Assign**

The user now has the selected role.

## 8. Configure Scope and Role Mappings (Client)

1. In the Clients list, click **HotelDruid-api**
2. Go to the **Client scopes** tab
3. Click **HotelDruid-api-dedicated**
4. Go to the **Mappers** tab
5. Click **Configure a new mapper** and select **User Roles**
6. Fill in:
   - **Name**: `user_roles`
   - **Token Claim Name**: `roles`
   - **Add to ID token**: ✅
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

## 13. Next Steps

- Integrate app/proxy with Keycloak's OIDC discovery endpoint
- Configure role-based access control (RBAC) in your reverse proxy
- Set up Keycloak backup/restore procedures
- Document team onboarding process for adding new users/roles

