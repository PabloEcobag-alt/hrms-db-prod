using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class InitialHrmsSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "r_Applicant_Records",
                schema: "public",
                columns: table => new
                {
                    Applicant_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    First_Name = table.Column<string>(type: "text", nullable: false),
                    Last_Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Contact_Details = table.Column<string>(type: "text", nullable: true),
                    Payment_Method = table.Column<string>(type: "text", nullable: true),
                    Resume_URL = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Application_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Apllicant_Records", x => x.Applicant_ID);
                });

            migrationBuilder.CreateTable(
                name: "r_Leave_Code",
                schema: "public",
                columns: table => new
                {
                    Leave_Code_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Leave_Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Leave_Code", x => x.Leave_Code_ID);
                });

            migrationBuilder.CreateTable(
                name: "r_Role",
                schema: "public",
                columns: table => new
                {
                    Role_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Role_Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Role", x => x.Role_ID);
                });

            migrationBuilder.CreateTable(
                name: "r_Checklist",
                schema: "public",
                columns: table => new
                {
                    Checklist_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Applicant_ID = table.Column<int>(type: "integer", nullable: false),
                    Has_NBI = table.Column<bool>(type: "boolean", nullable: false),
                    Has_Medical = table.Column<bool>(type: "boolean", nullable: false),
                    Has_Xray = table.Column<bool>(type: "boolean", nullable: false),
                    has_SSS = table.Column<bool>(type: "boolean", nullable: false),
                    has_PAGIBIG = table.Column<bool>(type: "boolean", nullable: false),
                    has_PhilHealth = table.Column<bool>(type: "boolean", nullable: false),
                    has_TIN = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Checklist", x => x.Checklist_ID);
                    table.ForeignKey(
                        name: "FK_r_Checklist_r_Apllicant_Records_Applicant_ID",
                        column: x => x.Applicant_ID,
                        principalSchema: "public",
                        principalTable: "r_Applicant_Records",
                        principalColumn: "Applicant_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Employee_Record",
                schema: "public",
                columns: table => new
                {
                    Employee_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    First_Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Last_Name = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: false),
                    Hire_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Contact_Details = table.Column<string>(type: "text", nullable: false),
                    Payment_Method = table.Column<string>(type: "text", nullable: false),
                    Role_ID = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Employee_Record", x => x.Employee_Id);
                    table.ForeignKey(
                        name: "FK_r_Employee_Record_r_Role_Role_ID",
                        column: x => x.Role_ID,
                        principalSchema: "public",
                        principalTable: "r_Role",
                        principalColumn: "Role_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Document",
                schema: "public",
                columns: table => new
                {
                    Document_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_ID = table.Column<int>(type: "integer", nullable: true),
                    Applicant_ID = table.Column<int>(type: "integer", nullable: true),
                    file_URL = table.Column<string>(type: "text", nullable: false),
                    Doc_Type = table.Column<string>(type: "text", nullable: false),
                    upload_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Document", x => x.Document_ID);
                    table.ForeignKey(
                        name: "FK_r_Document_r_Apllicant_Records_Applicant_ID",
                        column: x => x.Applicant_ID,
                        principalSchema: "public",
                        principalTable: "r_Applicant_Records",
                        principalColumn: "Applicant_ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_r_Document_r_Employee_Record_Employee_ID",
                        column: x => x.Employee_ID,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "r_Employee_History",
                schema: "public",
                columns: table => new
                {
                    History_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_ID = table.Column<int>(type: "integer", nullable: false),
                    Action_Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Old_Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    New_Value = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Changed_By = table.Column<string>(type: "text", nullable: false),
                    Changed_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Employee_History", x => x.History_ID);
                    table.ForeignKey(
                        name: "FK_r_Employee_History_r_Employee_Record_Employee_ID",
                        column: x => x.Employee_ID,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Exit_Interviews",
                schema: "public",
                columns: table => new
                {
                    Exit_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_ID = table.Column<int>(type: "integer", nullable: false),
                    Reason_For_Leaving = table.Column<string>(type: "text", nullable: false),
                    Interview_Notes = table.Column<string>(type: "text", nullable: false),
                    Interviewed_By = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Exit_Interviews", x => x.Exit_ID);
                    table.ForeignKey(
                        name: "FK_r_Exit_Interviews_r_Employee_Record_Employee_ID",
                        column: x => x.Employee_ID,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Leave_Balance",
                schema: "public",
                columns: table => new
                {
                    Balance_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_ID = table.Column<int>(type: "integer", nullable: false),
                    Leave_Code_ID = table.Column<int>(type: "integer", nullable: false),
                    Allocated_Days = table.Column<int>(type: "integer", nullable: false),
                    Used_Days = table.Column<int>(type: "integer", nullable: false),
                    Pending_Days = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Leave_Balance", x => x.Balance_ID);
                    table.ForeignKey(
                        name: "FK_r_Leave_Balance_r_Employee_Record_Employee_ID",
                        column: x => x.Employee_ID,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_r_Leave_Balance_r_Leave_Code_Leave_Code_ID",
                        column: x => x.Leave_Code_ID,
                        principalSchema: "public",
                        principalTable: "r_Leave_Code",
                        principalColumn: "Leave_Code_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_r_Checklist_Applicant_ID",
                schema: "public",
                table: "r_Checklist",
                column: "Applicant_ID");

            migrationBuilder.CreateIndex(
                name: "IX_r_Document_Applicant_ID",
                schema: "public",
                table: "r_Document",
                column: "Applicant_ID");

            migrationBuilder.CreateIndex(
                name: "IX_r_Document_Employee_ID",
                schema: "public",
                table: "r_Document",
                column: "Employee_ID");

            migrationBuilder.CreateIndex(
                name: "IX_r_Employee_History_Employee_ID",
                schema: "public",
                table: "r_Employee_History",
                column: "Employee_ID");

            migrationBuilder.CreateIndex(
                name: "IX_r_Employee_Record_Role_ID",
                schema: "public",
                table: "r_Employee_Record",
                column: "Role_ID");

            migrationBuilder.CreateIndex(
                name: "IX_r_Exit_Interviews_Employee_ID",
                schema: "public",
                table: "r_Exit_Interviews",
                column: "Employee_ID");

            migrationBuilder.CreateIndex(
                name: "IX_r_Leave_Balance_Employee_ID",
                schema: "public",
                table: "r_Leave_Balance",
                column: "Employee_ID");

            migrationBuilder.CreateIndex(
                name: "IX_r_Leave_Balance_Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance",
                column: "Leave_Code_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "r_Checklist",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Document",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Employee_History",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Exit_Interviews",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Leave_Balance",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Applicant_Records",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Employee_Record",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Leave_Code",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Role",
                schema: "public");
        }
    }
}
