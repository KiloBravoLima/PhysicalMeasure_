using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LogMeasurement.Migrations
{
    public partial class UpdateDatabaseForUnitExponents : Migration
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

            /*
            migrationBuilder.AlterColumn<byte[]>(
                name: "ExponentsBin",
                table: "Units",
                nullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Units",
                nullable: false)
                //.Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                ;
            */
            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Units_MeasuredUnitId",
                table: "Measurements",
                column: "MeasuredUnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            /*
            migrationBuilder.RenameColumn(
                name: "ExponentsBin",
                table: "Units",
                newName: "Exponents");
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

            migrationBuilder.AlterColumn<string>(
                name: "Exponents",
                table: "Units",
                nullable: true);

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

            migrationBuilder.RenameColumn(
                name: "Exponents",
                table: "Units",
                newName: "ExponentsBin");
        }
    }
}
