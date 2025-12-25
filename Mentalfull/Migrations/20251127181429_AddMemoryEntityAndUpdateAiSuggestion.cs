using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentalfull.Migrations
{
    /// <inheritdoc />
    public partial class AddMemoryEntityAndUpdateAiSuggestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "JournalEntryId",
                table: "AppAiSuggestions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "MemoryId",
                table: "AppAiSuggestions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppMemories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MemoryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    PeopleInvolved = table.Column<string>(type: "text", nullable: false),
                    EmotionAtThatTime = table.Column<string>(type: "text", nullable: false),
                    EmotionNow = table.Column<string>(type: "text", nullable: false),
                    IntensityScore = table.Column<int>(type: "integer", nullable: false),
                    IsPositive = table.Column<bool>(type: "boolean", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    HasAiAnalysis = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
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
                    table.PrimaryKey("PK_AppMemories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppMemories_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAiSuggestions_MemoryId",
                table: "AppAiSuggestions",
                column: "MemoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppMemories_MemoryDate",
                table: "AppMemories",
                column: "MemoryDate");

            migrationBuilder.CreateIndex(
                name: "IX_AppMemories_UserId",
                table: "AppMemories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAiSuggestions_AppMemories_MemoryId",
                table: "AppAiSuggestions",
                column: "MemoryId",
                principalTable: "AppMemories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAiSuggestions_AppMemories_MemoryId",
                table: "AppAiSuggestions");

            migrationBuilder.DropTable(
                name: "AppMemories");

            migrationBuilder.DropIndex(
                name: "IX_AppAiSuggestions_MemoryId",
                table: "AppAiSuggestions");

            migrationBuilder.DropColumn(
                name: "MemoryId",
                table: "AppAiSuggestions");

            migrationBuilder.AlterColumn<Guid>(
                name: "JournalEntryId",
                table: "AppAiSuggestions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
