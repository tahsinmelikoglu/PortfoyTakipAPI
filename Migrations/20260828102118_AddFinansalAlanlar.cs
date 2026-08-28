using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfoyTakipAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddFinansalAlanlar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AlimTarihi",
                table: "Varliklar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "AlisFiyati",
                table: "Varliklar",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlimTarihi",
                table: "Varliklar");

            migrationBuilder.DropColumn(
                name: "AlisFiyati",
                table: "Varliklar");
        }
    }
}
