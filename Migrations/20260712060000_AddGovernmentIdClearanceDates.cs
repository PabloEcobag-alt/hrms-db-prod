using ApiHrm.Infrastructures.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Migrations
{
    /// <summary>
    /// Adds the NbiClearanceDate and BarangayClearanceDate columns to r_Government_Id.
    /// These properties already existed on the GovernmentId entity but were never
    /// migrated, causing "column does not exist" errors on fresh databases.
    /// Uses IF NOT EXISTS so it is safe on databases that were manually patched.
    /// </summary>
    [DbContext(typeof(hrmAppDbContext))]
    [Migration("20260712060000_AddGovernmentIdClearanceDates")]
    public partial class AddGovernmentIdClearanceDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""public"".""r_Government_Id"" ADD COLUMN IF NOT EXISTS ""NbiClearanceDate"" date NULL;");
            migrationBuilder.Sql(
                @"ALTER TABLE ""public"".""r_Government_Id"" ADD COLUMN IF NOT EXISTS ""BarangayClearanceDate"" date NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE ""public"".""r_Government_Id"" DROP COLUMN IF EXISTS ""BarangayClearanceDate"";");
            migrationBuilder.Sql(
                @"ALTER TABLE ""public"".""r_Government_Id"" DROP COLUMN IF EXISTS ""NbiClearanceDate"";");
        }
    }
}
