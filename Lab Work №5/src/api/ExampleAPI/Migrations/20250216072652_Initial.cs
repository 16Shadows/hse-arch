using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExampleAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "filials",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_filials", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "housing_types",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_housing_types", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "settlements",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    FilialID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settlements", x => x.ID);
                    table.ForeignKey(
                        name: "FK_settlements_filials_FilialID",
                        column: x => x.FilialID,
                        principalTable: "filials",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "teos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FilialID = table.Column<int>(type: "integer", nullable: true),
                    HousingTypeID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_teos_filials_FilialID",
                        column: x => x.FilialID,
                        principalTable: "filials",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_teos_housing_types_HousingTypeID",
                        column: x => x.HousingTypeID,
                        principalTable: "housing_types",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SettlementTeo",
                columns: table => new
                {
                    SettlementsID = table.Column<int>(type: "integer", nullable: false),
                    TeoID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementTeo", x => new { x.SettlementsID, x.TeoID });
                    table.ForeignKey(
                        name: "FK_SettlementTeo_settlements_SettlementsID",
                        column: x => x.SettlementsID,
                        principalTable: "settlements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SettlementTeo_teos_TeoID",
                        column: x => x.TeoID,
                        principalTable: "teos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_settlements_FilialID",
                table: "settlements",
                column: "FilialID");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementTeo_TeoID",
                table: "SettlementTeo",
                column: "TeoID");

            migrationBuilder.CreateIndex(
                name: "IX_teos_FilialID",
                table: "teos",
                column: "FilialID");

            migrationBuilder.CreateIndex(
                name: "IX_teos_HousingTypeID",
                table: "teos",
                column: "HousingTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettlementTeo");

            migrationBuilder.DropTable(
                name: "settlements");

            migrationBuilder.DropTable(
                name: "teos");

            migrationBuilder.DropTable(
                name: "filials");

            migrationBuilder.DropTable(
                name: "housing_types");
        }
    }
}
