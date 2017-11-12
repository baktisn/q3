using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Db.Data.Migrations
{
    public partial class _12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CitizenDepts");

            migrationBuilder.AlterColumn<decimal>(
                name: "Interest",
                table: "SettlementTypes",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AlterColumn<decimal>(
                name: "Interest",
                table: "Settlements",
                type: "decimal(18, 2)",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<string>(
                name: "BillId",
                table: "CitizenDepts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserGUId",
                table: "CitizenDepts",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillId",
                table: "CitizenDepts");

            migrationBuilder.DropColumn(
                name: "UserGUId",
                table: "CitizenDepts");

            migrationBuilder.AlterColumn<short>(
                name: "Interest",
                table: "SettlementTypes",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AlterColumn<short>(
                name: "Interest",
                table: "Settlements",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 2)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CitizenDepts",
                nullable: true);
        }
    }
}
