using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeInformationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Hire_Date",
                schema: "public",
                table: "r_Employee_Record",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AvatarIndex",
                schema: "public",
                table: "r_Employee_Record",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BloodType",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CivilStatus",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfBirth",
                schema: "public",
                table: "r_Employee_Record",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Department",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ErpUserId",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Application_Date",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Expected_Start_Date",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Hiring_Stage",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Interview_Date",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "Medical_Document_Completed",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NBI_Document_Completed",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "XRay_Document_Completed",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "r_Company_Property",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Employee_Id_Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Id_Issue_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Uniform_Top_Size = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Uniform_Bottom_Size = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Uniform_Shoe_Size = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Uniform_Issue_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Equipment_JSON = table.Column<string>(type: "text", nullable: false),
                    Return_Date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Company_Property", x => x.Id);
                    table.ForeignKey(
                        name: "FK_r_Company_Property_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Document_Status",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Document_Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Is_Completed = table.Column<bool>(type: "boolean", nullable: false),
                    Last_Updated = table.Column<DateOnly>(type: "date", nullable: false),
                    Expiry_Date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Document_Status", x => x.Id);
                    table.ForeignKey(
                        name: "FK_r_Document_Status_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Emergency_Contact",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Relationship = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Emergency_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_r_Emergency_Contact_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "r_Government_Id",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Employee_Id = table.Column<int>(type: "integer", nullable: false),
                    SSS_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PhilHealth_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TIN_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    HDMF_Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Government_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_r_Government_Id_r_Employee_Record_Employee_Id",
                        column: x => x.Employee_Id,
                        principalSchema: "public",
                        principalTable: "r_Employee_Record",
                        principalColumn: "Employee_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_r_Company_Property_Employee_Id",
                schema: "public",
                table: "r_Company_Property",
                column: "Employee_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_r_Document_Status_Employee_Id",
                schema: "public",
                table: "r_Document_Status",
                column: "Employee_Id");

            migrationBuilder.CreateIndex(
                name: "IX_r_Emergency_Contact_Employee_Id",
                schema: "public",
                table: "r_Emergency_Contact",
                column: "Employee_Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_r_Government_Id_Employee_Id",
                schema: "public",
                table: "r_Government_Id",
                column: "Employee_Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "r_Company_Property",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Document_Status",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Emergency_Contact",
                schema: "public");

            migrationBuilder.DropTable(
                name: "r_Government_Id",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "AvatarIndex",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "BloodType",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "CivilStatus",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Department",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "ErpUserId",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "Expected_Start_Date",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.DropColumn(
                name: "Hiring_Stage",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.DropColumn(
                name: "Interview_Date",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.DropColumn(
                name: "Medical_Document_Completed",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.DropColumn(
                name: "NBI_Document_Completed",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.DropColumn(
                name: "Position",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.DropColumn(
                name: "Source",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.DropColumn(
                name: "XRay_Document_Completed",
                schema: "public",
                table: "r_Apllicant_Records");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Hire_Date",
                schema: "public",
                table: "r_Employee_Record",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Application_Date",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
