# Test Incident Reporting Endpoint

## API Endpoint
```
POST /api/incidents/report
```

## Request Body
```json
{
  "scheduleId": "uuid",
  "title": "string (required, max 200 chars)",
  "description": "string (required, max 4000 chars)"
}
```

---

## Test Cases

### Test 1: Validation Error - Missing Title
**Expected:** HTTP 400 with validation error

```powershell
$json = @{
    scheduleId = "550e8400-e29b-41d4-a716-446655440001"
    title = ""
    description = "Test description"
} | ConvertTo-Json

curl.exe -X POST http://localhost:5047/api/incidents/report `
  -H "Content-Type: application/json" `
  -d $json
```

**Expected Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["Title is required"]
  }
}
```

---

### Test 2: Validation Error - Empty ScheduleId
**Expected:** HTTP 400 with validation error

```powershell
$json = @{
    scheduleId = "00000000-0000-0000-0000-000000000000"
    title = "Test Report"
    description = "Test description"
} | ConvertTo-Json

curl.exe -X POST http://localhost:5047/api/incidents/report `
  -H "Content-Type: application/json" `
  -d $json
```

**Expected Response:**
```json
{
  "status": 400,
  "errors": {
    "ScheduleId": ["ScheduleId is required"]
  }
}
```

---

### Test 3: Not Found Error - Schedule Doesn't Exist
**Expected:** HTTP 404 when Schedule not found

```powershell
$json = @{
    scheduleId = "550e8400-e29b-41d4-a716-446655440001"
    title = "Test Report"
    description = "Test description"
} | ConvertTo-Json

curl.exe -X POST http://localhost:5047/api/incidents/report `
  -H "Content-Type: application/json" `
  -d $json
```

**Expected Response:**
```json
{
  "error": "Entity \"Schedule\" (550e8400-e29b-41d4-a716-446655440001) was not found."
}
```

---

### Test 4: Success - Create Report (Need Valid ScheduleId)
**Expected:** HTTP 201 with reportId in response

**Step 1: Get a valid Schedule ID from database**
```bash
psql "host=bolab-database.postgres.database.azure.com port=5432 dbname=postgres user=pgadmin sslmode=require" \
  -c "SELECT \"Id\" FROM public.\"Schedules\" LIMIT 1;"
```

**Step 2: Use the Schedule ID in request**
```powershell
$json = @{
    scheduleId = "VALID-SCHEDULE-ID-HERE"  # Replace with actual ID
    title = "Critical Database Issue"
    description = "Database connection timeout after 2pm UTC"
} | ConvertTo-Json

curl.exe -X POST http://localhost:5047/api/incidents/report `
  -H "Content-Type: application/json" `
  -d $json
```

**Expected Response (HTTP 201):**
```json
{
  "incidentId": "550e8400-e29b-41d4-a716-446655440123"
}
```

**Step 3: Verify in Database**
```sql
SELECT "Id", "ScheduleId", "ReportType", "Description", "IsResolved", "CreatedAt" 
FROM public."Reports" 
WHERE "ReportType" = 1  -- 1 = Incident type
ORDER BY "CreatedAt" DESC 
LIMIT 5;
```

---

## Key Points
✅ `ReportType = 1` represents "Incident"  
✅ `ScheduleId` must exist in the `Schedules` table  
✅ `Title` and `Description` are required and trimmed  
✅ Valid ScheduleIds come from health endpoint or database query  
✅ Response uses HTTP 201 (Created) on success  
✅ Validation errors return HTTP 400 with field-level details  

