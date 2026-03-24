# BookLAB Authentication System - Executive Summary

## Quick Answer: How to Get a JWT Token and Test the API

### 1️⃣ **Start Here: Available Test Users**

The system comes with 3 pre-seeded test users ready to use:

```
Admin User:
  📧 alice@example.edu
  👤 Alice Tran
  🔑 ID: 11111111-1111-1111-1111-111111111111
  
Manager User:
  📧 bob@example.edu
  👤 Bob Nguyen
  🔑 ID: 22222222-2222-2222-2222-222222222222
  
Lecturer User:
  📧 carol@example.edu
  👤 Carol Le
  🔑 ID: 33333333-3333-3333-3333-333333333333
```

---

### 2️⃣ **Generate a JWT Token for Testing**

**Two methods to get a token:**

#### **Method A: Google OAuth Flow (Production)**
```
Step 1: Visit in browser:
https://localhost:7256/api/auth/login/google?returnUrl=https://localhost:5173/dashboard

Step 2: Complete Google login
Step 3: System generates JWT automatically → stored in cookie
```

#### **Method B: Manual Token Creation (Faster for Testing)**

Use [JWT.io](https://jwt.io) online tool:

**Algorithm:** HS256

**Header:**
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload (for Alice - Admin):**
```json
{
  "sub": "11111111-1111-1111-1111-111111111111",
  "email": "alice@example.edu",
  "name": "Alice Tran",
  "iat": 1700000000,
  "exp": 2000000000
}
```

**Secret Key:**
```
SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKey
```

✅ This generates a valid JWT token for testing!

---

### 3️⃣ **Use the Token to Call Protected APIs**

#### **Option A: In Authorization Header**
```bash
curl -X GET https://localhost:7256/api/bookings \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"
```

#### **Option B: In VS Code REST Client (.http file)**

Create a `.http` file:
```http
@baseUrl = https://localhost:7256
@token = eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...YOUR_TOKEN...

### Test Protected Endpoint
GET {{baseUrl}}/api/bookings
Authorization: Bearer {{token}}
Content-Type: application/json
```

#### **Option C: In Postman**

1. Create a variable: `token = YOUR_JWT_TOKEN`
2. Add Authorization Header:
   ```
   Authorization: Bearer {{token}}
   ```
3. Send request

---

### 4️⃣ **Protected Endpoints Examples**

| Endpoint | Method | Requires Auth | Purpose |
|----------|--------|---------------|---------|
| `/api/auth/login/google` | GET | ❌ | Start Google login |
| `/api/auth/login/google/callback` | GET | ❌ | OAuth callback |
| `/api/bookings` | GET | ✅ YES | List bookings |
| `/api/bookings/add` | POST | ✅ YES | Create booking |
| `/api/groups` | GET | ✅ YES | List groups |
| `/api/attendances/generate-qrcode` | GET | ✅ YES | Generate QR code |

---

## 📋 Authentication Flow (How It Works)

```
┌─────────────────────────────────────────────────────────────┐
│                    AUTHENTICATION FLOW                      │
└─────────────────────────────────────────────────────────────┘

1. User Visits Login Endpoint
   └─→ GET /api/auth/login/google?returnUrl=...
   
2. Redirected to Google OAuth
   └─→ User enters Google credentials
   
3. OAuth Callback
   └─→ GET /api/auth/login/google/callback
   
4. JWT Token Generated
   ├─→ Algorithm: HS256
   ├─→ Claims: UserId, Email, FullName
   ├─→ Expiration: 1 hour
   └─→ Stored in: Cookie (accessToken)
   
5. Token Ready for API Calls
   └─→ Authorization: Bearer <token>
   
6. Protected Endpoints Validate Token
   └─→ Check signature
   └─→ Check expiration
   └─→ Extract claims (User ID, Email, etc.)
```

---

## 🔐 JWT Token Details

### Token Claims:
```json
{
  "sub": "user-id-guid",           // Subject (User ID)
  "email": "user@example.edu",     // Email claim
  "name": "Full Name",             // User full name
  "iat": 1700000000,               // Issued at
  "exp": 1700003600                // Expires at (1 hour later)
}
```

### Token Configuration:
```
Secret Key:     "SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKey"
Issuer:         "FU Lab Issuer"
Audience:       "FU Lab Audience"
Algorithm:      HS256 (HMAC SHA-256)
Expiration:     1 hour (3600 seconds)
```

---

## 🗂️ Codebase Structure for Authentication

```
src/BookLAB.API/
├── Controllers/
│   └── AuthController.cs              ← Google login endpoints
└── Program.cs                         ← JWT Bearer configuration

src/BookLAB.Application/
├── Features/LoginWithGoogle/
│   ├── LoginWithGoogleCommand.cs      ← CQRS Command
│   ├── LoginWithGoogleHandler.cs      ← CQRS Handler
│   └── LoginWithGoogleValidator.cs    ← Validation
└── Common/Interfaces/Identity/
    └── IJwtTokenGenerator.cs          ← Token generation interface

src/BookLAB.Infrastructure/
├── Identity/
│   └── JwtTokenGenerator.cs           ← Token generation implementation
├── Services/
│   └── GoogleAuthService.cs           ← Google OAuth verification
└── Persistence/
    └── SeedData.cs                    ← Test users
```

---

## 📝 Quick Reference: File Locations

| What | Where |
|------|-------|
| **Login endpoints** | [AuthController.cs](src/BookLAB.API/Controllers/AuthController.cs) |
| **JWT generation** | [JwtTokenGenerator.cs](src/BookLAB.Infrastructure/Identity/JwtTokenGenerator.cs) |
| **JWT validation** | [Program.cs](src/BookLAB.API/Program.cs) |
| **Test users** | [SeedData.cs](src/BookLAB.Infrastructure/Persistence/SeedData.cs) |
| **Google OAuth** | [GoogleAuthService.cs](src/BookLAB.Infrastructure/Services/GoogleAuthService.cs) |
| **Login handlers** | [LoginWithGoogleHandler.cs](src/BookLAB.Application/Features/LoginWithGoogle/LoginWithGoogleHandler.cs) |

---

## ⚡ 30-Second Quick Start

1. **Copy a JWT token template from JWT.io:**
   - Use Algorithm: HS256
   - Add payload with user ID from seed data
   - Use the secret key from appsettings.json

2. **Test the API:**
   ```bash
   curl -X GET https://localhost:7256/api/bookings \
     -H "Authorization: Bearer YOUR_JWT_TOKEN"
   ```

3. **Include token in every request to protected endpoints**

---

## 🎯 Most Important Code Regions

### 1. Where Tokens Are Generated
**File:** `src/BookLAB.Infrastructure/Identity/JwtTokenGenerator.cs`
```csharp
public string GenerateToken(User user)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("name", user.FullName)
    };
    // ... token creation ...
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### 2. Where Tokens Are Validated
**File:** `src/BookLAB.API/Program.cs`
```csharp
.AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"])),
        ValidIssuer = _config["Jwt:Issuer"],
        ValidAudience = _config["Jwt:Audience"]
    };
});
```

### 3. Where Tokens Are Used
**File:** Any protected Controller action
```csharp
[Authorize]  // ← This attribute requires a valid JWT token
public async Task<IActionResult> GetBookings()
{
    // Extract user ID from token claims
    var userId = Guid.Parse(HttpContext.User.FindFirst("Id")?.Value);
    // ... rest of logic ...
}
```

---

## 🔍 Accessing User Information from Token

In any protected endpoint, extract user info from claims:

```csharp
// Get User ID from token
var userId = HttpContext.User.FindFirst("sub")?.Value;

// Get Email from token
var email = HttpContext.User.FindFirst("email")?.Value;

// Get Full Name from token
var fullName = HttpContext.User.FindFirst("name")?.Value;

// Parse GUID properly
if (!Guid.TryParse(HttpContext.User.FindFirst("Id")?.Value, out Guid userId))
    return Unauthorized();
```

---

## ✅ Verification Checklist

When testing authentication, verify:

- [ ] JWT token is generated correctly
- [ ] Token includes required claims (sub, email, name)
- [ ] Token is included in Authorization header
- [ ] Token hasn't expired (1-hour expiration)
- [ ] Secret key in token matches appsettings.json
- [ ] Protected endpoints reject requests without token
- [ ] Protected endpoints accept requests with valid token
- [ ] User ID can be extracted from token claims
- [ ] CORS allows credentials for cookie-based auth

---

## 📚 Additional Documentation

For more details, see:
1. **Full Authentication Guide:** [AUTHENTICATION_GUIDE.md](AUTHENTICATION_GUIDE.md)
2. **Testing Guide:** [AUTHENTICATION_TESTING.md](AUTHENTICATION_TESTING.md)
3. **HTTP Test Examples:** [BOLAB-42_GROUP_API_TESTS.http](BOLAB-42_GROUP_API_TESTS.http)

---

**Last Updated:** 2026-03-23  
**API Version:** .NET 10.0  
**Database:** PostgreSQL
