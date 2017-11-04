using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Db.Data.Migrations
{
    public partial class _8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "AnswerDate",
                table: "Settlements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "BillsID",
                table: "PaymentMethods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CitizenDepts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YourDropdownSelectedValue",
                table: "Bills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "County",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_BillsID",
                table: "PaymentMethods",
                column: "BillsID");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Bills_BillsID",
                table: "PaymentMethods",
                column: "BillsID",
                principalTable: "Bills",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Bills_BillsID",
                table: "PaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_BillsID",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "AnswerDate",
                table: "Settlements");

            migrationBuilder.DropColumn(
                name: "BillsID",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CitizenDepts");

            migrationBuilder.DropColumn(
                name: "YourDropdownSelectedValue",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "County",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastName",
                table: "Settlements",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
