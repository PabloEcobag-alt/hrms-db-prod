using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class MakeResumeUrlNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Resume_URL",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Resume_URL",
                schema: "public",
                table: "r_Apllicant_Records",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
