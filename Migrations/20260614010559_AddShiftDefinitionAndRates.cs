using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftDefinitionAndRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Shift_Code",
                schema: "public",
                table: "r_Employee_Shift",
                type: "character varying(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "DailyRate",
                schema: "public",
                table: "r_Employee_Record",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyBasePay",
                schema: "public",
                table: "r_Employee_Record",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "r_Shift_Definition",
                schema: "public",
                columns: table => new
                {
                    ShiftCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    StandardHours = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Shift_Definition", x => x.ShiftCode);
                });

            migrationBuilder.CreateIndex(
                name: "IX_r_Employee_Shift_Shift_Code",
                schema: "public",
                table: "r_Employee_Shift",
                column: "Shift_Code");

            migrationBuilder.AddForeignKey(
                name: "FK_r_Employee_Shift_r_Shift_Definition_Shift_Code",
                schema: "public",
                table: "r_Employee_Shift",
                column: "Shift_Code",
                principalSchema: "public",
                principalTable: "r_Shift_Definition",
                principalColumn: "ShiftCode",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Employee_Shift_r_Shift_Definition_Shift_Code",
                schema: "public",
                table: "r_Employee_Shift");

            migrationBuilder.DropTable(
                name: "r_Shift_Definition",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_r_Employee_Shift_Shift_Code",
                schema: "public",
                table: "r_Employee_Shift");

            migrationBuilder.DropColumn(
                name: "DailyRate",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "MonthlyBasePay",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.AlterColumn<string>(
                name: "Shift_Code",
                schema: "public",
                table: "r_Employee_Shift",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)");
        }
    }
}
