using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDigital201Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Emergency_Contact_r_Employee_Record_Employee_Id",
                schema: "public",
                table: "r_Emergency_Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_r_Employee_Document_r_Employee_Record_Employee_Id",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropIndex(
                name: "IX_r_Emergency_Contact_Employee_Id",
                schema: "public",
                table: "r_Emergency_Contact");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Contact_Details",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "DailyRate",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Department",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Hire_Date",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Last_Name",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "MonthlyBasePay",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Position",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Document_Type",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropColumn(
                name: "File_Url",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.RenameColumn(
                name: "First_Name",
                schema: "public",
                table: "r_Employee_Record",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Employee_Id",
                schema: "public",
                table: "r_Employee_Record",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "Employee_Id",
                schema: "public",
                table: "r_Employee_Document",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "employee_document_id",
                schema: "public",
                table: "r_Employee_Document",
                newName: "DocumentId");

            migrationBuilder.RenameIndex(
                name: "IX_r_Employee_Document_Employee_Id",
                schema: "public",
                table: "r_Employee_Document",
                newName: "IX_r_Employee_Document_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "Phone",
                schema: "public",
                table: "r_Emergency_Contact",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "public",
                table: "r_Emergency_Contact",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Employee_Id",
                schema: "public",
                table: "r_Emergency_Contact",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "public",
                table: "r_Emergency_Contact",
                newName: "EmergencyContactId");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                schema: "public",
                table: "r_Employee_Record",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ErpUserId",
                schema: "public",
                table: "r_Employee_Record",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CivilStatus",
                schema: "public",
                table: "r_Employee_Record",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "public",
                table: "r_Employee_Record",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "public",
                table: "r_Employee_Record",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "r_Employee_Document",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                schema: "public",
                table: "r_Employee_Document",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                schema: "public",
                table: "r_Employee_Document",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                schema: "public",
                table: "r_Employee_Document",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                schema: "public",
                table: "r_Employee_Document",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "public",
                table: "r_Emergency_Contact",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "public",
                table: "r_Emergency_Contact",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "r_Contact_Information",
                schema: "public",
                columns: table => new
                {
                    ContactInformationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TelephoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PresentAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PermanentAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Contact_Information", x => x.ContactInformationId);
                    table.ForeignKey(
                        name: "FK_r_Contact_Information_r_Employee_Record_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Employment_Details",
                schema: "public",
                columns: table => new
                {
                    EmploymentDetailsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Tenure = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EmploymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SalaryGrade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BasePay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Employment_Details", x => x.EmploymentDetailsId);
                    table.ForeignKey(
                        name: "FK_r_Employment_Details_r_Employee_Record_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_r_Emergency_Contact_EmployeeId",
                schema: "public",
                table: "r_Emergency_Contact",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_r_Contact_Information_EmployeeId",
                schema: "public",
                table: "r_Contact_Information",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_r_Employment_Details_EmployeeId",
                schema: "public",
                table: "r_Employment_Details",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_r_Emergency_Contact_r_Employee_Record_EmployeeId",
                schema: "public",
                table: "r_Emergency_Contact",
                column: "EmployeeId",
                principalSchema: "public",
                principalTable: "r_Employee_Record",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Emergency_Contact_r_Employee_Record_EmployeeId",
                schema: "public",
                table: "r_Emergency_Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_r_Employee_Document_r_Employee_Record_EmployeeId",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropTable(
                name: "r_Contact_Information",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Employment_Details",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_r_Emergency_Contact_EmployeeId",
                schema: "public",
                table: "r_Emergency_Contact");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "public",
                table: "r_Emergency_Contact");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "public",
                table: "r_Emergency_Contact");

            migrationBuilder.RenameColumn(
                name: "LastName",
                schema: "public",
                table: "r_Employee_Record",
                newName: "First_Name");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                schema: "public",
                table: "r_Employee_Record",
                newName: "Employee_Id");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                schema: "public",
                table: "r_Employee_Document",
                newName: "Employee_Id");

            migrationBuilder.RenameColumn(
                name: "DocumentId",
                schema: "public",
                table: "r_Employee_Document",
                newName: "employee_document_id");

            migrationBuilder.RenameIndex(
                name: "IX_r_Employee_Document_EmployeeId",
                schema: "public",
                table: "r_Employee_Document",
                newName: "IX_r_Employee_Document_Employee_Id");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                schema: "public",
                table: "r_Emergency_Contact",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "LastName",
                schema: "public",
                table: "r_Emergency_Contact",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                schema: "public",
                table: "r_Emergency_Contact",
                newName: "Employee_Id");

            migrationBuilder.RenameColumn(
                name: "EmergencyContactId",
                schema: "public",
                table: "r_Emergency_Contact",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ErpUserId",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CivilStatus",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contact_Details",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DailyRate",
                schema: "public",
                table: "r_Employee_Record",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Hire_Date",
                schema: "public",
                table: "r_Employee_Record",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Last_Name",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyBasePay",
                schema: "public",
                table: "r_Employee_Record",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "public",
                table: "r_Employee_Document",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Document_Type",
                schema: "public",
                table: "r_Employee_Document",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "File_Url",
                schema: "public",
                table: "r_Employee_Document",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_r_Emergency_Contact_Employee_Id",
                schema: "public",
                table: "r_Emergency_Contact",
                column: "Employee_Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_r_Emergency_Contact_r_Employee_Record_Employee_Id",
                schema: "public",
                table: "r_Emergency_Contact",
                column: "Employee_Id",
                principalSchema: "public",
                principalTable: "r_Employee_Record",
                principalColumn: "Employee_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_r_Employee_Document_r_Employee_Record_Employee_Id",
                schema: "public",
                table: "r_Employee_Document",
                column: "Employee_Id",
                principalSchema: "public",
                principalTable: "r_Employee_Record",
                principalColumn: "Employee_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
