using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_InitiatorId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_ReceiverId",
                table: "Friendships");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Friendships",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Friendships",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_ApplicationUserId",
                table: "Friendships",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_ApplicationUserId1",
                table: "Friendships",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_ApplicationUserId",
                table: "Friendships",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_ApplicationUserId1",
                table: "Friendships",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_InitiatorId",
                table: "Friendships",
                column: "InitiatorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_ReceiverId",
                table: "Friendships",
                column: "ReceiverId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_ApplicationUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_ApplicationUserId1",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_InitiatorId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_ReceiverId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_ApplicationUserId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_ApplicationUserId1",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Friendships");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_InitiatorId",
                table: "Friendships",
                column: "InitiatorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_ReceiverId",
                table: "Friendships",
                column: "ReceiverId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
