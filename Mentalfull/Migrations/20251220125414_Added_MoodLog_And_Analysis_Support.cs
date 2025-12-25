using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentalfull.Migrations
{
    /// <inheritdoc />
    public partial class Added_MoodLog_And_Analysis_Support : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MoodTag",
                table: "AppJournalEntries",
                newName: "AudioUrl");

            migrationBuilder.RenameColumn(
                name: "MoodScore",
                table: "AppJournalEntries",
                newName: "DurationSeconds");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "AppJournalEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppEmotionalAnalysisResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JournalEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SentimentScore = table.Column<float>(type: "real", nullable: false),
                    DominantEmotion = table.Column<string>(type: "text", nullable: false),
                    AnalysisSummary = table.Column<string>(type: "text", nullable: false),
                    EmotionProbabilities = table.Column<string>(type: "text", nullable: false),
                    ClinicalFlags = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEmotionalAnalysisResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEmotionalAnalysisResults_AppJournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "AppJournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppMoodLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Intensity = table.Column<int>(type: "integer", nullable: false),
                    PrimaryEmotion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppMoodLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMoodLogs_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppEmotionalAnalysisResults_JournalEntryId",
                table: "AppEmotionalAnalysisResults",
                column: "JournalEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMoodLogs_Timestamp",
                table: "AppMoodLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AppMoodLogs_UserId",
                table: "AppMoodLogs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppEmotionalAnalysisResults");

            migrationBuilder.DropTable(
                name: "AppMoodLogs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "AppJournalEntries");

            migrationBuilder.RenameColumn(
                name: "DurationSeconds",
                table: "AppJournalEntries",
                newName: "MoodScore");

            migrationBuilder.RenameColumn(
                name: "AudioUrl",
                table: "AppJournalEntries",
                newName: "MoodTag");
        }
    }
}
