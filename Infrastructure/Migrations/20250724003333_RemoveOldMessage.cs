using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HostReply_AspNetUsers_UserId",
                table: "HostReply");

            migrationBuilder.DropForeignKey(
                name: "FK_HostReply_Reviews_ReviewId",
                table: "HostReply");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HostReply",
                table: "HostReply");

            migrationBuilder.RenameTable(
                name: "HostReply",
                newName: "HOST");

            migrationBuilder.RenameIndex(
                name: "IX_HostReply_UserId",
                table: "HOST",
                newName: "IX_HOST_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HostReply_ReviewId",
                table: "HOST",
                newName: "IX_HOST_ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HOST",
                table: "HOST",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HOST_AspNetUsers_UserId",
                table: "HOST",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HOST_Reviews_ReviewId",
                table: "HOST",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HOST_AspNetUsers_UserId",
                table: "HOST");

            migrationBuilder.DropForeignKey(
                name: "FK_HOST_Reviews_ReviewId",
                table: "HOST");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HOST",
                table: "HOST");

            migrationBuilder.RenameTable(
                name: "HOST",
                newName: "HostReply");

            migrationBuilder.RenameIndex(
                name: "IX_HOST_UserId",
                table: "HostReply",
                newName: "IX_HostReply_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HOST_ReviewId",
                table: "HostReply",
                newName: "IX_HostReply_ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HostReply",
                table: "HostReply",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Isread = table.Column<bool>(type: "bit", nullable: false),
                    MesageText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_PropertyId",
                table: "Messages",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverId",
                table: "Messages",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_HostReply_AspNetUsers_UserId",
                table: "HostReply",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HostReply_Reviews_ReviewId",
                table: "HostReply",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
