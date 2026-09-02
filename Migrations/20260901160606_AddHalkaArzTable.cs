using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfoyTakipAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddHalkaArzTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IslemGormeyeBasladiMi",
                table: "HalkaArzlar");

            migrationBuilder.RenameColumn(
                name: "TalepFiyati",
                table: "HalkaArzlar",
                newName: "LotFiyati");

            migrationBuilder.AddColumn<string>(
                name: "BorsaKodu",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Statu",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TalepToplamaBaslangic",
                table: "HalkaArzlar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TalepToplamaBitis",
                table: "HalkaArzlar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "ToplamDagilacakLot",
                table: "HalkaArzlar",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorsaKodu",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "Statu",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "TalepToplamaBaslangic",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "TalepToplamaBitis",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "ToplamDagilacakLot",
                table: "HalkaArzlar");

            migrationBuilder.RenameColumn(
                name: "LotFiyati",
                table: "HalkaArzlar",
                newName: "TalepFiyati");

            migrationBuilder.AddColumn<bool>(
                name: "IslemGormeyeBasladiMi",
                table: "HalkaArzlar",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
