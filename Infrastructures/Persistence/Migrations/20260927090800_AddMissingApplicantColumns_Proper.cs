using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Infrastructures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingApplicantColumns_Proper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Date_Of_Birth\" date NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Experience\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Extension_Name\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Skills\" text NULL;");
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
