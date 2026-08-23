using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateComprobanteFiscalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "estatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_estatus = table.Column<string>(type: "text", nullable: false),
                    descripcion_estatus = table.Column<string>(type: "text", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_estatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "comprobantes_fiscales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    estatud_cfdi_id = table.Column<int>(type: "integer", nullable: false),
                    estatus_cfdi_id = table.Column<int>(type: "integer", nullable: false),
                    estatud_cancelacion_id = table.Column<int>(type: "integer", nullable: true),
                    estatus_cancelacion_id = table.Column<int>(type: "integer", nullable: false),
                    entidad_fiscal_id = table.Column<int>(type: "integer", nullable: false),
                    json_request = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    tipo_request = table.Column<string>(type: "text", nullable: false),
                    success = table.Column<bool>(type: "boolean", nullable: false),
                    mensaje = table.Column<string>(type: "text", nullable: false),
                    no_certificado_sat = table.Column<string>(type: "text", nullable: false),
                    no_certificado_emisor = table.Column<string>(type: "text", nullable: false),
                    folio_fiscal = table.Column<string>(type: "text", nullable: false),
                    sello_sat = table.Column<string>(type: "text", nullable: false),
                    sello_emisor = table.Column<string>(type: "text", nullable: false),
                    fecha_timbrado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cadena_original_sat = table.Column<string>(type: "text", nullable: false),
                    path_xml_cfdi = table.Column<string>(type: "text", nullable: true),
                    path_xml_cancelacion = table.Column<string>(type: "text", nullable: true),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comprobantes_fiscales", x => x.id);
                    table.ForeignKey(
                        name: "fk_comprobantes_fiscales_entidades_fiscales_entidad_fiscal_id",
                        column: x => x.entidad_fiscal_id,
                        principalTable: "entidades_fiscales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comprobantes_fiscales_estatus_estatus_cancelacion_id",
                        column: x => x.estatus_cancelacion_id,
                        principalTable: "estatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_comprobantes_fiscales_estatus_estatus_cfdi_id",
                        column: x => x.estatus_cfdi_id,
                        principalTable: "estatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_comprobantes_fiscales_entidad_fiscal_id",
                table: "comprobantes_fiscales",
                column: "entidad_fiscal_id");

            migrationBuilder.CreateIndex(
                name: "ix_comprobantes_fiscales_estatus_cancelacion_id",
                table: "comprobantes_fiscales",
                column: "estatus_cancelacion_id");

            migrationBuilder.CreateIndex(
                name: "ix_comprobantes_fiscales_estatus_cfdi_id",
                table: "comprobantes_fiscales",
                column: "estatus_cfdi_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comprobantes_fiscales");

            migrationBuilder.DropTable(
                name: "estatus");
        }
    }
}
