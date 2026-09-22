using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Infrastructures.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProbationaryEndDateToEmploymentDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Probationary_End_Date",
                schema: "public",
                table: "r_Employment_Details",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Probationary_End_Date",
                schema: "public",
                table: "r_Employment_Details");
        }
    }
}
