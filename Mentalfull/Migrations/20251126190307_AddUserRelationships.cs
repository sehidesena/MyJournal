using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mentalfull.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_AppAiSuggestions_AbpUsers_UserId",
                table: "AppAiSuggestions",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppJournalEntries_AbpUsers_UserId",
                table: "AppJournalEntries",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAiSuggestions_AbpUsers_UserId",
                table: "AppAiSuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppJournalEntries_AbpUsers_UserId",
                table: "AppJournalEntries");
        }
    }
}
