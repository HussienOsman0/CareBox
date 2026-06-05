using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareBox.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIgnoredEmergencyRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IgnoredEmergencyRequests",
                columns: table => new
                {
                    EmergencyRequestId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IgnoredEmergencyRequests", x => new { x.EmergencyRequestId, x.ServiceProviderId });
                    table.ForeignKey(
                        name: "FK_IgnoredEmergencyRequests_EmergencyRequests_EmergencyRequestId",
                        column: x => x.EmergencyRequestId,
                        principalTable: "EmergencyRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IgnoredEmergencyRequests_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "ServiceProviderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IgnoredEmergencyRequests_ServiceProviderId",
                table: "IgnoredEmergencyRequests",
                column: "ServiceProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IgnoredEmergencyRequests");
        }
    }
}
