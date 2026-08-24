using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfoyTakipAPI.Migrations
{
    /// <inheritdoc />
    public partial class RolVeRefreshTokenEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenBitisSuresi",
                table: "Kullanicilar",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Kullanicilar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "RefreshTokenBitisSuresi",
                table: "Kullanicilar");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Kullanicilar");
        }
    }
}
