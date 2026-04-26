using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPathfinderCampaignTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FavoriteCampaignId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FavoriteCampaignId",
                table: "Users",
                column: "FavoriteCampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Campaigns_FavoriteCampaignId",
                table: "Users",
                column: "FavoriteCampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Campaigns_FavoriteCampaignId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_FavoriteCampaignId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FavoriteCampaignId",
                table: "Users");
        }
    }
}
