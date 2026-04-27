using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPathfinderCampaignTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterSheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterSheets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SysId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "int", nullable: false),
                    CurrentHitPoints = table.Column<int>(type: "int", nullable: false),
                    TemporaryHitPoints = table.Column<int>(type: "int", nullable: false),
                    ArmorClass = table.Column<int>(type: "int", nullable: false),
                    TouchArmorClass = table.Column<int>(type: "int", nullable: false),
                    FlatFootedArmorClass = table.Column<int>(type: "int", nullable: false),
                    InitiativeBonus = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: false),
                    BaseAttackBonus = table.Column<int>(type: "int", nullable: false),
                    CombatManeuverBonus = table.Column<int>(type: "int", nullable: false),
                    CombatManeuverDefense = table.Column<int>(type: "int", nullable: false),
                    FortitudeSave = table.Column<int>(type: "int", nullable: false),
                    ReflexSave = table.Column<int>(type: "int", nullable: false),
                    WillSave = table.Column<int>(type: "int", nullable: false),
                    SkillsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Feats = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialAbilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Spells = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterSheets", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_CharacterSheets_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSheets_CharacterId",
                table: "CharacterSheets",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterSheets_SysId",
                table: "CharacterSheets",
                column: "SysId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterSheets");
        }
    }
}
