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

        // Sprint 2 & 3 - Attendance & Shifts
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<MissingLog> MissingLogs { get; set; }
        public DbSet<EmployeeShift> EmployeeShifts { get; set; }
        public DbSet<ShiftDefinition> ShiftDefinitions { get; set; }

        // Sprint 2 - ESS (Leaves & Cash Advance)
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<CashAdvance> CashAdvances { get; set; }

        // Sprint 3 - Payroll
        public DbSet<PayrollRun> PayrollRuns { get; set; }
        public DbSet<EmployeePayrollRecord> EmployeePayrollRecords { get; set; }
        public DbSet<BonusIncentive> BonusIncentives { get; set; }
        public DbSet<Payslip> Payslips { get; set; }

        // Sprint 3 - Statutory Tables
        public DbSet<SssBracket> SssBrackets { get; set; }
        public DbSet<PhilHealthBracket> PhilHealthBrackets { get; set; }
        public DbSet<PagIbigBracket> PagIbigBrackets { get; set; }
        public DbSet<TaxBracket> TaxBrackets { get; set; }

        // Sprint 3 - Payout & Email
        public DbSet<PayoutSummary> PayoutSummaries { get; set; }
        public DbSet<EmailDispatchLog> EmailDispatchLogs { get; set; }

        // Sprint 3 - Documents & Audit
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

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

            //Employee History
            modelBuilder.Entity<EmployeeHistory>()
                .HasOne(eh => eh.Employee)
                .WithMany()
                .HasForeignKey(eh => eh.Employee_ID)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Sprint 2 & 3 =====

            // Attendance Log -> Employee
            modelBuilder.Entity<AttendanceLog>()
                .HasOne(a => a.Employee)
                .WithMany()
                .HasForeignKey(a => a.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Missing Log -> Employee
            modelBuilder.Entity<MissingLog>()
                .HasOne(m => m.Employee)
                .WithMany()
                .HasForeignKey(m => m.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee Shift -> Employee
            modelBuilder.Entity<EmployeeShift>()
                .HasOne(s => s.Employee)
                .WithMany()
                .HasForeignKey(s => s.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee Shift -> ShiftDefinition
            modelBuilder.Entity<EmployeeShift>()
                .HasOne(s => s.ShiftDefinition)
                .WithMany(sd => sd.EmployeeShifts)
                .HasForeignKey(s => s.Shift_Code)
                .OnDelete(DeleteBehavior.Restrict);

            // Leave Request -> Employee
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.Employee)
                .WithMany()
                .HasForeignKey(l => l.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Leave Request -> LeaveType
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.LeaveType)
                .WithMany()
                .HasForeignKey(l => l.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cash Advance -> Employee
            modelBuilder.Entity<CashAdvance>()
                .HasOne(c => c.Employee)
                .WithMany()
                .HasForeignKey(c => c.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee Payroll Record -> Payroll Run
            modelBuilder.Entity<EmployeePayrollRecord>()
                .HasOne(p => p.PayrollRun)
                .WithMany(r => r.EmployeePayrollRecords)
                .HasForeignKey(p => p.Payroll_Run_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee Payroll Record -> Employee
            modelBuilder.Entity<EmployeePayrollRecord>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Bonus Incentive -> Payroll Run
            modelBuilder.Entity<BonusIncentive>()
                .HasOne(b => b.PayrollRun)
                .WithMany(r => r.BonusIncentives)
                .HasForeignKey(b => b.Payroll_Run_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Bonus Incentive -> Employee
            modelBuilder.Entity<BonusIncentive>()
                .HasOne(b => b.Employee)
                .WithMany()
                .HasForeignKey(b => b.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Payslip -> Payroll Run
            modelBuilder.Entity<Payslip>()
                .HasOne(p => p.PayrollRun)
                .WithMany(r => r.Payslips)
                .HasForeignKey(p => p.Payroll_Run_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // Payslip -> Employee
            modelBuilder.Entity<Payslip>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(p => p.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Payout Summary -> Payroll Run
            modelBuilder.Entity<PayoutSummary>()
                .HasOne(p => p.PayrollRun)
                .WithMany(r => r.PayoutSummaries)
                .HasForeignKey(p => p.PayrollRunId)
                .OnDelete(DeleteBehavior.Cascade);

            // Email Dispatch Log -> Payslip
            modelBuilder.Entity<EmailDispatchLog>()
                .HasOne(e => e.Payslip)
                .WithMany()
                .HasForeignKey(e => e.PayslipId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee Document -> Employee
            modelBuilder.Entity<EmployeeDocument>()
                .HasOne(d => d.Employee)
                .WithMany()
                .HasForeignKey(d => d.Employee_Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee Document: expiry_date > issue_date constraint
            modelBuilder.Entity<EmployeeDocument>()
                .ToTable(t => t.HasCheckConstraint(
                    "CK_Employee_Document_Expiry_After_Issue",
                    "\"Expiry_Date\" > \"Issue_Date\""));

        }
    }
}