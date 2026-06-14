using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_PayrollEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Download_Url",
                schema: "public",
                table: "r_Payslip",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Email_Retry_Count",
                schema: "public",
                table: "r_Payslip",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Email_Sent_At",
                schema: "public",
                table: "r_Payslip",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Finalized_At",
                schema: "public",
                table: "r_Payroll_Run",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Finalized_By",
                schema: "public",
                table: "r_Payroll_Run",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Payout_Summary_Json",
                schema: "public",
                table: "r_Payroll_Run",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Days_Worked",
                schema: "public",
                table: "r_Employee_Payroll_Record",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OT_Hours",
                schema: "public",
                table: "r_Employee_Payroll_Record",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax_Deduction",
                schema: "public",
                table: "r_Employee_Payroll_Record",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total_Deductions",
                schema: "public",
                table: "r_Employee_Payroll_Record",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "r_Email_Dispatch_Log",
                schema: "public",
                columns: table => new
                {
                    email_dispatch_log_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PayslipId = table.Column<int>(type: "integer", nullable: false),
                    EmployeeEmail = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Email_Dispatch_Log", x => x.email_dispatch_log_id);
                    table.ForeignKey(
                        name: "FK_r_Email_Dispatch_Log_r_Payslip_PayslipId",
                        column: x => x.PayslipId,
                        principalSchema: "public",
                        principalTable: "r_Payslip",
                        principalColumn: "payslip_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "r_PagIbig_Bracket",
                schema: "public",
                columns: table => new
                {
                    pagibig_bracket_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalaryRangeStart = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalaryRangeEnd = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmployeeShareRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmployerShareRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonthlyContribution = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EffectiveYear = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_PagIbig_Bracket", x => x.pagibig_bracket_id);
                });

            migrationBuilder.CreateTable(
                name: "r_Payout_Summary",
                schema: "public",
                columns: table => new
                {
                    payout_summary_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PayrollRunId = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmployeeCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Payout_Summary", x => x.payout_summary_id);
                    table.ForeignKey(
                        name: "FK_r_Payout_Summary_r_Payroll_Run_PayrollRunId",
                        column: x => x.PayrollRunId,
                        principalSchema: "public",
                        principalTable: "r_Payroll_Run",
                        principalColumn: "payroll_run_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_PhilHealth_Bracket",
                schema: "public",
                columns: table => new
                {
                    philhealth_bracket_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalaryRangeStart = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalaryRangeEnd = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmployeeShareRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmployerShareRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonthlyContribution = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EffectiveYear = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_PhilHealth_Bracket", x => x.philhealth_bracket_id);
                });

            migrationBuilder.CreateTable(
                name: "r_Sss_Bracket",
                schema: "public",
                columns: table => new
                {
                    sss_bracket_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalaryRangeStart = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SalaryRangeEnd = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmployeeShareRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EmployerShareRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonthlyContribution = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EffectiveYear = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Sss_Bracket", x => x.sss_bracket_id);
                });

            migrationBuilder.CreateTable(
                name: "r_Tax_Bracket",
                schema: "public",
                columns: table => new
                {
                    tax_bracket_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnualRangeStart = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AnnualRangeEnd = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    BaseTax = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EffectiveYear = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Tax_Bracket", x => x.tax_bracket_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_r_Email_Dispatch_Log_PayslipId",
                schema: "public",
                table: "r_Email_Dispatch_Log",
                column: "PayslipId");

            migrationBuilder.CreateIndex(
                name: "IX_r_Payout_Summary_PayrollRunId",
                schema: "public",
                table: "r_Payout_Summary",
                column: "PayrollRunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "r_Email_Dispatch_Log",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_PagIbig_Bracket",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Payout_Summary",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_PhilHealth_Bracket",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Sss_Bracket",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Tax_Bracket",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "Download_Url",
                schema: "public",
                table: "r_Payslip");

            migrationBuilder.DropColumn(
                name: "Email_Retry_Count",
                schema: "public",
                table: "r_Payslip");

            migrationBuilder.DropColumn(
                name: "Email_Sent_At",
                schema: "public",
                table: "r_Payslip");

            migrationBuilder.DropColumn(
                name: "Finalized_At",
                schema: "public",
                table: "r_Payroll_Run");

            migrationBuilder.DropColumn(
                name: "Finalized_By",
                schema: "public",
                table: "r_Payroll_Run");

            migrationBuilder.DropColumn(
                name: "Payout_Summary_Json",
                schema: "public",
                table: "r_Payroll_Run");

            migrationBuilder.DropColumn(
                name: "Days_Worked",
                schema: "public",
                table: "r_Employee_Payroll_Record");

            migrationBuilder.DropColumn(
                name: "OT_Hours",
                schema: "public",
                table: "r_Employee_Payroll_Record");

            migrationBuilder.DropColumn(
                name: "Tax_Deduction",
                schema: "public",
                table: "r_Employee_Payroll_Record");

            migrationBuilder.DropColumn(
                name: "Total_Deductions",
                schema: "public",
                table: "r_Employee_Payroll_Record");
        }
    }
}
