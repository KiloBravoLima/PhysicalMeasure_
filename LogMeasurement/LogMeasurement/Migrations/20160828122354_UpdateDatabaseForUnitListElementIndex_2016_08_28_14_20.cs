using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LogMeasurement.Migrations
{
    public partial class UpdateDatabaseForUnitListElementIndex_2016_08_28_14_20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Units");

            migrationBuilder.DropTable(
                name: "UnitListItems");

            migrationBuilder.CreateTable(
                name: "UnitListElements",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ElementIndex = table.Column<int>(nullable: false),
                    ListId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitListElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitListElements_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitListElements_UnitId",
                table: "UnitListElements",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements",
                column: "MeasuredUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements");

            migrationBuilder.DropTable(
                name: "UnitListElements");

            migrationBuilder.CreateTable(
                name: "UnitListItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ListId = table.Column<int>(nullable: false),
                    UnitId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitListItems", x => x.Id);
                });

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Units",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements",
                column: "MeasuredUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
