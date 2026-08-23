using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateCertificadoSelloDigitalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "certificados_fiscales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    entidad_fiscal_id = table.Column<int>(type: "integer", nullable: false),
                    archivo_cer = table.Column<string>(type: "text", nullable: false),
                    archivo_key = table.Column<string>(type: "text", nullable: false),
                    password_cert = table.Column<string>(type: "text", nullable: false),
                    valido_desde = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    valido_hasta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tamano_clave = table.Column<int>(type: "integer", nullable: false),
                    numero_serie = table.Column<string>(type: "text", nullable: false),
                    huella_digital = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_certificados_fiscales", x => x.id);
                    table.ForeignKey(
                        name: "fk_certificados_fiscales_entidades_fiscales_entidad_fiscal_id",
                        column: x => x.entidad_fiscal_id,
                        principalTable: "entidades_fiscales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_certificados_fiscales_entidad_fiscal_id",
                table: "certificados_fiscales",
                column: "entidad_fiscal_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificados_fiscales");
        }
    }
}
