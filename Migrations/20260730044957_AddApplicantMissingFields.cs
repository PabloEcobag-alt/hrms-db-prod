using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicantMissingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                schema: "public",
                table: "r_Applicant_Records",
                newName: "Mobile");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date_Of_Birth",
                schema: "public",
                table: "r_Applicant_Records",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Extension_Name",
                schema: "public",
                table: "r_Applicant_Records",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date_Of_Birth",
                schema: "public",
                table: "r_Applicant_Records");

            migrationBuilder.DropColumn(
                name: "Extension_Name",
                schema: "public",
                table: "r_Applicant_Records");

            migrationBuilder.RenameColumn(
                name: "Mobile",
                schema: "public",
                table: "r_Applicant_Records",
                newName: "Phone");
        }
    }
}
