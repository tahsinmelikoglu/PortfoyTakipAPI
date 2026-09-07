using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfoyTakipAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdminPaneli : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Statu",
                table: "HalkaArzlar",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SirketAdi",
                table: "HalkaArzlar",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Sektor",
                table: "HalkaArzlar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KonsorsiyumLideri",
                table: "HalkaArzlar",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BorsaKodu",
                table: "HalkaArzlar",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "ArzBuyukluguTL",
                table: "HalkaArzlar",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DagitimYontemi",
                table: "HalkaArzlar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinansalBorcluluk",
                table: "HalkaArzlar",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinansalCiroArtisi",
                table: "HalkaArzlar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinansalKarMarji",
                table: "HalkaArzlar",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FonKullanimYerleriJson",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "HalkaAciklikOrani",
                table: "HalkaArzlar",
                type: "decimal(5,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "IskontoOrani",
                table: "HalkaArzlar",
                type: "decimal(5,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SirketOzeti",
                table: "HalkaArzlar",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaahhutlerJson",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ArzBuyukluguTL", "DagitimYontemi", "FinansalBorcluluk", "FinansalCiroArtisi", "FinansalKarMarji", "FonKullanimYerleriJson", "HalkaAciklikOrani", "IskontoOrani", "SirketOzeti", "TaahhutlerJson" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ArzBuyukluguTL", "DagitimYontemi", "FinansalBorcluluk", "FinansalCiroArtisi", "FinansalKarMarji", "FonKullanimYerleriJson", "HalkaAciklikOrani", "IskontoOrani", "SirketOzeti", "TaahhutlerJson" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "HalkaArzlar",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ArzBuyukluguTL", "DagitimYontemi", "FinansalBorcluluk", "FinansalCiroArtisi", "FinansalKarMarji", "FonKullanimYerleriJson", "HalkaAciklikOrani", "IskontoOrani", "SirketOzeti", "TaahhutlerJson" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArzBuyukluguTL",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "DagitimYontemi",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "FinansalBorcluluk",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "FinansalCiroArtisi",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "FinansalKarMarji",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "FonKullanimYerleriJson",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "HalkaAciklikOrani",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "IskontoOrani",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "SirketOzeti",
                table: "HalkaArzlar");

            migrationBuilder.DropColumn(
                name: "TaahhutlerJson",
                table: "HalkaArzlar");

            migrationBuilder.AlterColumn<string>(
                name: "Statu",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "SirketAdi",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Sektor",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KonsorsiyumLideri",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BorsaKodu",
                table: "HalkaArzlar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }
    }
}
