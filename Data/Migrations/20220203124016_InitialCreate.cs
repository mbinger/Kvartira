using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProviderHealthLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IdsLoaded = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NewIdsLoaded = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DetailsLoaded = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AllDetailsComlete = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderHealthLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Wohnungen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Geladen = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    WohnungId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Wichtigkeit = table.Column<int>(type: "INTEGER", nullable: false),
                    SucheShort = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    SucheDetails = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    Gesehen = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Gemeldet = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LoadDetailsTries = table.Column<int>(type: "INTEGER", nullable: false),
                    Ueberschrift = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_Wohnungen", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wohnungen_Provider_WohnungId",
                table: "Wohnungen",
                columns: new[] { "Provider", "WohnungId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProviderHealthLogs");

            migrationBuilder.DropTable(
                name: "Wohnungen");
        }
    }
}
