using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPathfinderCampaignTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoreacleCompactionFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompacted",
                table: "LoreacleMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompaction",
                table: "LoreacleMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompacted",
                table: "LoreacleMessages");

            migrationBuilder.DropColumn(
                name: "IsCompaction",
                table: "LoreacleMessages");
        }
    }
}
