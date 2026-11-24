using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ratel_backend_creatures.DAO.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRefreshTokenExpirationTIme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "SessionRefreshTokenDbo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "SessionRefreshTokenDbo",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
