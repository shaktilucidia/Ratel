using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ratel_backend_creatures.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddedSessionEventDateandIndexForRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OccurredAt",
                table: "SessionEventDbo",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_SessionRefreshTokenDbo_TokenHash",
                table: "SessionRefreshTokenDbo",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SessionRefreshTokenDbo_TokenHash",
                table: "SessionRefreshTokenDbo");

            migrationBuilder.DropColumn(
                name: "OccurredAt",
                table: "SessionEventDbo");
        }
    }
}
