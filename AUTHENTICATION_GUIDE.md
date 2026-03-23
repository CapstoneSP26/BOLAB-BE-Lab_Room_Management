# BookLAB Authentication Flow & JWT Token Guide

## Overview
The BookLAB system implements **JWT-based authentication** with **Google OAuth2** integration. Users can log in via Google, and the system generates JWT tokens for API access.

---

## 1. Authentication Controller

**Location:** [src/BookLAB.API/Controllers/AuthController.cs](src/BookLAB.API/Controllers/AuthController.cs)

### Endpoints:

#### 1.1 **Google Login Initiation**
- **Endpoint:** `GET /api/auth/login/google`
- **Parameters:** `returnUrl` (query string)
- **Description:** Redirects user to Google OAuth consent screen
- **Response:** Redirect to Google authentication

```
GET /api/auth/login/google?returnUrl=https%3A%2F%2Flocalhost%3A5173%2Fdashboard
```

#### 1.2 **Google Login Callback**
- **Endpoint:** `GET /api/auth/login/google/callback`
- **Named Route:** `"GoogleLoginCallback"`
- **Parameters:** `returnUrl` (query string)
- **Description:** OAuth callback endpoint - authenticates with Google, generates JWT token, and stores it in cookie
- **Response:** Redirects to `returnUrl`

**JWT Token Generation for Google Users:**
- Claims included:
  - `Id` (User ID)
  - `Role` (Role ID)
- Expiration: **30 minutes**
- Stored in HTTP-only Cookie: `accessToken`
- Cookie properties:
  - `HttpOnly`: true
  - `Secure`: true
  - `SameSite`: None

#### 1.3 **Sign Out**
- **Endpoint:** `GET /api/auth/sign-out`
- **Description:** Signs out the current user
- **Response:** 
```json
{
  "success": true,
  "message": "Sign out successfully!"
}
```

---

## 2. JWT Token Generation

**File:** [src/BookLAB.Infrastructure/Identity/JwtTokenGenerator.cs](src/BookLAB.Infrastructure/Identity/JwtTokenGenerator.cs)

### Token Configuration:

```csharp
public string GenerateToken(User user)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("name", user.FullName)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),  // 1-hour expiration
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### JWT Configuration:

**appsettings.json:**
```json
{
  "Jwt": {
    "SecretKey": "SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKey",
    "Issuer": "FU Lab Issuer",
    "Audience": "FU Lab Audience"
  }
}
```

### JWT Claims:
| Claim | Type | Description |
|-------|------|-------------|
| `sub` | string | User ID (Guid) |
| `email` | string | User email address |
| `name` | string | User full name |

---

## 3. JWT Token Validation

**Location:** [src/BookLAB.API/Program.cs](src/BookLAB.API/Program.cs#L32)

### Bearer Token Configuration:
```csharp
.AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidIssuer = "FU Lab Issuer",
        ValidAudience = "FU Lab Audience",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKey"))
    };

    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            // Token can be passed as:
            // 1. Authorization Header: "Bearer <token>"
            // 2. Cookie: "accessToken"
            ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
            if (!string.IsNullOrEmpty(accessToken))
                ctx.Token = accessToken;

            return Task.CompletedTask;
        }
    };
});
```

### Default Authentication Scheme:
- **Primary:** JWT Bearer
- **Challenge:** JWT Bearer
- **Sign In:** JWT Bearer

---

## 4. Test Users (Seed Data)

**Location:** [src/BookLAB.Infrastructure/Persistence/SeedData.cs](src/BookLAB.Infrastructure/Persistence/SeedData.cs#L116)

### Available Test Users:

| Email | Name | Role ID | Role | User ID |
|-------|------|---------|------|---------|
| alice@example.edu | Alice Tran | 1 | Admin | 11111111-1111-1111-1111-111111111111 |
| bob@example.edu | Bob Nguyen | 2 | Manager | 22222222-2222-2222-2222-222222222222 |
| carol@example.edu | Carol Le | 3 | Lecturer | 33333333-3333-3333-3333-333333333333 |

### Seed Data Details:
- Campus: Main Campus (ID: 1)
- Created: 2025-01-01, 2025-01-02, 2025-01-03
- IsActive: true
- IsDeleted: false

---

## 5. How to Use JWT Tokens for API Testing

### Option 1: Authorization Header
```
Authorization: Bearer <jwt_token>
```

### Option 2: Cookie
Token is automatically stored in `accessToken` cookie by the `/api/auth/login/google/callback` endpoint.

### Example with curl:
```bash
# Using Authorization header
curl -H "Authorization: Bearer <token>" https://localhost:7256/api/bookings

# Using cookies (if set by callback)
curl -b "accessToken=<token>" https://localhost:7256/api/bookings
```

---

## 6. Authentication Flow Diagram

```
┌─────────────┐
│   Browser   │
└──────┬──────┘
       │ 1. GET /api/auth/login/google?returnUrl=...
       ├────────────────────────────────────────────────────┐
       │                                                    │
       ▼                                        ┌────────────▼──────────┐
   ┌─────────────────┐                         │   AuthController      │
   │  OAuth Screen   │                         │                       │
   │  (Google)       │                         │  1. Validates user    │
   └────────┬────────┘                         │  2. Gets role         │
            │                                  │  3. Generates JWT     │
            │ 2. OAuth Code                    │  4. Sets cookie       │
            │                                  └────────┬──────────────┘
            └──────────────┬──────────────────────────◄──┘
                           │                           
                           ▼
                GET /api/auth/login/google/callback
                        │
                        ▼
              Redirect to returnUrl
              with accessToken cookie
```

---

## 7. Application Layer (MediatR Handler)

**Location:** [src/BookLAB.Application/Features/LoginWithGoogle/LoginWithGoogleHandler.cs](src/BookLAB.Application/Features/LoginWithGoogle/LoginWithGoogleHandler.cs)

**Command:** `LoginWithGoogleCommand`
- Parameters: `IdToken`, `email`, `fullname`, `studentId`

**Interface:** [IJwtTokenGenerator](src/BookLAB.Application/Common/Interfaces/Identity/IJwtTokenGenerator.cs)

---

## 8. CORS Configuration

**Origin:** `https://localhost:5173`
- Allows any header
- Allows any method
- Allows credentials (cookies)

---

## 9. Testing API Endpoints

### With JWT Token:
1. Get JWT token from `/api/auth/login/google/callback`
2. Include token in requests:
   ```
   Authorization: Bearer <token>
   ```

### Protected Endpoints Example:
- `GET /api/attendances/generate-qrcode` - Requires `[Authorize]`
- `POST /api/bookings/add` - Requires `[Authorize]`

### Extract User ID from Claims:
```csharp
if (!Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out Guid userId))
    return Unauthorized();
```

---

## 10. Key Files Summary

| File | Purpose |
|------|---------|
| [AuthController.cs](src/BookLAB.API/Controllers/AuthController.cs) | Authentication endpoints |
| [JwtTokenGenerator.cs](src/BookLAB.Infrastructure/Identity/JwtTokenGenerator.cs) | JWT token generation logic |
| [LoginWithGoogleHandler.cs](src/BookLAB.Application/Features/LoginWithGoogle/LoginWithGoogleHandler.cs) | CQRS handler for Google login |
| [SeedData.cs](src/BookLAB.Infrastructure/Persistence/SeedData.cs) | Test users and seed data |
| [Program.cs](src/BookLAB.API/Program.cs) | JWT Bearer configuration |
| [IJwtTokenGenerator.cs](src/BookLAB.Application/Common/Interfaces/Identity/IJwtTokenGenerator.cs) | JWT interface |

---

## 11. Quick Reference: Testing Authentication

### Step 1: Navigate to Google Login
```
https://localhost:7256/api/auth/login/google?returnUrl=https://localhost:5173/dashboard
```

### Step 2: Complete Google OAuth Flow
- User logs in with Google credentials
- Authorization callback processes the authentication

### Step 3: Use Token in Requests
```bash
curl -H "Authorization: Bearer <token>" \
     -X GET https://localhost:7256/api/bookings
```

### Step 4: Verify Token Claims
- Extract user ID from `sub` claim
- Extract email from `email` claim
- Extract full name from `name` claim

---

**Note:** The current implementation shows that the `LoginWithGoogleHandler` has commented-out validation logic. The actual implementation may need to be reviewed and completed for production use.
