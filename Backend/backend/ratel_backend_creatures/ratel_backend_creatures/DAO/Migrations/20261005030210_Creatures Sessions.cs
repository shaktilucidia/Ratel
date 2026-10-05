using System;
using System.Net;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ratel_backend_creatures.DAO.Migrations
{
    /// <inheritdoc />
    public partial class CreaturesSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreatureSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatureId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatureSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreatureSessions_AspNetUsers_CreatureId",
                        column: x => x.CreatureId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionEventDbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Source = table.Column<IPAddress>(type: "inet", nullable: false),
                    DeviceInfo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionEventDbo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionEventDbo_CreatureSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "CreatureSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionRefreshTokenDbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionRefreshTokenDbo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionRefreshTokenDbo_CreatureSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "CreatureSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreatureSessions_CreatureId",
                table: "CreatureSessions",
                column: "CreatureId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionEventDbo_SessionId",
                table: "SessionEventDbo",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionRefreshTokenDbo_SessionId",
                table: "SessionRefreshTokenDbo",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionEventDbo");

            migrationBuilder.DropTable(
                name: "SessionRefreshTokenDbo");

            migrationBuilder.DropTable(
                name: "CreatureSessions");
        }
    }
}
