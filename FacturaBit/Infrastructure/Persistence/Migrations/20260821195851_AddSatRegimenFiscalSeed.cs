using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSatRegimenFiscalSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "guid",
                table: "sat_regimen_fiscal",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "sat_regimen_fiscal",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "sat_regimen_fiscal",
                columns: new[] { "id", "created_at", "cve_regimen", "guid", "regimen_fiscal", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "601", "3a1c1e08-a225-4e0c-8cba-cecd1a7f4612", "GENERAL DE LEY PERSONAS MORALES", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "603", "a04a8875-dcc4-4644-b1ed-e3daf7748c1b", "PERSONAS MORALES CON FINES NO LUCRATIVOS", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "605", "a1c2fe58-bf50-408b-a1ce-4260f52a301a", "SUELDOS Y SALARIOS E INGRESOS ASIMILADOS A SALARIOS", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "606", "141264d9-200b-488f-aec7-3610dbef91ae", "ARRENDAMIENTO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "608", "6ec15ed1-223e-4d85-8dc3-71a8cddd45f7", "DEMÁS INGRESOS", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "609", "f96ac295-f6c8-4a51-a0e3-d47effa0d5c6", "CONSOLIDACIÓN", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "610", "322cb4dc-1265-4fd1-9d43-c0a508d9fc38", "RESIDENTES EN EL EXTRANJERO SIN ESTABLECIMIENTO PERMANENTE EN MÉXICO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "611", "b66af89c-a507-4242-a682-1732ae023d90", "INGRESOS POR DIVIDENDOS (SOCIOS Y ACCIONISTAS)", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "612", "f5039019-d505-4b74-8802-0b8acac233e2", "PERSONAS FÍSICAS CON ACTIVIDADES EMPRESARIALES Y PROFESIONALES", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "614", "572f39b2-c4e0-484b-8acb-3e4b2276f13a", "INGRESOS POR INTERESES", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "616", "7946b1ad-eedb-4b75-bb7e-588ef0e5ef08", "SIN OBLIGACIONES FISCALES", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "620", "ba166528-74db-4bd7-8812-3055d024f840", "SOCIEDADES COOPERATIVAS DE PRODUCCIÓN QUE OPTAN POR DIFERIR SUS INGRESOS", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "621", "632a6438-ef84-4921-9d03-c03b4f2aba76", "INCORPORACIÓN FISCAL", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "622", "d4679262-60a7-4fb0-bf36-13dc4a283474", "ACTIVIDADES AGRÍCOLAS, GANADERAS, SILVÍCOLAS Y PESQUERAS", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "623", "2b3fcec3-7f93-4ed7-8344-37b86932ea35", "OPCIONAL PARA GRUPOS DE SOCIEDADES", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 16, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "624", "e8a4c6b3-6e3f-4558-b283-8304cc925fd9", "COORDINADOS", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 17, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "628", "9620335d-e312-47cb-af7b-96b50deded1f", "HIDROCARBUROS", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 18, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "607", "d1ce0849-d62d-4cad-b2d4-3ff07d818e0f", "RÉGIMEN DE ENAJENACIÓN O ADQUISICIÓN DE BIENES", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 19, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "629", "bf4246d9-9749-4114-aea5-8bf35847df1c", "DE LOS REGÍMENES FISCALES PREFERENTES Y DE LAS EMPRESAS MULTINACIONALES", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 20, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "630", "5bb1e920-0e59-4328-acca-cbc8f67d90ae", "ENAJENACIÓN DE ACCIONES EN BOLSA DE VALORES", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 21, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "615", "248e523e-8685-48e8-9dac-d898e92044ec", "RÉGIMEN DE LOS INGRESOS POR OBTENCIÓN DE PREMIOS", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 22, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "626", "6eff09ee-49bc-49cf-bb3b-f80b1d646613", "RÉGIMEN SIMPLIFICADO DE CONFIANZA", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "sat_regimen_fiscal",
                keyColumn: "id",
                keyValue: 22);

            migrationBuilder.DropColumn(
                name: "guid",
                table: "sat_regimen_fiscal");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "sat_regimen_fiscal");
        }
    }
}
