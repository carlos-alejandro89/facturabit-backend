using FacturaBit.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FacturaBit.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDBContext))]
[Migration("20261107000000_AddPasswordRecoveryCodes")]
public sealed class AddPasswordRecoveryCodes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "password_recovery_codes",
            columns: table => new
            {
                id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                account_id = table.Column<int>(type: "integer", nullable: false),
                code_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                salt = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                consumed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                failed_attempts = table.Column<int>(type: "integer", nullable: false),
                guid = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_password_recovery_codes", x => x.id);
                table.ForeignKey(
                    name: "fk_password_recovery_codes_accounts_account_id",
                    column: x => x.account_id,
                    principalTable: "accounts",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_password_recovery_codes_account_id_created_at",
            table: "password_recovery_codes",
            columns: new[] { "account_id", "created_at" });

        migrationBuilder.CreateIndex(
            name: "ix_password_recovery_codes_guid",
            table: "password_recovery_codes",
            column: "guid",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "password_recovery_codes");
    }
}
