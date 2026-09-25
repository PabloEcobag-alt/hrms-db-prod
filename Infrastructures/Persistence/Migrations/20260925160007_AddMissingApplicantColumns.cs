using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Infrastructures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingApplicantColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Date_Of_Birth",
                table: "r_Applicant_Records",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Experience",
                table: "r_Applicant_Records",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension_Name",
                table: "r_Applicant_Records",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "r_Applicant_Records",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date_Of_Birth",
                table: "r_Applicant_Records");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "r_Applicant_Records");

            migrationBuilder.DropColumn(
                name: "Extension_Name",
                table: "r_Applicant_Records");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "r_Applicant_Records");
        }
    }
}
