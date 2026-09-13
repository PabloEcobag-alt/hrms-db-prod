using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Infrastructures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJsonbAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Employee_Document_r_Employee_Record_EmployeeId",
                schema: "public",
                table: "r_Employee_Document");
            /* 
            // Commented out to bypass Schema Drift (Error 42701)
            migrationBuilder.AddColumn<DateOnly>(
                name: "ProbationaryEndDate",
                schema: "public",
                table: "r_Employment_Details",
                type: "date",
                nullable: true);
            */
            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                schema: "public",
                table: "r_Audit_Log",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                schema: "public",
                table: "r_Audit_Log",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValues",
                schema: "public",
                table: "r_Audit_Log",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Operation",
                schema: "public",
                table: "r_Audit_Log",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_r_Employee_Document_r_Employee_Record_EmployeeId",
                schema: "public",
                table: "r_Employee_Document",
                column: "EmployeeId",
                principalSchema: "public",
                principalTable: "r_Employee_Record",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Employee_Document_r_Employee_Record_EmployeeId",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropColumn(
                name: "ProbationaryEndDate",
                schema: "public",
                table: "r_Employment_Details");

            migrationBuilder.DropColumn(
                name: "EntityName",
                schema: "public",
                table: "r_Audit_Log");

            migrationBuilder.DropColumn(
                name: "NewValues",
                schema: "public",
                table: "r_Audit_Log");

            migrationBuilder.DropColumn(
                name: "OldValues",
                schema: "public",
                table: "r_Audit_Log");

            migrationBuilder.DropColumn(
                name: "Operation",
                schema: "public",
                table: "r_Audit_Log");

            migrationBuilder.AddForeignKey(
                name: "FK_r_Employee_Document_r_Employee_Record_EmployeeId",
                schema: "public",
                table: "r_Employee_Document",
                column: "EmployeeId",
                principalSchema: "public",
                principalTable: "r_Employee_Record",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
