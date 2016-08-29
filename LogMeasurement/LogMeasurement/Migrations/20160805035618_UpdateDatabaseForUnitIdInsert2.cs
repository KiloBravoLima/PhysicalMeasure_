using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogMeasurement.Migrations
{
    public partial class UpdateDatabaseForUnitIdInsert2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements");

            /*
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Units");
            */
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
