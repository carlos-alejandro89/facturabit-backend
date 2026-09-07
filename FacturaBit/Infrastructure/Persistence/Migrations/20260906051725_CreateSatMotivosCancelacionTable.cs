using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateSatMotivosCancelacionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sat_motivos_cancelacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cve_motivo = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    motivo_cancelacion = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    requiere_folio_sustitucion = table.Column<bool>(type: "boolean", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sat_motivos_cancelacion", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "sat_motivos_cancelacion",
                columns: new[] { "id", "created_at", "cve_motivo", "guid", "motivo_cancelacion", "requiere_folio_sustitucion", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "01", "2c7833b3-5570-4b97-b626-5d881e491a35", "COMPROBANTES EMITIDOS CON ERRORES CON RELACIÓN", true, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "02", "aad0a49f-fb42-4bbb-9dc7-c8ad097d5ae0", "COMPROBANTES EMITIDOS CON ERRORES SIN RELACIÓN", false, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "03", "09948acc-2002-4cbd-a16e-c6a127515a93", "NO SE LLEVÓ A CABO LA OPERACIÓN", false, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "04", "0d887f32-485c-4d10-92ce-0228a22d4006", "OPERACIÓN NOMINATIVA RELACIONADA EN UNA FACTURA GLOBAL", false, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_sat_motivos_cancelacion_cve_motivo",
                table: "sat_motivos_cancelacion",
                column: "cve_motivo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sat_motivos_cancelacion_guid",
                table: "sat_motivos_cancelacion",
                column: "guid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sat_motivos_cancelacion");
        }
    }
}
