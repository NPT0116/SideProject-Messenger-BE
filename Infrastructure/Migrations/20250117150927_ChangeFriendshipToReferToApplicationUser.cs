using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFriendshipToReferToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "ReceiverId",
                table: "Friendships",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "InitiatorId",
                table: "Friendships",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Friendships",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Friendships",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId1",
                table: "Friendships",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_InitiatorId",
                table: "Friendships",
                column: "InitiatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_ReceiverId",
                table: "Friendships",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_UserId",
                table: "Friendships",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_User_UserId1",
                table: "Friendships",
                column: "UserId1",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_InitiatorId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_ReceiverId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_UserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_User_UserId1",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId1",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Friendships");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverId",
                table: "Friendships",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "InitiatorId",
                table: "Friendships",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

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
    }
}
