using Microsoft.EntityFrameworkCore;
using ApiHrm.Infrastructures.Persistence.Analytics;

namespace ApiHrm.Infrastructures.Persistence
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<SummaryReport> SummaryReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Prediction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CandidateId).IsRequired();
                entity.Property(e => e.MatchScore).IsRequired();
                entity.Property(e => e.ScreeningResult).IsRequired();
                entity.Property(e => e.ModelVersion).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            modelBuilder.Entity<SummaryReport>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReportType).IsRequired();
                entity.Property(e => e.ReportDate).IsRequired();
                entity.Property(e => e.SummaryData).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
