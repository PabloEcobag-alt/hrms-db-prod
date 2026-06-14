using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api_hrm.Migrations
{
    /// <inheritdoc />
    public partial class Phase1ProductionUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Leave_Balance_r_Leave_Code_Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance");

            migrationBuilder.DropForeignKey(
                name: "FK_r_Leave_Request_r_Leave_Code_Leave_Code_Id",
                schema: "public",
                table: "r_Leave_Request");

            migrationBuilder.DropTable(
                name: "r_Leave_Code",
                schema: "public");

            migrationBuilder.RenameColumn(
                name: "Leave_Code_Id",
                schema: "public",
                table: "r_Leave_Request",
                newName: "LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_r_Leave_Request_Leave_Code_Id",
                schema: "public",
                table: "r_Leave_Request",
                newName: "IX_r_Leave_Request_LeaveTypeId");

            migrationBuilder.RenameColumn(
                name: "Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance",
                newName: "LeaveTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_r_Leave_Balance_Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance",
                newName: "IX_r_Leave_Balance_LeaveTypeId");

            migrationBuilder.RenameColumn(
                name: "Payment_Method",
                schema: "public",
                table: "r_Employee_Record",
                newName: "PaymentMethod");

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                schema: "public",
                table: "r_Employee_Record",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                schema: "public",
                table: "r_Employee_Document",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                schema: "public",
                table: "r_Employee_Document",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VerifiedByUserId",
                schema: "public",
                table: "r_Employee_Document",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "r_Leave_Type",
                schema: "public",
                columns: table => new
                {
                    LeaveTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeaveTypeName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Leave_Type", x => x.LeaveTypeId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_r_Leave_Balance_r_Leave_Type_LeaveTypeId",
                schema: "public",
                table: "r_Leave_Balance",
                column: "LeaveTypeId",
                principalSchema: "public",
                principalTable: "r_Leave_Type",
                principalColumn: "LeaveTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_r_Leave_Request_r_Leave_Type_LeaveTypeId",
                schema: "public",
                table: "r_Leave_Request",
                column: "LeaveTypeId",
                principalSchema: "public",
                principalTable: "r_Leave_Type",
                principalColumn: "LeaveTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_r_Leave_Balance_r_Leave_Type_LeaveTypeId",
                schema: "public",
                table: "r_Leave_Balance");

            migrationBuilder.DropForeignKey(
                name: "FK_r_Leave_Request_r_Leave_Type_LeaveTypeId",
                schema: "public",
                table: "r_Leave_Request");

            migrationBuilder.DropTable(
                name: "r_Leave_Type",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                schema: "public",
                table: "r_Employee_Record");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.DropColumn(
                name: "VerifiedByUserId",
                schema: "public",
                table: "r_Employee_Document");

            migrationBuilder.RenameColumn(
                name: "LeaveTypeId",
                schema: "public",
                table: "r_Leave_Request",
                newName: "Leave_Code_Id");

            migrationBuilder.RenameIndex(
                name: "IX_r_Leave_Request_LeaveTypeId",
                schema: "public",
                table: "r_Leave_Request",
                newName: "IX_r_Leave_Request_Leave_Code_Id");

            migrationBuilder.RenameColumn(
                name: "LeaveTypeId",
                schema: "public",
                table: "r_Leave_Balance",
                newName: "Leave_Code_ID");

            migrationBuilder.RenameIndex(
                name: "IX_r_Leave_Balance_LeaveTypeId",
                schema: "public",
                table: "r_Leave_Balance",
                newName: "IX_r_Leave_Balance_Leave_Code_ID");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                schema: "public",
                table: "r_Employee_Record",
                newName: "Payment_Method");

            migrationBuilder.CreateTable(
                name: "r_Leave_Code",
                schema: "public",
                columns: table => new
                {
                    Leave_Code_ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Leave_Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_r_Leave_Code", x => x.Leave_Code_ID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_r_Leave_Balance_r_Leave_Code_Leave_Code_ID",
                schema: "public",
                table: "r_Leave_Balance",
                column: "Leave_Code_ID",
                principalSchema: "public",
                principalTable: "r_Leave_Code",
                principalColumn: "Leave_Code_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_r_Leave_Request_r_Leave_Code_Leave_Code_Id",
                schema: "public",
                table: "r_Leave_Request",
                column: "Leave_Code_Id",
                principalSchema: "public",
                principalTable: "r_Leave_Code",
                principalColumn: "Leave_Code_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
