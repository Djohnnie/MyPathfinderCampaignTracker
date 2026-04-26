using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPathfinderCampaignTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PhotoData",
                table: "Characters",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoData",
                table: "Characters");
        }
    }
}
