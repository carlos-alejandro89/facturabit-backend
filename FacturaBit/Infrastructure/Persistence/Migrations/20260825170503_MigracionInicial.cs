using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "canales_pagos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    canal = table.Column<string>(type: "text", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_canales_pagos", x => x.id);
                });

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
                name: "redes_comeciales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_red_comercial = table.Column<string>(type: "text", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_redes_comeciales", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sat_regimen_fiscal",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cve_regimen = table.Column<string>(type: "text", nullable: false),
                    regimen_fiscal = table.Column<string>(type: "text", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sat_regimen_fiscal", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "suscripciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "text", nullable: false),
                    nombre_suscripcion = table.Column<string>(type: "text", nullable: false),
                    caracteristicas = table.Column<string>(type: "jsonb", nullable: false),
                    precio = table.Column<decimal>(type: "numeric", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_suscripciones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    rol_id = table.Column<int>(type: "integer", nullable: false),
                    red_comercial_id = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    telefono = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    password_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_accounts_redes_comeciales_red_comercial_id",
                        column: x => x.red_comercial_id,
                        principalTable: "redes_comeciales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_accounts_roles_rol_id",
                        column: x => x.rol_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "entidades_fiscales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sat_regimen_fiscal_id = table.Column<int>(type: "integer", nullable: true),
                    razon_social = table.Column<string>(type: "text", nullable: false),
                    rfc = table.Column<string>(type: "text", nullable: false),
                    registro_patronal = table.Column<string>(type: "text", nullable: true),
                    calle = table.Column<string>(type: "text", nullable: true),
                    num_ext = table.Column<string>(type: "text", nullable: true),
                    num_int = table.Column<string>(type: "text", nullable: true),
                    colonia = table.Column<string>(type: "text", nullable: true),
                    ciudad = table.Column<string>(type: "text", nullable: true),
                    estado = table.Column<string>(type: "text", nullable: true),
                    codigo_postal = table.Column<string>(type: "text", nullable: false),
                    timbres_disponibles = table.Column<int>(type: "integer", nullable: false),
                    es_prepago = table.Column<bool>(type: "boolean", nullable: false),
                    nombre_comercial = table.Column<string>(type: "text", nullable: true),
                    telefono_comercial = table.Column<string>(type: "text", nullable: true),
                    telefono_comercial2 = table.Column<string>(type: "text", nullable: true),
                    correo_comercial = table.Column<string>(type: "text", nullable: true),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entidades_fiscales", x => x.id);
                    table.ForeignKey(
                        name: "fk_entidades_fiscales_sat_regimen_fiscal_sat_regimen_fiscal_id",
                        column: x => x.sat_regimen_fiscal_id,
                        principalTable: "sat_regimen_fiscal",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ordenes_compra_suscripcion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    folio = table.Column<string>(type: "text", nullable: false),
                    red_comercial_id = table.Column<int>(type: "integer", nullable: false),
                    estatus_id = table.Column<int>(type: "integer", nullable: false),
                    suscripcion_id = table.Column<int>(type: "integer", nullable: false),
                    canal_id = table.Column<int>(type: "integer", nullable: true),
                    subtotal = table.Column<decimal>(type: "numeric", nullable: false),
                    iva = table.Column<decimal>(type: "numeric", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ordenes_compra_suscripcion", x => x.id);
                    table.ForeignKey(
                        name: "fk_ordenes_compra_suscripcion_canales_pagos_canal_id",
                        column: x => x.canal_id,
                        principalTable: "canales_pagos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ordenes_compra_suscripcion_estatus_estatus_id",
                        column: x => x.estatus_id,
                        principalTable: "estatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ordenes_compra_suscripcion_redes_comeciales_red_comercial_id",
                        column: x => x.red_comercial_id,
                        principalTable: "redes_comeciales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ordenes_compra_suscripcion_suscripciones_suscripcion_id",
                        column: x => x.suscripcion_id,
                        principalTable: "suscripciones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateTable(
                name: "comprobantes_fiscales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    estatus_cfdi_id = table.Column<int>(type: "integer", nullable: false),
                    estatus_cancelacion_id = table.Column<int>(type: "integer", nullable: true),
                    entidad_fiscal_id = table.Column<int>(type: "integer", nullable: false),
                    folio = table.Column<string>(type: "text", nullable: false),
                    serie_cfdi = table.Column<string>(type: "text", nullable: true),
                    receptor_rfc = table.Column<string>(type: "text", nullable: false),
                    receptor_nombre = table.Column<string>(type: "text", nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_comprobantes_fiscales_estatus_estatus_cfdi_id",
                        column: x => x.estatus_cfdi_id,
                        principalTable: "estatus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "redes_comerciales_entidades_fiscales",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    red_comercial_id = table.Column<int>(type: "integer", nullable: false),
                    entidad_fiscal_id = table.Column<int>(type: "integer", nullable: false),
                    guid = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_redes_comerciales_entidades_fiscales", x => x.id);
                    table.ForeignKey(
                        name: "fk_redes_comerciales_entidades_fiscales_entidades_fiscales_ent",
                        column: x => x.entidad_fiscal_id,
                        principalTable: "entidades_fiscales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_redes_comerciales_entidades_fiscales_redes_comeciales_red_c",
                        column: x => x.red_comercial_id,
                        principalTable: "redes_comeciales",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    { 8, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "Ocurrió un error al generar, validar o timbrar el XML.", "f83c67c9-6d73-404f-e637-4837e657f911", "ERROR", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "La operación se completó satisfactoriamente.", "a9c3ee31-6bb3-4e4c-8d12-80a455bd635f", "COMPLETADA", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "El importe de la operación fue reembolsado al cliente.", "a00daf3a-eaed-48c9-bc4d-247013e48980", "REEMBOLSADA", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "created_at", "guid", "nombre", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "f0e47c96-4708-46b6-a914-1830e2cfd252", "SUPERADMIN", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "582d00c4-cd5b-48c0-8c14-0735db2b8866", "ADMIN", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "2c04f5e5-9398-4f9a-b880-f0c01476e334", "FACTURISTA", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) }
                });

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

            migrationBuilder.InsertData(
                table: "suscripciones",
                columns: new[] { "id", "caracteristicas", "codigo", "created_at", "guid", "nombre_suscripcion", "precio", "updated_at" },
                values: new object[,]
                {
                    { 1, "{\"TimbresIncluidos\":50,\"EmisoresIncluidos\":1,\"Vigencia\":7,\"UnidadVigencia\":\"D\\u00EDas\",\"SoportePrioritario\":false,\"RespaldoCfdi\":false,\"AccesoApi\":false,\"Recomendado\":false}", "FREE", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "4ea467b7-58a0-47ad-8310-bf5575af103e", "Prueba gratuita 7 días", 0m, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 2, "{\"TimbresIncluidos\":100,\"EmisoresIncluidos\":1,\"Vigencia\":1,\"UnidadVigencia\":\"A\\u00F1os\",\"SoportePrioritario\":false,\"RespaldoCfdi\":false,\"AccesoApi\":false,\"Recomendado\":false}", "INICIAL", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "be8919b2-e842-46ed-a172-746967a639f6", "Plan Inicial", 649m, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 3, "{\"TimbresIncluidos\":200,\"EmisoresIncluidos\":3,\"Vigencia\":1,\"UnidadVigencia\":\"A\\u00F1os\",\"SoportePrioritario\":false,\"RespaldoCfdi\":false,\"AccesoApi\":false,\"Recomendado\":false}", "BASICO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "2317a364-c2f9-4651-ad32-1f2fb4a86ade", "Plan Básico", 1099m, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 4, "{\"TimbresIncluidos\":500,\"EmisoresIncluidos\":10,\"Vigencia\":2,\"UnidadVigencia\":\"A\\u00F1os\",\"SoportePrioritario\":true,\"RespaldoCfdi\":true,\"AccesoApi\":true,\"Recomendado\":true}", "PRO", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "a5c16c34-605d-4b79-ad61-18c53fff0b5a", "Plan PRO", 1999m, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 5, "{\"TimbresIncluidos\":1000,\"EmisoresIncluidos\":-1,\"Vigencia\":2,\"UnidadVigencia\":\"A\\u00F1os\",\"SoportePrioritario\":true,\"RespaldoCfdi\":true,\"AccesoApi\":true,\"Recomendado\":false}", "PREMIUM", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "e5e2bcac-0412-4f73-b2ba-bc5ee1e580d8", "Premium", 3299m, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounts_email",
                table: "accounts",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accounts_guid",
                table: "accounts",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accounts_red_comercial_id",
                table: "accounts",
                column: "red_comercial_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounts_rol_id",
                table: "accounts",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "ix_certificados_fiscales_entidad_fiscal_id",
                table: "certificados_fiscales",
                column: "entidad_fiscal_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_comprobantes_fiscales_guid",
                table: "comprobantes_fiscales",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_entidades_fiscales_guid",
                table: "entidades_fiscales",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_entidades_fiscales_sat_regimen_fiscal_id",
                table: "entidades_fiscales",
                column: "sat_regimen_fiscal_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_compra_suscripcion_canal_id",
                table: "ordenes_compra_suscripcion",
                column: "canal_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_compra_suscripcion_estatus_id",
                table: "ordenes_compra_suscripcion",
                column: "estatus_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_compra_suscripcion_folio",
                table: "ordenes_compra_suscripcion",
                column: "folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_compra_suscripcion_guid",
                table: "ordenes_compra_suscripcion",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_compra_suscripcion_red_comercial_id",
                table: "ordenes_compra_suscripcion",
                column: "red_comercial_id");

            migrationBuilder.CreateIndex(
                name: "ix_ordenes_compra_suscripcion_suscripcion_id",
                table: "ordenes_compra_suscripcion",
                column: "suscripcion_id");

            migrationBuilder.CreateIndex(
                name: "ix_redes_comerciales_entidades_fiscales_entidad_fiscal_id",
                table: "redes_comerciales_entidades_fiscales",
                column: "entidad_fiscal_id");

            migrationBuilder.CreateIndex(
                name: "ix_redes_comerciales_entidades_fiscales_guid",
                table: "redes_comerciales_entidades_fiscales",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_redes_comerciales_entidades_fiscales_red_comercial_id_entid",
                table: "redes_comerciales_entidades_fiscales",
                columns: new[] { "red_comercial_id", "entidad_fiscal_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "certificados_fiscales");

            migrationBuilder.DropTable(
                name: "comprobantes_fiscales");

            migrationBuilder.DropTable(
                name: "ordenes_compra_suscripcion");

            migrationBuilder.DropTable(
                name: "redes_comerciales_entidades_fiscales");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "canales_pagos");

            migrationBuilder.DropTable(
                name: "estatus");

            migrationBuilder.DropTable(
                name: "suscripciones");

            migrationBuilder.DropTable(
                name: "entidades_fiscales");

            migrationBuilder.DropTable(
                name: "redes_comeciales");

            migrationBuilder.DropTable(
                name: "sat_regimen_fiscal");
        }
    }
}
