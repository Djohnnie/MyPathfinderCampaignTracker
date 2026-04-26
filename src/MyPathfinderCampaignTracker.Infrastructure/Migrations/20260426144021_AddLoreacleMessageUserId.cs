using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPathfinderCampaignTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoreacleMessageUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing rows have no valid UserId — clear them before adding the FK.
            migrationBuilder.Sql("DELETE FROM [LoreacleMessages]");

            migrationBuilder.DropIndex(
                name: "IX_LoreacleMessages_CampaignId_SentAt",
                table: "LoreacleMessages");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "LoreacleMessages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LoreacleMessages_CampaignId_UserId_SentAt",
                table: "LoreacleMessages",
                columns: new[] { "CampaignId", "UserId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LoreacleMessages_UserId",
                table: "LoreacleMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoreacleMessages_Users_UserId",
                table: "LoreacleMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoreacleMessages_Users_UserId",
                table: "LoreacleMessages");

            migrationBuilder.DropIndex(
                name: "IX_LoreacleMessages_CampaignId_UserId_SentAt",
                table: "LoreacleMessages");

            migrationBuilder.DropIndex(
                name: "IX_LoreacleMessages_UserId",
                table: "LoreacleMessages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LoreacleMessages");

            migrationBuilder.CreateIndex(
                name: "IX_LoreacleMessages_CampaignId_SentAt",
                table: "LoreacleMessages",
                columns: new[] { "CampaignId", "SentAt" });
        }
    }
}
