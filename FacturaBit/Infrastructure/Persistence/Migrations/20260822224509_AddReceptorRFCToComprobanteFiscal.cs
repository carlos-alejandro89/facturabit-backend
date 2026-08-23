using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReceptorRFCToComprobanteFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "folio",
                table: "comprobantes_fiscales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "receptor_nombre",
                table: "comprobantes_fiscales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "receptor_rfc",
                table: "comprobantes_fiscales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "serie_cfdi",
                table: "comprobantes_fiscales",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "folio",
                table: "comprobantes_fiscales");

            migrationBuilder.DropColumn(
                name: "receptor_nombre",
                table: "comprobantes_fiscales");

            migrationBuilder.DropColumn(
                name: "receptor_rfc",
                table: "comprobantes_fiscales");

            migrationBuilder.DropColumn(
                name: "serie_cfdi",
                table: "comprobantes_fiscales");
        }
    }
}
