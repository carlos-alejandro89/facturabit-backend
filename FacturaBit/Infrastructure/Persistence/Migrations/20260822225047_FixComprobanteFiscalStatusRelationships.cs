using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixComprobanteFiscalStatusRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comprobantes_fiscales_estatus_estatus_cancelacion_id",
                table: "comprobantes_fiscales");

            migrationBuilder.DropForeignKey(
                name: "fk_comprobantes_fiscales_estatus_estatus_cfdi_id",
                table: "comprobantes_fiscales");

            migrationBuilder.DropColumn(
                name: "estatud_cancelacion_id",
                table: "comprobantes_fiscales");

            migrationBuilder.DropColumn(
                name: "estatud_cfdi_id",
                table: "comprobantes_fiscales");

            migrationBuilder.AlterColumn<int>(
                name: "estatus_cancelacion_id",
                table: "comprobantes_fiscales",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "estatus",
                columns: new[] { "id", "created_at", "descripcion_estatus", "guid", "nombre_estatus", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "Se recibe solicitud de timbrado JSON.", "f0e47c96-4708-46b6-a914-1830e2cfd252", "SOLICITUD TIMBRADO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "Comprobante timbrado y activo fiscalmente ante el SAT.", "582d00c4-cd5b-48c0-8c14-0735db2b8866", "VIGENTE", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "Comprobante anulado formalmente ante el SAT.", "a38f12d4-1e28-4b9a-b182-9382f102a4bc", "CANCELADO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "Cancela de inmediato sin pedir permiso al receptor (por monto, tipo de factura o tiempo transcurrido).", "b49e23e5-2f39-4c0b-a293-0493a213b5cd", "SIN ACEPTABILIDAD", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "Solicitada. Se envió la notificación al Buzón Tributario del receptor y está en espera de su respuesta.", "c50f34f6-3a40-4d1c-b304-1504b324c6de", "EN PROCESO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "El receptor aprobó la cancelación desde su Buzón Tributario.", "d61a45a7-4b51-4e2d-c415-2615c435d7ef", "ACEPTADO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "El receptor negó la cancelación. La factura se mantiene Vigente.", "e72b56b8-5c62-4f3e-d526-3726d546e800", "RECHAZADO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "Ocurrió un error al generar, validar o timbrar el XML.", "f83c67c9-6d73-404f-e637-4837e657f911", "ERROR", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_comprobantes_fiscales_guid",
                table: "comprobantes_fiscales",
                column: "guid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_comprobantes_fiscales_estatus_estatus_cancelacion_id",
                table: "comprobantes_fiscales",
                column: "estatus_cancelacion_id",
                principalTable: "estatus",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_comprobantes_fiscales_estatus_estatus_cfdi_id",
                table: "comprobantes_fiscales",
                column: "estatus_cfdi_id",
                principalTable: "estatus",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comprobantes_fiscales_estatus_estatus_cancelacion_id",
                table: "comprobantes_fiscales");

            migrationBuilder.DropForeignKey(
                name: "fk_comprobantes_fiscales_estatus_estatus_cfdi_id",
                table: "comprobantes_fiscales");

            migrationBuilder.DropIndex(
                name: "ix_comprobantes_fiscales_guid",
                table: "comprobantes_fiscales");

            migrationBuilder.DeleteData(
                table: "estatus",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "estatus",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "estatus",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "estatus",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "estatus",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "estatus",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "estatus",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "estatus",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.AlterColumn<int>(
                name: "estatus_cancelacion_id",
                table: "comprobantes_fiscales",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "estatud_cancelacion_id",
                table: "comprobantes_fiscales",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "estatud_cfdi_id",
                table: "comprobantes_fiscales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "fk_comprobantes_fiscales_estatus_estatus_cancelacion_id",
                table: "comprobantes_fiscales",
                column: "estatus_cancelacion_id",
                principalTable: "estatus",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comprobantes_fiscales_estatus_estatus_cfdi_id",
                table: "comprobantes_fiscales",
                column: "estatus_cfdi_id",
                principalTable: "estatus",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
