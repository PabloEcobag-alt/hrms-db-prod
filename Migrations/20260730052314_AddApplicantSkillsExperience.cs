using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicantSkillsExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //
            /*
            migrationBuilder.AddColumn<string>(
                name: "Experience",
                schema: "public",
                table: "r_Applicant_Records",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                schema: "public",
                table: "r_Applicant_Records",
                type: "text",
                nullable: true);
                */
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Experience",
                schema: "public",
                table: "r_Applicant_Records");

            migrationBuilder.DropColumn(
                name: "Skills",
                schema: "public",
                table: "r_Applicant_Records");
        }
    }
}
