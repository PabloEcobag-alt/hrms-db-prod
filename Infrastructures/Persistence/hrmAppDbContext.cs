using Microsoft.EntityFrameworkCore;
using ApiHrm.Domains.Entities;

namespace ApiHrm.Infrastructures.Persistence
{
    public class hrmAppDbContext : DbContext
    {
        public hrmAppDbContext(DbContextOptions<hrmAppDbContext> options) : base(options) {}
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Checklist> Checklists { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Exit> Exits { get; set; }
        public DbSet<EmployeeHistory> EmployeeHistories { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }
        public DbSet<LeaveCode> LeaveCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("public");

            //Employee Documents
            modelBuilder.Entity<Document>()
            .HasOne ( d => d.employee )
            .WithMany ( e => e.Documents )
            .HasForeignKey ( d => d.Employee_ID)
            .OnDelete ( DeleteBehavior.SetNull ); //For future reference, the document will be saved even if the employee has rendered their resignation
        

            //Applicant Documents
            modelBuilder.Entity<Document>()
            .HasOne ( d => d.applicant )
            .WithMany ( a => a.Documents)
            .HasForeignKey (d => d.Applicant_ID)
            .OnDelete ( DeleteBehavior.SetNull ); //For the record, the document will be saved even if the applicant is not hired.
        
            //Checklist
            modelBuilder.Entity<Checklist>()
            .HasOne ( c => c.Applicant )
            .WithMany ( a => a.Checklists )
            .HasForeignKey ( c => c.Applicant_ID )
            .OnDelete ( DeleteBehavior.Cascade );

            //Employee History (Audit Logging)
            modelBuilder.Entity<EmployeeHistory>()
            .HasOne ( h => h.Employee )
            .WithMany ( e => e.EmployeeHistories )
            .HasForeignKey ( h => h.Employee_ID )
            .OnDelete ( DeleteBehavior.Cascade );

            //Leave Balance -> Employee
            modelBuilder.Entity<LeaveBalance>()
            .HasOne ( lb => lb.Employee )
            .WithMany ( e => e.LeaveBalances )
            .HasForeignKey ( lb => lb.Employee_ID )
            .OnDelete ( DeleteBehavior.Cascade );

            //Leave Balance -> Leave Code
            modelBuilder.Entity<LeaveBalance>()
            .HasOne ( lb => lb.LeaveCode )
            .WithMany ( lc => lc.LeaveBalances )
            .HasForeignKey ( lb => lb.Leave_Code_ID )
            .OnDelete ( DeleteBehavior.Restrict );

        }
    }
}