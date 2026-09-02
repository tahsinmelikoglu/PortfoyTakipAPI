using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PortfoyTakipAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedHalkaArzData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HalkaArzlar",
                columns: new[] { "Id", "BorsaKodu", "LotFiyati", "SirketAdi", "Statu", "TalepToplamaBaslangic", "TalepToplamaBitis", "ToplamDagilacakLot" },
                values: new object[,]
                {
                    { 1, "MCARD", 25.00m, "Metropol Kurumsal Hizmetler", "İşlem Gören", new DateTime(2026, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 30000000L },
                    { 2, "TKNJI", 42.50m, "Teknoloji Gelecek A.Ş.", "Talep Toplayan", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 15000000L },
                    { 3, "ENRGY", 18.20m, "Yeşil Enerji Üretim", "Yaklaşan", new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 55000000L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
