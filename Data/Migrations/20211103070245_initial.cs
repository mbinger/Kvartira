using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WohnungHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    WohnungId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    TraumWohnung = table.Column<bool>(type: "INTEGER", nullable: false),
                    Gesehen = table.Column<bool>(type: "INTEGER", nullable: false),
                    Gemeldet = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WohnungHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WohnungDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WohnungHeaderId = table.Column<int>(type: "INTEGER", nullable: false),
                    Bezirk = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Anschrift = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    MieteKalt = table.Column<decimal>(type: "TEXT", nullable: true),
                    MieteWarm = table.Column<decimal>(type: "TEXT", nullable: true),
                    Etage = table.Column<int>(type: "INTEGER", nullable: true),
                    Etagen = table.Column<int>(type: "INTEGER", nullable: true),
                    Zimmer = table.Column<int>(type: "INTEGER", nullable: true),
                    Flaeche = table.Column<decimal>(type: "TEXT", nullable: true),
                    FreiAb = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Beschreibung = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    Wbs = table.Column<bool>(type: "INTEGER", nullable: true),
                    Balkon = table.Column<bool>(type: "INTEGER", nullable: true),
                    Keller = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WohnungDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WohnungDetails_WohnungHeaders_WohnungHeaderId",
                        column: x => x.WohnungHeaderId,
                        principalTable: "WohnungHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WohnungDetails_WohnungHeaderId",
                table: "WohnungDetails",
                column: "WohnungHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_WohnungHeaders_Provider_WohnungId",
                table: "WohnungHeaders",
                columns: new[] { "Provider", "WohnungId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WohnungDetails");

            migrationBuilder.DropTable(
                name: "WohnungHeaders");
        }
    }
}
