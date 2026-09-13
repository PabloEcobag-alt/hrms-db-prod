using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence.Analytics;
using Applications.Interfaces;

namespace ApiHrm.Infrastructures.Persistence
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AnalyticsDbContext _context;

        public AnalyticsRepository(AnalyticsDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Prediction>> GetAllPredictionsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Predictions
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetPredictionCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Predictions.CountAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Prediction>> GetTopPredictionsAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _context.Predictions
                .AsNoTracking()
                .OrderByDescending(p => p.MatchScore)
                .ThenByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}
