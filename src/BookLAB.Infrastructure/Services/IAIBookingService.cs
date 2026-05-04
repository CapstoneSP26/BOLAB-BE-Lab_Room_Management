using BookLAB.Application.Common.Models;

namespace BookLAB.Infrastructure.Services
{

    public interface IAIBookingService
    {
        Task<AIBookingResponse> ParseAndSuggestAsync(string userPrompt, CancellationToken ct = default);
    }
}
