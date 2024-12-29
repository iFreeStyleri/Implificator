using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Implificator.DAL.Migrations
{
    public partial class AddBlockedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TgId = table.Column<long>(type: "bigint", nullable: false),
                    CountSubscribe = table.Column<long>(type: "bigint", nullable: false),
                    CountMail = table.Column<long>(type: "bigint", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QRMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SharedUserId = table.Column<long>(type: "bigint", nullable: false),
                    URL = table.Column<string>(type: "text", nullable: false),
                    IsPublish = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRMessages_Users_SharedUserId",
                        column: x => x.SharedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QRMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VIPs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClosedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VIPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VIPs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QRMessages_SharedUserId",
                table: "QRMessages",
                column: "SharedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QRMessages_URL",
                table: "QRMessages",
                column: "URL");

            migrationBuilder.CreateIndex(
                name: "IX_QRMessages_UserId",
                table: "QRMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TgId",
                table: "Users",
                column: "TgId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VIPs_UserId",
                table: "VIPs",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QRMessages");

            migrationBuilder.DropTable(
                name: "VIPs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
