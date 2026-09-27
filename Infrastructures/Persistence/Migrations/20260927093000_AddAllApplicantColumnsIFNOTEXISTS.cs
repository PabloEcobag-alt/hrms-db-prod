using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Infrastructures.Persistence.Migrations
{
    public partial class AddAllApplicantColumnsIFNOTEXISTS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Application_Date\" date NOT NULL DEFAULT CURRENT_DATE;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Contact_Details\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Email\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"First_Name\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Hiring_Stage\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Last_Name\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Middle_Name\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Mobile\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Payment_Method\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Position\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Resume_URL\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Source\" text NULL;");
            migrationBuilder.Sql("ALTER TABLE \"r_Applicant_Records\" ADD COLUMN IF NOT EXISTS \"Status\" text NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
