using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEsPrepagoToEntidadFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "es_prepago",
                table: "entidades_fiscales",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "es_prepago",
                table: "entidades_fiscales");
        }
    }
}
