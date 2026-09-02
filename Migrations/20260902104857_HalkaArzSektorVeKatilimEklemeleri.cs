using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfoyTakipAPI.Migrations
{
    /// <inheritdoc />
    public partial class HalkaArzSektorVeKatilimEklemeleri : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ToplamDagilacakLot",
                table: "HalkaArzlar",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "GerceklesenKatilimciSayisi",
                table: "HalkaArzlar",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "KatilimEndeksineUygunMu",
                table: "HalkaArzlar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KonsorsiyumLideri",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sektor",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "GerceklesenKatilimciSayisi", "KatilimEndeksineUygunMu", "KonsorsiyumLideri", "Sektor", "ToplamDagilacakLot" },
                values: new object[] { null, false, null, null, 30000000 });

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GerceklesenKatilimciSayisi", "KatilimEndeksineUygunMu", "KonsorsiyumLideri", "Sektor", "ToplamDagilacakLot" },
                values: new object[] { null, false, null, null, 15000000 });

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "GerceklesenKatilimciSayisi", "KatilimEndeksineUygunMu", "KonsorsiyumLideri", "Sektor", "ToplamDagilacakLot" },
                values: new object[] { null, false, null, null, 55000000 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GerceklesenKatilimciSayisi",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "KatilimEndeksineUygunMu",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "KonsorsiyumLideri",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "Sektor",
                table: "HalkaArzlar");

            migrationBuilder.AlterColumn<long>(
                name: "ToplamDagilacakLot",
                table: "HalkaArzlar",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 1,
                column: "ToplamDagilacakLot",
                value: 30000000L);

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 2,
                column: "ToplamDagilacakLot",
                value: 15000000L);

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 3,
                column: "ToplamDagilacakLot",
                value: 55000000L);
        }
    }
}
