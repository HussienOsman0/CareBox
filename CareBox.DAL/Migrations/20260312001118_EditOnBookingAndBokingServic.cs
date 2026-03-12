using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareBox.DAL.Migrations
{
    /// <inheritdoc />
    public partial class EditOnBookingAndBokingServic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "BookingServices",
                newName: "Price");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "BookingServices",
                newName: "TotalPrice");
        }
    }
}
