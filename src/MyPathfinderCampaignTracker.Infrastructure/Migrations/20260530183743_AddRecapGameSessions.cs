using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MyPathfinderCampaignTracker.Infrastructure.Data;

#nullable disable

namespace MyPathfinderCampaignTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260530183743_AddRecapGameSessions")]
    public partial class AddRecapGameSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GameSessionId",
                table: "Recaps",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recaps_GameSessionId",
                table: "Recaps",
                column: "GameSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recaps_GameSessions_GameSessionId",
                table: "Recaps",
                column: "GameSessionId",
                principalTable: "GameSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recaps_GameSessions_GameSessionId",
                table: "Recaps");

            migrationBuilder.DropIndex(
                name: "IX_Recaps_GameSessionId",
                table: "Recaps");

            migrationBuilder.DropColumn(
                name: "GameSessionId",
                table: "Recaps");
        }
    }
}
