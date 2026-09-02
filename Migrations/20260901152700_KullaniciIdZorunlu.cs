using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfoyTakipAPI.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciIdZorunlu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Önce eski boş kayıtları senin Admin hesabına (ID: 3) bağlıyoruz
            migrationBuilder.Sql("UPDATE Varliklar SET KullaniciId = '3' WHERE KullaniciId IS NULL");

            // 2. Ardından kolonu kalıcı olarak zorunlu (NOT NULL) hale getiriyoruz
            migrationBuilder.AlterColumn<string>(
                name: "KullaniciId",
                table: "Varliklar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
