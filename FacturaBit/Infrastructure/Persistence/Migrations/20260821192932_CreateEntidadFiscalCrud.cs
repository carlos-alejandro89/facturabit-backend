using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FacturaBit.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateEntidadFiscalCrud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sat_regimen_fiscal", x => x.id);
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
                table: "roles",
                columns: new[] { "id", "created_at", "guid", "nombre", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "f0e47c96-4708-46b6-a914-1830e2cfd252", "SUPERADMIN", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "582d00c4-cd5b-48c0-8c14-0735db2b8866", "ADMIN", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc), "2c04f5e5-9398-4f9a-b880-f0c01476e334", "FACTURISTA", new DateTime(2026, 8, 21, 20, 21, 33, 0, DateTimeKind.Utc) }
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
                name: "ix_entidades_fiscales_guid",
                table: "entidades_fiscales",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_entidades_fiscales_sat_regimen_fiscal_id",
                table: "entidades_fiscales",
                column: "sat_regimen_fiscal_id");

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
                name: "redes_comerciales_entidades_fiscales");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "entidades_fiscales");

            migrationBuilder.DropTable(
                name: "redes_comeciales");

            migrationBuilder.DropTable(
                name: "sat_regimen_fiscal");
        }
    }
}
