using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyPathfinderCampaignTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSysIdClusteredIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop all FK constraints that reference the PKs we need to rebuild.
            migrationBuilder.DropForeignKey(name: "FK_Users_Campaigns_FavoriteCampaignId", table: "Users");
            migrationBuilder.DropForeignKey(name: "FK_CampaignUsers_Campaigns_CampaignId", table: "CampaignUsers");
            migrationBuilder.DropForeignKey(name: "FK_CampaignUsers_Users_PlayersId", table: "CampaignUsers");
            migrationBuilder.DropForeignKey(name: "FK_Characters_Campaigns_CampaignId", table: "Characters");
            migrationBuilder.DropForeignKey(name: "FK_Characters_Users_UserId", table: "Characters");
            migrationBuilder.DropForeignKey(name: "FK_Recaps_Campaigns_CampaignId", table: "Recaps");
            migrationBuilder.DropForeignKey(name: "FK_Recaps_Users_UserId", table: "Recaps");
            migrationBuilder.DropForeignKey(name: "FK_ChatMessages_Campaigns_CampaignId", table: "ChatMessages");
            migrationBuilder.DropForeignKey(name: "FK_ChatMessages_Users_UserId", table: "ChatMessages");
            migrationBuilder.DropForeignKey(name: "FK_GameSessions_Campaigns_CampaignId", table: "GameSessions");
            migrationBuilder.DropForeignKey(name: "FK_CampaignNotes_Campaigns_CampaignId", table: "CampaignNotes");
            migrationBuilder.DropForeignKey(name: "FK_CampaignNotes_Users_UserId", table: "CampaignNotes");
            migrationBuilder.DropForeignKey(name: "FK_LoreacleMessages_Campaigns_CampaignId", table: "LoreacleMessages");
            migrationBuilder.DropForeignKey(name: "FK_LoreacleMessages_Users_UserId", table: "LoreacleMessages");

            // Step 2: Drop all existing clustered PKs.
            migrationBuilder.DropPrimaryKey(name: "PK_Users", table: "Users");
            migrationBuilder.DropPrimaryKey(name: "PK_Campaigns", table: "Campaigns");
            migrationBuilder.DropPrimaryKey(name: "PK_CampaignUsers", table: "CampaignUsers");
            migrationBuilder.DropPrimaryKey(name: "PK_Characters", table: "Characters");
            migrationBuilder.DropPrimaryKey(name: "PK_Recaps", table: "Recaps");
            migrationBuilder.DropPrimaryKey(name: "PK_ChatMessages", table: "ChatMessages");
            migrationBuilder.DropPrimaryKey(name: "PK_GameSessions", table: "GameSessions");
            migrationBuilder.DropPrimaryKey(name: "PK_CampaignNotes", table: "CampaignNotes");
            migrationBuilder.DropPrimaryKey(name: "PK_LoreacleMessages", table: "LoreacleMessages");

            // Step 3: Add SysId IDENTITY columns (SQL Server auto-populates existing rows).
            migrationBuilder.AddColumn<int>(name: "SysId", table: "Users", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "SysId", table: "Campaigns", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "SysId", table: "CampaignUsers", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "SysId", table: "Characters", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "SysId", table: "Recaps", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "SysId", table: "ChatMessages", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "SysId", table: "GameSessions", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "SysId", table: "CampaignNotes", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");
            migrationBuilder.AddColumn<int>(name: "SysId", table: "LoreacleMessages", type: "int", nullable: false, defaultValue: 0).Annotation("SqlServer:Identity", "1, 1");

            // Step 4: Recreate PKs as non-clustered.
            migrationBuilder.AddPrimaryKey(name: "PK_Users", table: "Users", column: "Id").Annotation("SqlServer:Clustered", false);
            migrationBuilder.AddPrimaryKey(name: "PK_Campaigns", table: "Campaigns", column: "Id").Annotation("SqlServer:Clustered", false);
            migrationBuilder.AddPrimaryKey(name: "PK_CampaignUsers", table: "CampaignUsers", columns: new[] { "CampaignId", "PlayersId" }).Annotation("SqlServer:Clustered", false);
            migrationBuilder.AddPrimaryKey(name: "PK_Characters", table: "Characters", column: "Id").Annotation("SqlServer:Clustered", false);
            migrationBuilder.AddPrimaryKey(name: "PK_Recaps", table: "Recaps", column: "Id").Annotation("SqlServer:Clustered", false);
            migrationBuilder.AddPrimaryKey(name: "PK_ChatMessages", table: "ChatMessages", column: "Id").Annotation("SqlServer:Clustered", false);
            migrationBuilder.AddPrimaryKey(name: "PK_GameSessions", table: "GameSessions", column: "Id").Annotation("SqlServer:Clustered", false);
            migrationBuilder.AddPrimaryKey(name: "PK_CampaignNotes", table: "CampaignNotes", column: "Id").Annotation("SqlServer:Clustered", false);
            migrationBuilder.AddPrimaryKey(name: "PK_LoreacleMessages", table: "LoreacleMessages", column: "Id").Annotation("SqlServer:Clustered", false);

            // Step 5: Create new clustered indexes on SysId.
            migrationBuilder.CreateIndex(name: "IX_Users_SysId", table: "Users", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);
            migrationBuilder.CreateIndex(name: "IX_Campaigns_SysId", table: "Campaigns", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);
            migrationBuilder.CreateIndex(name: "IX_CampaignUsers_SysId", table: "CampaignUsers", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);
            migrationBuilder.CreateIndex(name: "IX_Characters_SysId", table: "Characters", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);
            migrationBuilder.CreateIndex(name: "IX_Recaps_SysId", table: "Recaps", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);
            migrationBuilder.CreateIndex(name: "IX_ChatMessages_SysId", table: "ChatMessages", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);
            migrationBuilder.CreateIndex(name: "IX_GameSessions_SysId", table: "GameSessions", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);
            migrationBuilder.CreateIndex(name: "IX_CampaignNotes_SysId", table: "CampaignNotes", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);
            migrationBuilder.CreateIndex(name: "IX_LoreacleMessages_SysId", table: "LoreacleMessages", column: "SysId", unique: true).Annotation("SqlServer:Clustered", true);

            // Step 6: Recreate all FK constraints.
            migrationBuilder.AddForeignKey(name: "FK_Users_Campaigns_FavoriteCampaignId", table: "Users", column: "FavoriteCampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
            migrationBuilder.AddForeignKey(name: "FK_CampaignUsers_Campaigns_CampaignId", table: "CampaignUsers", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CampaignUsers_Users_PlayersId", table: "CampaignUsers", column: "PlayersId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Characters_Campaigns_CampaignId", table: "Characters", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Characters_Users_UserId", table: "Characters", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_Recaps_Campaigns_CampaignId", table: "Recaps", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Recaps_Users_UserId", table: "Recaps", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_ChatMessages_Campaigns_CampaignId", table: "ChatMessages", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_ChatMessages_Users_UserId", table: "ChatMessages", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_GameSessions_Campaigns_CampaignId", table: "GameSessions", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CampaignNotes_Campaigns_CampaignId", table: "CampaignNotes", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CampaignNotes_Users_UserId", table: "CampaignNotes", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_LoreacleMessages_Campaigns_CampaignId", table: "LoreacleMessages", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_LoreacleMessages_Users_UserId", table: "LoreacleMessages", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop all FKs referencing the PKs we need to rebuild.
            migrationBuilder.DropForeignKey(name: "FK_Users_Campaigns_FavoriteCampaignId", table: "Users");
            migrationBuilder.DropForeignKey(name: "FK_CampaignUsers_Campaigns_CampaignId", table: "CampaignUsers");
            migrationBuilder.DropForeignKey(name: "FK_CampaignUsers_Users_PlayersId", table: "CampaignUsers");
            migrationBuilder.DropForeignKey(name: "FK_Characters_Campaigns_CampaignId", table: "Characters");
            migrationBuilder.DropForeignKey(name: "FK_Characters_Users_UserId", table: "Characters");
            migrationBuilder.DropForeignKey(name: "FK_Recaps_Campaigns_CampaignId", table: "Recaps");
            migrationBuilder.DropForeignKey(name: "FK_Recaps_Users_UserId", table: "Recaps");
            migrationBuilder.DropForeignKey(name: "FK_ChatMessages_Campaigns_CampaignId", table: "ChatMessages");
            migrationBuilder.DropForeignKey(name: "FK_ChatMessages_Users_UserId", table: "ChatMessages");
            migrationBuilder.DropForeignKey(name: "FK_GameSessions_Campaigns_CampaignId", table: "GameSessions");
            migrationBuilder.DropForeignKey(name: "FK_CampaignNotes_Campaigns_CampaignId", table: "CampaignNotes");
            migrationBuilder.DropForeignKey(name: "FK_CampaignNotes_Users_UserId", table: "CampaignNotes");
            migrationBuilder.DropForeignKey(name: "FK_LoreacleMessages_Campaigns_CampaignId", table: "LoreacleMessages");
            migrationBuilder.DropForeignKey(name: "FK_LoreacleMessages_Users_UserId", table: "LoreacleMessages");

            // Step 2: Drop clustered SysId indexes.
            migrationBuilder.DropIndex(name: "IX_Users_SysId", table: "Users");
            migrationBuilder.DropIndex(name: "IX_Campaigns_SysId", table: "Campaigns");
            migrationBuilder.DropIndex(name: "IX_CampaignUsers_SysId", table: "CampaignUsers");
            migrationBuilder.DropIndex(name: "IX_Characters_SysId", table: "Characters");
            migrationBuilder.DropIndex(name: "IX_Recaps_SysId", table: "Recaps");
            migrationBuilder.DropIndex(name: "IX_ChatMessages_SysId", table: "ChatMessages");
            migrationBuilder.DropIndex(name: "IX_GameSessions_SysId", table: "GameSessions");
            migrationBuilder.DropIndex(name: "IX_CampaignNotes_SysId", table: "CampaignNotes");
            migrationBuilder.DropIndex(name: "IX_LoreacleMessages_SysId", table: "LoreacleMessages");

            // Step 3: Drop non-clustered PKs.
            migrationBuilder.DropPrimaryKey(name: "PK_Users", table: "Users");
            migrationBuilder.DropPrimaryKey(name: "PK_Campaigns", table: "Campaigns");
            migrationBuilder.DropPrimaryKey(name: "PK_CampaignUsers", table: "CampaignUsers");
            migrationBuilder.DropPrimaryKey(name: "PK_Characters", table: "Characters");
            migrationBuilder.DropPrimaryKey(name: "PK_Recaps", table: "Recaps");
            migrationBuilder.DropPrimaryKey(name: "PK_ChatMessages", table: "ChatMessages");
            migrationBuilder.DropPrimaryKey(name: "PK_GameSessions", table: "GameSessions");
            migrationBuilder.DropPrimaryKey(name: "PK_CampaignNotes", table: "CampaignNotes");
            migrationBuilder.DropPrimaryKey(name: "PK_LoreacleMessages", table: "LoreacleMessages");

            // Step 4: Drop SysId columns.
            migrationBuilder.DropColumn(name: "SysId", table: "Users");
            migrationBuilder.DropColumn(name: "SysId", table: "Campaigns");
            migrationBuilder.DropColumn(name: "SysId", table: "CampaignUsers");
            migrationBuilder.DropColumn(name: "SysId", table: "Characters");
            migrationBuilder.DropColumn(name: "SysId", table: "Recaps");
            migrationBuilder.DropColumn(name: "SysId", table: "ChatMessages");
            migrationBuilder.DropColumn(name: "SysId", table: "GameSessions");
            migrationBuilder.DropColumn(name: "SysId", table: "CampaignNotes");
            migrationBuilder.DropColumn(name: "SysId", table: "LoreacleMessages");

            // Step 5: Recreate PKs as clustered (default).
            migrationBuilder.AddPrimaryKey(name: "PK_Users", table: "Users", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Campaigns", table: "Campaigns", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_CampaignUsers", table: "CampaignUsers", columns: new[] { "CampaignId", "PlayersId" });
            migrationBuilder.AddPrimaryKey(name: "PK_Characters", table: "Characters", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_Recaps", table: "Recaps", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_ChatMessages", table: "ChatMessages", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_GameSessions", table: "GameSessions", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_CampaignNotes", table: "CampaignNotes", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_LoreacleMessages", table: "LoreacleMessages", column: "Id");

            // Step 6: Recreate all FK constraints.
            migrationBuilder.AddForeignKey(name: "FK_Users_Campaigns_FavoriteCampaignId", table: "Users", column: "FavoriteCampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
            migrationBuilder.AddForeignKey(name: "FK_CampaignUsers_Campaigns_CampaignId", table: "CampaignUsers", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CampaignUsers_Users_PlayersId", table: "CampaignUsers", column: "PlayersId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Characters_Campaigns_CampaignId", table: "Characters", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Characters_Users_UserId", table: "Characters", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_Recaps_Campaigns_CampaignId", table: "Recaps", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Recaps_Users_UserId", table: "Recaps", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_ChatMessages_Campaigns_CampaignId", table: "ChatMessages", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_ChatMessages_Users_UserId", table: "ChatMessages", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_GameSessions_Campaigns_CampaignId", table: "GameSessions", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CampaignNotes_Campaigns_CampaignId", table: "CampaignNotes", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_CampaignNotes_Users_UserId", table: "CampaignNotes", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_LoreacleMessages_Campaigns_CampaignId", table: "LoreacleMessages", column: "CampaignId", principalTable: "Campaigns", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_LoreacleMessages_Users_UserId", table: "LoreacleMessages", column: "UserId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        }
    }
}
