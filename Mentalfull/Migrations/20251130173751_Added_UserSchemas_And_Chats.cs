using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentalfull.Migrations
{
    /// <inheritdoc />
    public partial class Added_UserSchemas_And_Chats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppGoalNotes_AbpUsers_UserId",
                table: "AppGoalNotes");

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityUserId",
                table: "AppGoalNotes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppChatSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    AiSummary = table.Column<string>(type: "text", nullable: false),
                    LastMessageTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_AppChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppChatSessions_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserSchemas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IntensityScore = table.Column<int>(type: "integer", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    SourceType = table.Column<int>(type: "integer", nullable: false),
                    LastAiDedectionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FeedbackStatus = table.Column<int>(type: "integer", nullable: false),
                    FeedbackNote = table.Column<string>(type: "text", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_AppUserSchemas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserSchemas_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChatSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sender = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    HasInlineAnalysis = table.Column<bool>(type: "boolean", nullable: false),
                    InlineAnalysisSummary = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_AppChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppChatMessages_AppChatSessions_ChatSessionId",
                        column: x => x.ChatSessionId,
                        principalTable: "AppChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppGoalNotes_IdentityUserId",
                table: "AppGoalNotes",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppChatMessages_ChatSessionId",
                table: "AppChatMessages",
                column: "ChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppChatSessions_UserId",
                table: "AppChatSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserSchemas_UserId",
                table: "AppUserSchemas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGoalNotes_AbpUsers_IdentityUserId",
                table: "AppGoalNotes",
                column: "IdentityUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppGoalNotes_AbpUsers_IdentityUserId",
                table: "AppGoalNotes");

            migrationBuilder.DropTable(
                name: "AppChatMessages");

            migrationBuilder.DropTable(
                name: "AppUserSchemas");

            migrationBuilder.DropTable(
                name: "AppChatSessions");

            migrationBuilder.DropIndex(
                name: "IX_AppGoalNotes_IdentityUserId",
                table: "AppGoalNotes");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "AppGoalNotes");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGoalNotes_AbpUsers_UserId",
                table: "AppGoalNotes",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");
        }
    }
}
