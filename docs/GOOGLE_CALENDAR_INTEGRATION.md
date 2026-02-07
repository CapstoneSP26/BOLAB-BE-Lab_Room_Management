# Google Calendar Integration - BOLAB-27

## Overview
This feature integrates Google Calendar API to automatically sync approved bookings to Google Calendar, providing users with calendar reminders and notifications.

## Features Implemented

### ✅ Domain Layer
- Added `CalendarEventId` property to `Booking` entity

### ✅ Application Layer
- **ICalendarSyncService** interface for calendar operations
- **CalendarEventDto** model for event data transfer
- **SyncToCalendar** Command/Handler/Validator
- **UpdateCalendarEvent** Command/Handler/Validator
- **DeleteCalendarEvent** Command/Handler/Validator

### ✅ Infrastructure Layer
- **GoogleCalendarSyncService** implementation with full CRUD operations
- Service registration in DependencyInjection
- EF Core configuration for `CalendarEventId` column

### ✅ API Layer
- **BookingsController** with 3 endpoints:
  - `POST /api/bookings/{id}/sync-calendar` - Sync approved booking to calendar
  - `PUT /api/bookings/{id}/update-calendar` - Update calendar event
  - `DELETE /api/bookings/{id}/delete-calendar` - Delete calendar event

### ✅ Database
- Migration created and applied: `AddCalendarEventIdToBooking`
- Column added to Bookings table

## Setup Instructions

### 1. Google Cloud Console Setup
1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select existing one
3. Enable **Google Calendar API**
4. Go to **IAM & Admin** > **Service Accounts**
5. Create a new service account with name like `booklab-calendar`
6. Create a JSON key for the service account
7. Download the JSON file

### 2. Configure Credentials
1. Place the downloaded JSON file in `src/BookLAB.API/credentials/`
2. Rename it to `google-calendar-credentials.json`
3. The path is already configured in `appsettings.json`

### 3. Share Calendar with Service Account
1. Copy the service account email from the JSON file (e.g., `booklab-calendar@your-project.iam.gserviceaccount.com`)
2. Open Google Calendar
3. Go to calendar settings
4. Share your calendar with the service account email
5. Give it **"Make changes to events"** permission

### 4. Configuration
Already configured in `appsettings.json`:
```json
{
  "GoogleCalendar": {
    "CalendarId": "primary",
    "CredentialsPath": "credentials/google-calendar-credentials.json"
  }
}
```

## API Endpoints

### 1. Sync Booking to Calendar
**POST** `/api/bookings/{id}/sync-calendar`

Creates a calendar event for an approved booking.

**Request:**
```http
POST /api/bookings/3fa85f64-5717-4562-b3fc-2c963f66afa6/sync-calendar
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Booking synced to Google Calendar successfully",
  "calendarEventId": "abc123xyz",
  "bookingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Error (400 Bad Request):**
```json
{
  "success": false,
  "message": "Only approved bookings can be synced to calendar."
}
```

### 2. Update Calendar Event
**PUT** `/api/bookings/{id}/update-calendar`

Updates an existing calendar event when booking details change.

**Request:**
```http
PUT /api/bookings/3fa85f64-5717-4562-b3fc-2c963f66afa6/update-calendar
```

**Response (204 No Content)**

### 3. Delete Calendar Event
**DELETE** `/api/bookings/{id}/delete-calendar`

Deletes the calendar event when booking is cancelled.

**Request:**
```http
DELETE /api/bookings/3fa85f64-5717-4562-b3fc-2c963f66afa6/delete-calendar
```

**Response (204 No Content)**

## Usage Flow

### Scenario 1: New Booking Approved
1. Admin approves a booking
2. System calls `POST /api/bookings/{id}/sync-calendar`
3. Calendar event is created with:
   - Title: "Lab Booking - [Room Name]"
   - Location: "[Room Name], [Building Name]"
   - Time: Booking start/end time
   - Reminders: 1 day before (email) + 30 minutes before (popup)
4. `CalendarEventId` is saved to booking record

### Scenario 2: Booking Updated
1. Booking details change (time, room, etc.)
2. System calls `PUT /api/bookings/{id}/update-calendar`
3. Calendar event is updated with new information

### Scenario 3: Booking Cancelled
1. Booking is cancelled
2. System calls `DELETE /api/bookings/{id}/delete-calendar`
3. Calendar event is removed
4. `CalendarEventId` is cleared from booking record

## Event Details

Calendar events include:
- **Summary:** Lab Booking - [Room Name]
- **Location:** [Room Name], [Building Name]
- **Description:**
  ```
  Booking Details:
  - Reason: [Booking Reason]
  - Status: [Booking Status]
  - Booking ID: [GUID]
  
  This is an automated booking from BookLAB system.
  ```
- **Reminders:**
  - Email: 1 day before (1440 minutes)
  - Popup: 30 minutes before

## Error Handling

### Common Errors
1. **"Booking not found"** - Invalid booking ID
2. **"Only approved bookings can be synced"** - Booking status is not Approved
3. **"Booking has no associated calendar event"** - Trying to update/delete without CalendarEventId
4. **"Credentials file not found"** - Missing or incorrect credentials path
5. **"Failed to sync to Google Calendar"** - API error or network issue

### Logging
All operations are logged:
- `LogInformation`: Success operations
- `LogWarning`: Business rule violations
- `LogError`: System errors and exceptions

## Testing

### Manual Testing
1. Create a test booking in database
2. Set status to `Approved`
3. Call sync endpoint:
   ```bash
   curl -X POST https://localhost:5001/api/bookings/{id}/sync-calendar
   ```
4. Check Google Calendar for the event
5. Verify `CalendarEventId` is saved in database

### Integration with Booking Workflow
You can automatically trigger calendar sync by:
1. Adding call to `SyncBookingToCalendarCommand` in your booking approval handler
2. Adding call to `UpdateCalendarEventCommand` in your booking update handler
3. Adding call to `DeleteCalendarEventCommand` in your booking cancellation handler

## Security Notes

⚠️ **Important Security Practices:**
1. Never commit `google-calendar-credentials.json` to version control
2. Use environment-specific credentials (Dev/Staging/Production)
3. Regularly rotate service account keys
4. Limit service account permissions to only Calendar API
5. Monitor API usage in Google Cloud Console

## Troubleshooting

### Issue: "Assets file not found"
**Solution:** Run `dotnet restore`

### Issue: Migration fails
**Solution:** Check database connection string and ensure PostgreSQL is running

### Issue: Calendar sync fails
**Solution:** 
- Verify credentials file exists and is valid
- Check service account has calendar access
- Ensure calendar is shared with service account email

### Issue: Build warnings about obsolete API
**Solution:** Already fixed - using `DateTimeDateTimeOffset` instead of deprecated `DateTime`

## Future Enhancements
- [ ] Support for recurring bookings
- [ ] Batch sync multiple bookings
- [ ] Calendar event notifications via webhook
- [ ] Support multiple calendar providers (Outlook, etc.)
- [ ] User-specific calendar sync (OAuth2)

## References
- [Google Calendar API Documentation](https://developers.google.com/calendar/api/guides/overview)
- [Service Account Authentication](https://developers.google.com/identity/protocols/oauth2/service-account)
- [Google.Apis.Calendar.v3 NuGet Package](https://www.nuget.org/packages/Google.Apis.Calendar.v3/)
