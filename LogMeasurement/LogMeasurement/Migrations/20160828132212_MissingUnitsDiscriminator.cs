using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogMeasurement.Migrations
{
    public partial class MissingUnitsDiscriminator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitListElements_Units_UnitId",
                table: "UnitListElements");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Units");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements",
                column: "MeasuredUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitListElements_Units_UnitId",
                table: "UnitListElements",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitListElements_Units_UnitId",
                table: "UnitListElements");

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

            migrationBuilder.AddForeignKey(
                name: "FK_UnitListElements_Units_UnitId",
                table: "UnitListElements",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
