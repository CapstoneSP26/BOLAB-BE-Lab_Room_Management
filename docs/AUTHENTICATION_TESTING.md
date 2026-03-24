# BookLAB API Testing - Authentication Quick Start

## Test Users (Available in Database Seed)

```
User 1 (Admin):
- Email: alice@example.edu
- Name: Alice Tran
- ID: 11111111-1111-1111-1111-111111111111
- Role ID: 1 (Admin)

User 2 (Manager):
- Email: bob@example.edu
- Name: Bob Nguyen
- ID: 22222222-2222-2222-2222-222222222222
- Role ID: 2 (Manager)

User 3 (Lecturer):
- Email: carol@example.edu
- Name: Carol Le
- ID: 33333333-3333-3333-3333-333333333333
- Role ID: 3 (Lecturer)
```

---

## JWT Token Structure

**Payload Example:**
```json
{
  "sub": "11111111-1111-1111-1111-111111111111",
  "email": "alice@example.edu",
  "name": "Alice Tran",
  "iat": 1700000000,
  "exp": 1700003600
}
```

**Token Expiration:** 1 hour (from `JwtTokenGenerator.cs`)

---

## How to Get a JWT Token

### Method 1: Via Google OAuth (Browser)
```
GET https://localhost:7256/api/auth/login/google?returnUrl=https://localhost:5173/dashboard
```

This will:
1. Redirect to Google login
2. Get user info from Google
3. Generate JWT token
4. Store in `accessToken` cookie

### Method 2: Generate Token Manually (For Testing)

Since the test users are seeded in the database, you can manually create a JWT token:

**Using JWT.io (Online Tool):**
1. Go to https://jwt.io
2. Use these credentials:
   - **Header (Algorithm):** HS256
   - **Secret Key:** `SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKey`
   - **Issuer:** `FU Lab Issuer`
   - **Audience:** `FU Lab Audience`

3. **Payload Example (for Alice):**
```json
{
  "sub": "11111111-1111-1111-1111-111111111111",
  "email": "alice@example.edu",
  "name": "Alice Tran",
  "iat": 1700000000,
  "exp": 2000000000
}
```

---

## Using JWT Tokens in API Requests

### Option 1: Authorization Header
```
GET /api/bookings HTTP/1.1
Host: localhost:7256
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

### Option 2: Cookie (Automatically Set)
```
GET /api/bookings HTTP/1.1
Host: localhost:7256
Cookie: accessToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

---

## Example API Calls with curl

### 1. Get Bookings (Requires Auth)
```bash
TOKEN="your_jwt_token_here"

curl -X GET \
  https://localhost:7256/api/bookings \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
```

### 2. Add Schedule (Requires Auth)
```bash
TOKEN="your_jwt_token_here"

curl -X POST \
  https://localhost:7256/api/bookings/add \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "lecturerId": "22222222-2222-2222-2222-222222222222",
    "labRoomId": 1,
    "slotTypeId": 1,
    "scheduleType": 0
  }'
```

### 3. Generate Attendance QR Code (Requires Auth)
```bash
TOKEN="your_jwt_token_here"

curl -X GET \
  "https://localhost:7256/api/attendances/generate-qrcode?scheduleId=27272727-2727-2727-2727-272727272727&isCheckIn=true" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
```

---

## Using VS Code REST Client (.http files)

**File:** [BOLAB-42_GROUP_API_TESTS.http](BOLAB-42_GROUP_API_TESTS.http)

### Example .http Request with Token:

```http
@baseUrl = https://localhost:7256
@token = your_jwt_token_here

### Get Bookings
GET {{baseUrl}}/api/bookings
Authorization: Bearer {{token}}
Content-Type: application/json

### Add Schedule
POST {{baseUrl}}/api/bookings/add
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "lecturerId": "22222222-2222-2222-2222-222222222222",
  "labRoomId": 1,
  "slotTypeId": 1,
  "scheduleType": 0
}
```

---

## Testing Protected Endpoints

### Endpoints that Require `[Authorize]` Attribute:

| Endpoint | Method | Purpose |
|----------|--------|---------|
| /api/attendances/generate-qrcode | GET | Generate QR code for attendance |
| /api/attendances/scan-qrcode | POST | Scan QR code for attendance |
| /api/bookings/add | POST | Add new schedule/booking |
| /api/groups/* | GET/POST | Group management |
| /api/schedules/* | GET | Schedule management |

### How to Extract User Info from Token:

In controllers, extract claims from JWT:
```csharp
// Get User ID from token
if (!Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out Guid userId))
    return Unauthorized();

// Get Email from token
var email = HttpContext.User.FindFirst("email")?.Value;

// Get Full Name from token
var fullName = HttpContext.User.FindFirst("name")?.Value;
```

---

## Troubleshooting

### Issue: "401 Unauthorized"
**Solution:** 
- Verify JWT token is valid
- Check token hasn't expired (1-hour expiration)
- Ensure token is in `Authorization: Bearer <token>` format
- Check JWT secret key matches in appsettings.json

### Issue: "Invalid token signature"
**Solution:**
- Verify the secret key is correct
- Make sure issuer/audience match configuration

### Issue: Token accepted but claims are wrong
**Solution:**
- Verify the User ID exists in seed data
- Check that roles are assigned in UserRole table

### Issue: Cookie not being set
**Solution:**
- Ensure browser is using HTTPS (cookies require Secure flag)
- Check CORS settings allow credentials
- Verify SameSite=None is set correctly

---

## JWT Configuration (appsettings.json)

```json
{
  "Jwt": {
    "SecretKey": "SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKey",
    "Issuer": "FU Lab Issuer",
    "Audience": "FU Lab Audience"
  },
  "Authentication": {
    "Google": {
      "ClientId": "828810744316-9p5pe33sc8fhills9galrf6hh87j5i45.apps.googleusercontent.com",
      "ClientSecret": "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"
    }
  }
}
```

---

## Key Files Reference

- **AuthController:** [src/BookLAB.API/Controllers/AuthController.cs](src/BookLAB.API/Controllers/AuthController.cs)
- **JWT Generator:** [src/BookLAB.Infrastructure/Identity/JwtTokenGenerator.cs](src/BookLAB.Infrastructure/Identity/JwtTokenGenerator.cs)
- **JWT Config:** [src/BookLAB.API/Program.cs](src/BookLAB.API/Program.cs#L32)
- **Test Users:** [src/BookLAB.Infrastructure/Persistence/SeedData.cs](src/BookLAB.Infrastructure/Persistence/SeedData.cs#L116)

---

## Manual JWT Creation Script (PowerShell)

```powershell
# Install JWT library first:
# dotnet add package System.IdentityModel.Tokens.Jwt

$secret = "SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKey"
$issuer = "FU Lab Issuer"
$audience = "FU Lab Audience"
$userId = "11111111-1111-1111-1111-111111111111"
$email = "alice@example.edu"
$fullName = "Alice Tran"

# (Code to generate JWT token programmatically)
Write-Host "Use JWT.io to manually create a token with above parameters"
```

---

## Next Steps

1. ✅ Understand JWT structure and claims
2. ✅ Get a valid JWT token (via Google or manually)
3. ✅ Include token in API requests
4. ✅ Test protected endpoints
5. ✅ Extract user info from claims in controllers
