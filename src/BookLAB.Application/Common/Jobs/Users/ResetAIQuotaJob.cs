using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLAB.Application.Common.Jobs.Users
{
    public class ResetAIQuotaJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResetAIQuotaJob> _logger;

        public ResetAIQuotaJob(IUnitOfWork unitOfWork, ILogger<ResetAIQuotaJob> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Execute()
        {
            try
            {
                _logger.LogInformation("Starting ResetAIQuotaJob...");
                
                var users = await _unitOfWork.Repository<User>().Entities
                    .Where(u => u.IsActive && !u.IsDeleted)
                    .ToListAsync();

                foreach (var user in users)
                {
                    user.AIRequestQuota = 20;
                    user.LastAIQuotaReset = DateTimeOffset.UtcNow;
                    _unitOfWork.Repository<User>().Update(user);
                }

                await _unitOfWork.SaveChangesAsync(default);

                _logger.LogInformation("Successfully reset AI quota for {Count} users.", users.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting AI quota.");
                throw;
            }
        }
    }
}
