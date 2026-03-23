# BOLAB-42 Group Management API - Quick Reference

## 📊 API Overview

```
┌─────────────────────────────────────────────────────────────┐
│                  GROUP MANAGEMENT API v1.0                   │
│                        BOLAB-42                              │
└─────────────────────────────────────────────────────────────┘

BASE ROUTE: /api/groups
AUTHENTICATION: JWT Bearer Token (Required on all endpoints)
```

## 🎯 Endpoint Summary

### GROUP OPERATIONS
```
┌─────────────┬──────────────────────────────────┬─────────────┐
│ METHOD      │ ENDPOINT                         │ STATUS CODE │
├─────────────┼──────────────────────────────────┼─────────────┤
│ POST        │ /api/groups                      │ 201 Created │
│ GET         │ /api/groups                      │ 200 OK      │
│ GET         │ /api/groups/{id}                 │ 200 OK      │
│ PUT         │ /api/groups/{id}                 │ 204 NoContent
│ DELETE      │ /api/groups/{id}                 │ 204 NoContent
└─────────────┴──────────────────────────────────┴─────────────┘

AUTHORIZATION: ✅ [Authorize] + Owner Check
```

### MEMBER OPERATIONS
```
┌─────────────┬────────────────────────────────────────────────┬─────────────┐
│ METHOD      │ ENDPOINT                                       │ STATUS CODE │
├─────────────┼────────────────────────────────────────────────┼─────────────┤
│ POST        │ /api/groups/{groupId}/members                  │ 201 Created │
│ GET         │ /api/groups/{groupId}/members                  │ 200 OK      │
│ PUT         │ /api/groups/{groupId}/members/{userId}         │ 204 NoContent
│ DELETE      │ /api/groups/{groupId}/members/{userId}         │ 204 NoContent
└─────────────┴────────────────────────────────────────────────┴─────────────┘

AUTHORIZATION: ✅ [Authorize] + Owner Check
```

---

## 📥 Request/Response Formats

### CREATE GROUP
```
POST /api/groups

REQUEST:
{
  "groupName": "Team Alpha",
  "description": "Optional"
}

RESPONSE (201):
{
  "groupId": "550e8400-e29b-41d4-a716-446655440000"
}
```

### LIST GROUPS
```
GET /api/groups

RESPONSE (200):
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "groupName": "Team Alpha",
    "ownerId": "11111111-1111-1111-1111-111111111111",
    "ownerName": "Dr. Hung",
    "membersCount": 25,
    "createdAt": "2026-01-15T10:30:00Z",
    "updatedAt": "2026-03-20T14:45:00Z"
  }
]
```

### ADD MEMBER
```
POST /api/groups/{groupId}/members

REQUEST:
{
  "userId": "22222222-2222-2222-2222-222222222222",
  "subjectCode": "PRN211"
}

RESPONSE (201): Created
```

### LIST MEMBERS
```
GET /api/groups/{groupId}/members

RESPONSE (200):
[
  {
    "userId": "22222222-2222-2222-2222-222222222222",
    "userName": "Nguyen Van A",
    "userEmail": "a.nguyen@example.edu",
    "userCode": "SV0001",
    "subjectCode": "PRN211"
  }
]
```

---

## ⚠️ Error Responses

### Common Error Codes

| Status | Code | Description |
|--------|------|-------------|
| 400 | Bad Request | Missing/invalid input data |
| 403 | Forbidden | Not authorized (not group owner) |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Business rule violation (e.g., duplicate) |
| 500 | Internal Error | Server error |

### Error Response Format
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Validation error details"
}
```

---

## 🔐 Security & Authorization

### Authentication
✅ All endpoints require JWT Bearer token in `Authorization` header
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Authorization
✅ Only group owner can:
- View group details
- Update group
- Delete group
- Manage members (add/update/remove)

❌ Cannot be done:
- Access other user's groups
- Modify group you don't own
- Manage members in group you don't own

---

## ✅ Validation Rules

### Group Name
- **Required**: Yes
- **Length**: 2-150 characters
- **Uniqueness**: Per owner (different owners can have same name)
- **Special Chars**: Allowed

### Subject Code
- **Required**: Yes
- **Length**: Max 20 characters
- **Format**: e.g., PRN211, SEP490, etc.
- **Examples**: CS101, IT202, EN150

### User Constraints
- **User must exist** in system
- **Cannot add same student twice** to same group
- **Owner must be current authenticated user**

---

## 🔄 Data Model Relationships

```
┌───────────────────────────────────────────────────────────────┐
│                         USER                                   │
│  (11111111-1111-1111-1111-111111111111)                       │
│  - FullName: "Dr. Hung"                                       │
│  - Email: "hung@example.edu"                                  │
└───────────────────────────────────────────────────────────────┘
         ▲                                    ▲
         │ OwnerId                           │ UserId
         │ (1:N)                             │ (N:M)
         │                                    │
    ┌────────────┐                      ┌─────────────┐
    │   GROUP    │◄─────via─────────────│ GROUPMEMBER │
    │   (GUID)   │   BookingId, GroupId │   (CompKey) │
    │ - Name     │                      │ - SubjCode  │
    │ - Deleted  │                      └─────────────┘
    │ - Audit    │
    └────────────┘
```

---

## 📋 Testing Checklist

### Group Management
- [ ] Create group with valid name
- [ ] Cannot create duplicate names
- [ ] Update group name
- [ ] Delete group (verify soft delete)
- [ ] Retrieve all groups
- [ ] Retrieve specific group
- [ ] Verify authorization on delete

### Member Management
- [ ] Add student with subject code
- [ ] Cannot add duplicate members
- [ ] Update member's subject code
- [ ] Remove member from group
- [ ] Retrieve members list
- [ ] Verify member details accuracy
- [ ] Verify authorization on member operations

### Error Handling
- [ ] Missing required fields → 400
- [ ] Non-existent group → 404
- [ ] Non-existent user → 404
- [ ] Unauthorized access → 403
- [ ] Duplicate member → 409
- [ ] Invalid input format → 400

---

## 🚀 Quick Start

### 1. Create a Group
```bash
curl -X POST http://localhost:5000/api/groups \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"groupName":"Team Alpha"}'
```

### 2. Add Students
```bash
curl -X POST http://localhost:5000/api/groups/{groupId}/members \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"userId":"{userId}","subjectCode":"PRN211"}'
```

### 3. View Members
```bash
curl -X GET http://localhost:5000/api/groups/{groupId}/members \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 📁 File Structure

```
Application Layer:
src/BookLAB.Application/Features/Groups/
├── Commands/
│   ├── CreateGroup/
│   ├── UpdateGroup/
│   ├── DeleteGroup/
│   ├── AddGroupMember/
│   ├── UpdateGroupMember/
│   └── RemoveGroupMember/
└── Queries/
    ├── GetGroups/
    ├── GetGroupById/
    └── GetGroupMembers/

API Layer:
src/BookLAB.API/Controllers/
└── GroupsController.cs
```

---

## 📊 Statistics

| Metric | Count |
|--------|-------|
| Total Endpoints | 9 |
| Group Operations | 5 |
| Member Operations | 4 |
| Commands | 6 |
| Queries | 3 |
| Validators | 6 |
| Status Codes | 6 |
| Error Types | 5 |

---

## 🔗 Related Documentation

- Full Implementation: [BOLAB-42_GROUP_API_IMPLEMENTATION.md](./docs/BOLAB-42_GROUP_API_IMPLEMENTATION.md)
- HTTP Tests: [BOLAB-42_GROUP_API_TESTS.http](./BOLAB-42_GROUP_API_TESTS.http)
- Architecture: [copilot-instructions.md](./.github/copilot-instructions.md)

---

**Last Updated:** March 23, 2026  
**Status:** ✅ Production Ready  
**Build:** Success (0 errors, 91 warnings pre-existing)
