using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareBox.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixProviderRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRequests_ServiceProviders_ServiceProviderId",
                table: "EmergencyRequests");

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ServiceProviderId",
                table: "Orders",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRequests_ServiceProviders_ServiceProviderId",
                table: "EmergencyRequests",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ServiceProviders_ServiceProviderId",
                table: "Orders",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "ServiceProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyRequests_ServiceProviders_ServiceProviderId",
                table: "EmergencyRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ServiceProviders_ServiceProviderId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ServiceProviderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyRequests_ServiceProviders_ServiceProviderId",
                table: "EmergencyRequests",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "ServiceProviderId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
