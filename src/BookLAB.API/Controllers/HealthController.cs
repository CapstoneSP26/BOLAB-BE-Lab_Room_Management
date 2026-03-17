using BookLAB.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly BookLABDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(BookLABDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                return StatusCode(503, new
                {
                    Status = "Unhealthy",
                    Error = "Cannot connect to database"
                });
            }

            var stats = new
            {
                Status = "Healthy",
                Database = "Connected ✅",
                DatabaseName = _context.Database.GetDbConnection().Database,
                Tables = new
                {
                    Users = await _context.Users.CountAsync(cancellationToken),
                    Buildings = await _context.Buildings.CountAsync(cancellationToken),
                    LabRooms = await _context.LabRooms.CountAsync(cancellationToken),
                    Bookings = await _context.Bookings.CountAsync(cancellationToken)
                },
                Timestamp = DateTime.UtcNow
            };

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new
            {
                Status = "Unhealthy",
                Error = ex.Message
            });
        }
    }

    [HttpGet("migrations")]
    public async Task<IActionResult> CheckMigrations(CancellationToken cancellationToken)
    {
        try
        {
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken);

            return Ok(new
            {
                Status = "OK",
                PendingMigrations = pendingMigrations.ToList(),
                AppliedMigrations = appliedMigrations.ToList(),
                TotalApplied = appliedMigrations.Count(),
                NeedsMigration = pendingMigrations.Any()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration check failed");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
