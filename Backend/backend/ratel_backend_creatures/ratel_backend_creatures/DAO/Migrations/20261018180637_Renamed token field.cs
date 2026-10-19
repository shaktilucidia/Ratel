using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ratel_backend_creatures.DAO.Migrations
{
    /// <inheritdoc />
    public partial class Renamedtokenfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "SessionRefreshTokenDbo",
                newName: "Hash");

            migrationBuilder.RenameIndex(
                name: "IX_SessionRefreshTokenDbo_TokenHash",
                table: "SessionRefreshTokenDbo",
                newName: "IX_SessionRefreshTokenDbo_Hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Hash",
                table: "SessionRefreshTokenDbo",
                newName: "TokenHash");

            migrationBuilder.RenameIndex(
                name: "IX_SessionRefreshTokenDbo_Hash",
                table: "SessionRefreshTokenDbo",
                newName: "IX_SessionRefreshTokenDbo_TokenHash");
        }
    }
}
