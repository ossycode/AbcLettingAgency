using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AbcLettingAgency.Migrations
{
    /// <inheritdoc />
    public partial class AddOutBoxMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "public",
                table: "Tenants",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "public",
                table: "RentCharges",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DedupKey = table.Column<string>(type: "text", nullable: true),
                    Payload = table.Column<string>(type: "jsonb", nullable: false),
                    OccurredUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LockedUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ProcessedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeadLetteredUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Error = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_DeadLetteredUtc",
                schema: "public",
                table: "outbox_messages",
                column: "DeadLetteredUtc");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_ProcessedUtc",
                schema: "public",
                table: "outbox_messages",
                column: "ProcessedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_Type_DedupKey",
                schema: "public",
                table: "outbox_messages",
                columns: new[] { "Type", "DedupKey" },
                unique: true,
                filter: "\"DedupKey\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "public");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "public",
                table: "RentCharges");
        }
    }
}
