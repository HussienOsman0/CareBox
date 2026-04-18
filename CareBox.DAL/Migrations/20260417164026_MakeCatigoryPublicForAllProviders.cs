using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareBox.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeCatigoryPublicForAllProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_ServiceProviders_ServiceProviderId",
                table: "ProductCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProductCategories_ServiceProviderId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "ServiceProviderId",
                table: "ProductCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderId",
                table: "ProductCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ServiceProviderId",
                table: "ProductCategories",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_ServiceProviders_ServiceProviderId",
                table: "ProductCategories",
                column: "ServiceProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "ServiceProviderId");
        }
    }
}
