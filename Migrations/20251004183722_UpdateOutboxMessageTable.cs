using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbcLettingAgency.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOutboxMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "outbox_messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_outbox_messages_AgencyId",
                schema: "public",
                table: "outbox_messages",
                column: "AgencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_outbox_messages_AgencyId",
                schema: "public",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "outbox_messages");
        }
    }
}
