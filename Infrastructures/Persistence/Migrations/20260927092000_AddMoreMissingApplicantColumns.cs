using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Infrastructures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreMissingApplicantColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Date_Of_Birth\" date NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Experience\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Extension_Name\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Skills\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Interview_Notes\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Interview_Date\" date NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Expected_Start_Date\" date NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Probationary_End_Date\" date NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Medical_Document_Completed\" boolean NOT NULL DEFAULT false;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"NBI_Document_Completed\" boolean NOT NULL DEFAULT false;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"XRay_Document_Completed\" boolean NOT NULL DEFAULT false;");
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
