using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CareBox.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedProviderTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProviderTypes",
                columns: new[] { "ProviderTypeId", "TypeName" },
                values: new object[,]
                {
                    { (byte)1, "Maintenance" },
                    { (byte)2, "Spare Parts" },
                    { (byte)3, "Emergency" },
                    { (byte)4, "Car Care" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProviderTypes",
                keyColumn: "ProviderTypeId",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "ProviderTypes",
                keyColumn: "ProviderTypeId",
                keyValue: (byte)2);

            migrationBuilder.DeleteData(
                table: "ProviderTypes",
                keyColumn: "ProviderTypeId",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "ProviderTypes",
                keyColumn: "ProviderTypeId",
                keyValue: (byte)4);
        }
    }
}
