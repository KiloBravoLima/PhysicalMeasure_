using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LogMeasurement.Migrations
{
    public partial class UpdateDatabaseForUnitIdInsert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Units");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Units",
                nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements",
                column: "MeasuredUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
                */
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

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Units",
                nullable: false);

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
