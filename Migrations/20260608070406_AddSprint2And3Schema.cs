using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class AddSprint2And3Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Employee_History_r_Employee_Record_Employee_ID",
                schema: "public",
                table: "r_Employee_History");

            migrationBuilder.DropForeignKey(
                name: "FK_r_Leave_Balance_r_Leave_Code_Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance");

            migrationBuilder.CreateTable(
                name: "r_Attendance_Log",
                schema: "public",
                columns: table => new
                {
                    attendance_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Log_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time_In = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Time_Out = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Log_Source = table.Column<string>(type: "text", nullable: false),
                    Is_Manual_Override = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Attendance_Log", x => x.attendance_id);
                    table.ForeignKey(
                        name: "FK_r_Attendance_Log_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "r_Audit_Log",
                schema: "public",
                columns: table => new
                {
                    audit_log_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    User_Id = table.Column<string>(type: "text", nullable: false),
                    User_Role = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Module = table.Column<string>(type: "text", nullable: false),
                    Record_Id = table.Column<string>(type: "text", nullable: false),
                    Record_Label = table.Column<string>(type: "text", nullable: false),
                    Ip_Address = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Audit_Log", x => x.audit_log_id);
                });

            migrationBuilder.CreateTable(
                name: "r_Cash_Advance",
                schema: "public",
                columns: table => new
                {
                    cash_advance_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Amount_Requested = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Cash_Advance", x => x.cash_advance_id);
                    table.ForeignKey(
                        name: "FK_r_Cash_Advance_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "r_Employee_Document",
                schema: "public",
                columns: table => new
                {
                    employee_document_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Document_Type = table.Column<string>(type: "text", nullable: false),
                    Issue_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Expiry_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    File_Url = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Employee_Document", x => x.employee_document_id);
                    table.CheckConstraint("CK_Employee_Document_Expiry_After_Issue", "\"Expiry_Date\" > \"Issue_Date\"");
                    table.ForeignKey(
                        name: "FK_r_Employee_Document_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "r_Employee_Shift",
                schema: "public",
                columns: table => new
                {
                    shift_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Shift_Code = table.Column<string>(type: "text", nullable: false),
                    Effective_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    End_Date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Employee_Shift", x => x.shift_id);
                    table.ForeignKey(
                        name: "FK_r_Employee_Shift_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "r_Leave_Request",
                schema: "public",
                columns: table => new
                {
                    leave_request_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Leave_Code_Id = table.Column<int>(type: "integer", nullable: false),
                    Start_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    End_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Leave_Request", x => x.leave_request_id);
                    table.ForeignKey(
                        name: "FK_r_Leave_Request_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_r_Leave_Request_r_Leave_Code_Leave_Code_Id",
                        column: x => x.Leave_Code_Id,
                        principalSchema: "public",
                        principalTable: "r_Leave_Code",
                        principalColumn: "Leave_Code_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "r_Missing_Log",
                schema: "public",
                columns: table => new
                {
                    missing_log_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Log_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Missing_Log", x => x.missing_log_id);
                    table.ForeignKey(
                        name: "FK_r_Missing_Log_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "r_Payroll_Run",
                schema: "public",
                columns: table => new
                {
                    payroll_run_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CutOff_Start_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CutOff_End_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Payroll_Run", x => x.payroll_run_id);
                });

            migrationBuilder.CreateTable(
                name: "r_Bonus_Incentive",
                schema: "public",
                columns: table => new
                {
                    bonus_incentive_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Payroll_Run_Id = table.Column<int>(type: "integer", nullable: false),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Bonus_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Incentive_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Bonus_Incentive", x => x.bonus_incentive_id);
                    table.ForeignKey(
                        name: "FK_r_Bonus_Incentive_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_r_Bonus_Incentive_r_Payroll_Run_Payroll_Run_Id",
                        column: x => x.Payroll_Run_Id,
                        principalSchema: "public",
                        principalTable: "r_Payroll_Run",
                        principalColumn: "payroll_run_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Employee_Payroll_Record",
                schema: "public",
                columns: table => new
                {
                    payroll_record_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Payroll_Run_Id = table.Column<int>(type: "integer", nullable: false),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Basic_Pay = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    OT_Pay = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Sss_Deduction = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PhilHealth_Deduction = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PagIbig_Deduction = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Net_Pay = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Employee_Payroll_Record", x => x.payroll_record_id);
                    table.ForeignKey(
                        name: "FK_r_Employee_Payroll_Record_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_r_Employee_Payroll_Record_r_Payroll_Run_Payroll_Run_Id",
                        column: x => x.Payroll_Run_Id,
                        principalSchema: "public",
                        principalTable: "r_Payroll_Run",
                        principalColumn: "payroll_run_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Payslip",
                schema: "public",
                columns: table => new
                {
                    payslip_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Payroll_Run_Id = table.Column<int>(type: "integer", nullable: false),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Payout_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Pdf_Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Payslip", x => x.payslip_id);
                    table.ForeignKey(
                        name: "FK_r_Payslip_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_r_Payslip_r_Payroll_Run_Payroll_Run_Id",
                        column: x => x.Payroll_Run_Id,
                        principalSchema: "public",
                        principalTable: "r_Payroll_Run",
                        principalColumn: "payroll_run_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_r_Attendance_Log_Employee_Id",
                schema: "public",
                table: "r_Attendance_Log",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Bonus_Incentive_Employee_Id",
                schema: "public",
                table: "r_Bonus_Incentive",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Bonus_Incentive_Payroll_Run_Id",
                schema: "public",
                table: "r_Bonus_Incentive",
                column: "Payroll_Run_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Cash_Advance_Employee_Id",
                schema: "public",
                table: "r_Cash_Advance",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Employee_Document_Employee_Id",
                schema: "public",
                table: "r_Employee_Document",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Employee_Payroll_Record_Employee_Id",
                schema: "public",
                table: "r_Employee_Payroll_Record",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Employee_Payroll_Record_Payroll_Run_Id",
                schema: "public",
                table: "r_Employee_Payroll_Record",
                column: "Payroll_Run_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Employee_Shift_Employee_Id",
                schema: "public",
                table: "r_Employee_Shift",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Leave_Request_Employee_Id",
                schema: "public",
                table: "r_Leave_Request",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Leave_Request_Leave_Code_Id",
                schema: "public",
                table: "r_Leave_Request",
                column: "Leave_Code_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Missing_Log_Employee_Id",
                schema: "public",
                table: "r_Missing_Log",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Payslip_Employee_Id",
                schema: "public",
                table: "r_Payslip",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Payslip_Payroll_Run_Id",
                schema: "public",
                table: "r_Payslip",
                column: "Payroll_Run_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_r_Employee_History_r_Employee_Record_Employee_ID",
                schema: "public",
                table: "r_Employee_History",
                column: "Employee_ID",
                principalSchema: "public",
                principalTable: "r_Employee_Record",
                principalColumn: "Employee_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_r_Leave_Balance_r_Leave_Code_Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance",
                column: "Leave_Code_ID",
                principalSchema: "public",
                principalTable: "r_Leave_Code",
                principalColumn: "Leave_Code_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Employee_History_r_Employee_Record_Employee_ID",
                schema: "public",
                table: "r_Employee_History");

            migrationBuilder.DropForeignKey(
                name: "FK_r_Leave_Balance_r_Leave_Code_Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance");

            migrationBuilder.DropTable(
                name: "r_Attendance_Log",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Audit_Log",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Bonus_Incentive",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Cash_Advance",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Employee_Document",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Employee_Payroll_Record",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Employee_Shift",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Leave_Request",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Missing_Log",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Payslip",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Payroll_Run",
                schema: "public");

            migrationBuilder.AddForeignKey(
                name: "FK_r_Employee_History_r_Employee_Record_Employee_ID",
                schema: "public",
                table: "r_Employee_History",
                column: "Employee_ID",
                principalSchema: "public",
                principalTable: "r_Employee_Record",
                principalColumn: "Employee_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_r_Leave_Balance_r_Leave_Code_Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance",
                column: "Leave_Code_ID",
                principalSchema: "public",
                principalTable: "r_Leave_Code",
                principalColumn: "Leave_Code_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
