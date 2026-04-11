using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareBox.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeCategoryPerProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "ServiceCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategories_ServiceProviderId",
                table: "ServiceCategories",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceCategories_ServiceProviders_ServiceProviderId",
                table: "ServiceCategories",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "ServiceProviderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceCategories_ServiceProviders_ServiceProviderId",
                table: "ServiceCategories");

            migrationBuilder.DropIndex(
                name: "IX_ServiceCategories_ServiceProviderId",
                table: "ServiceCategories");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "ServiceCategories");
        }
    }
}
