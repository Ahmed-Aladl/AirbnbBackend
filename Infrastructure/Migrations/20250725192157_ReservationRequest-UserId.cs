using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReservationRequestUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ReservationRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservationRequests_UserId",
                table: "ReservationRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationRequests_AspNetUsers_UserId",
                table: "ReservationRequests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReservationRequests_AspNetUsers_UserId",
                table: "ReservationRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReservationRequests_UserId",
                table: "ReservationRequests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ReservationRequests");
        }
    }
}
