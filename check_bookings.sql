-- SQL query to check booking dates
SELECT 
    COUNT(*) as TotalBookings,
    MIN(EXTRACT(YEAR FROM "CreatedAt")) as MinYear,
    MAX(EXTRACT(YEAR FROM "CreatedAt")) as MaxYear,
    MIN(EXTRACT(MONTH FROM "CreatedAt")) as MinMonth,
    MAX(EXTRACT(MONTH FROM "CreatedAt")) as MaxMonth,
    MIN("CreatedAt") as OldestBooking,
    MAX("CreatedAt") as NewestBooking
FROM "Bookings";

-- Check booking status distribution
SELECT 
    "BookingStatus",
    COUNT(*) as Count,
    MIN("CreatedAt") as OldestInStatus,
    MAX("CreatedAt") as NewestInStatus
FROM "Bookings"
GROUP BY "BookingStatus"
ORDER BY "BookingStatus";

-- Check if bookings are in the correct date range
SELECT 
    EXTRACT(YEAR FROM "CreatedAt") as Year,
    EXTRACT(MONTH FROM "CreatedAt") as Month,
    COUNT(*) as BookingCount,
    "BookingStatus"
FROM "Bookings"
GROUP BY 
    EXTRACT(YEAR FROM "CreatedAt"),
    EXTRACT(MONTH FROM "CreatedAt"),
    "BookingStatus"
ORDER BY Year DESC, Month DESC;
