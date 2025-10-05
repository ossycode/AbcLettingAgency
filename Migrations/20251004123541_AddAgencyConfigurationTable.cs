using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbcLettingAgency.Migrations
{
    /// <inheritdoc />
    public partial class AddAgencyConfigurationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branding_LogoBlobId",
                schema: "public",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "Branding_PrimaryColor",
                schema: "public",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "Branding_SecondaryColor",
                schema: "public",
                table: "Agencies");

            migrationBuilder.DropColumn(
                name: "Settings",
                schema: "public",
                table: "Agencies");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.CreateTable(
                name: "AgencyConfigurations",
                schema: "public",
                columns: table => new
                {
                    AgencyId = table.Column<long>(type: "bigint", nullable: false),
                    LogoBlobId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PrimaryColorHex = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    SecondaryColorHex = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    DefaultRentDueDay = table.Column<int>(type: "integer", nullable: false),
                    DefaultRentFrequency = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DefaultCommissionPercent = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    EnableArrearsEmails = table.Column<bool>(type: "boolean", nullable: false),
                    ArrearsEmailDays = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyConfigurations", x => x.AgencyId);
                    table.ForeignKey(
                        name: "FK_AgencyConfigurations_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalSchema: "public",
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgencyConfigurations",
                schema: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AddColumn<string>(
                name: "Branding_LogoBlobId",
                schema: "public",
                table: "Agencies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Branding_PrimaryColor",
                schema: "public",
                table: "Agencies",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Branding_SecondaryColor",
                schema: "public",
                table: "Agencies",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                schema: "public",
                table: "Agencies",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }
    }
}
