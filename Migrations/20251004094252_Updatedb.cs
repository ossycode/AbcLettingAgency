using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AbcLettingAgency.Migrations
{
    /// <inheritdoc />
    public partial class Updatedb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Landlords_LandlordId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Properties_PropertyId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Tenancies_TenancyId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Tenants_TenantId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Invoices_InvoiceId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Properties_PropertyId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Tenancies_TenancyId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Tenants_TenantId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Properties_PropertyId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Tenancies_TenancyId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Invoices_InvoiceId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Properties_PropertyId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_TenancyTenants_Tenants_TenantId",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Email",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_LastName_FirstName",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_Phone",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_TenancyTenants_TenancyId_IsPrimary",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenancies_PropertyId_Status",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropIndex(
                name: "IX_RentReceipts_ReceivedAt",
                schema: "public",
                table: "RentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_RentReceipts_TenancyId",
                schema: "public",
                table: "RentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_RentCharges_TenancyId",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropIndex(
                name: "IX_RentCharges_TenancyId_DueDate_Status",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropIndex(
                name: "IX_Properties_Code",
                schema: "public",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceJobs_PropertyId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "Updates",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Updates",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "Updates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "Updates",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "Tenants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "Tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "Tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "TenancyTenants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "TenancyTenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "TenancyTenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "TenancyTenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "Tenancies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Tenancies",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "Tenancies",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "Tenancies",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "public",
                table: "RentReceipts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "RentReceipts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "RentReceipts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "RentReceipts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "RentReceipts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionDue",
                schema: "public",
                table: "RentCharges",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountAfterCommission",
                schema: "public",
                table: "RentCharges",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "public",
                table: "RentCharges",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "RentCharges",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "RentCharges",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "RentCharges",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "RentCharges",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "Properties",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Properties",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "Properties",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "Properties",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                schema: "public",
                table: "MaintenanceJobs",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "MaintenanceJobs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "MaintenanceJobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "MaintenanceJobs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "MaintenanceJobs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "Landlords",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Landlords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "Landlords",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "Landlords",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "VatAmount",
                schema: "public",
                table: "Invoices",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "NetAmount",
                schema: "public",
                table: "Invoices",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "GrossAmount",
                schema: "public",
                table: "Invoices",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "Invoices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Invoices",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "Invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Documents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "Documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "public",
                table: "ClientLedgers",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,2)",
                oldPrecision: 12,
                oldScale: 2);

            migrationBuilder.AddColumn<long>(
                name: "AgencyId",
                schema: "public",
                table: "ClientLedgers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                schema: "public",
                table: "ClientLedgers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserDeletedId",
                schema: "public",
                table: "ClientLedgers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserUpdatedId",
                schema: "public",
                table: "ClientLedgers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Agencies",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrgId = table.Column<long>(type: "bigint", nullable: true),
                    ParentAgencyId = table.Column<long>(type: "bigint", nullable: true),
                    Slug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LegalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PhoneNumber2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Address_Line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address_Line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address_Line3 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address_City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address_Region = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address_PostCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Address_CountryCode = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    TimeZone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CompanyNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    VatNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Branding_LogoBlobId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Branding_PrimaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Branding_SecondaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    BillingPlan = table.Column<string>(type: "text", nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserDeletedId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserUpdatedId = table.Column<Guid>(type: "uuid", nullable: true),
                    Settings = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agencies_Agencies_ParentAgencyId",
                        column: x => x.ParentAgencyId,
                        principalSchema: "public",
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgencyGroups",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Slug = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserDeletedId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserUpdatedId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillingInvoices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgencyId = table.Column<long>(type: "bigint", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AmountDue = table.Column<long>(type: "bigint", nullable: false),
                    AmountPaid = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HostedInvoiceUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PdfUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserDeletedId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserUpdatedId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingInvoices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgencyUsers",
                schema: "public",
                columns: table => new
                {
                    AgencyId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyUsers", x => new { x.AgencyId, x.UserId });
                    table.ForeignKey(
                        name: "FK_AgencyUsers_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalSchema: "public",
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgencyUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillingAccounts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgencyId = table.Column<long>(type: "bigint", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StripeCustomerId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BillingEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    TaxNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TaxExempt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserDeletedId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserUpdatedId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingAccounts_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalSchema: "public",
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgencyGroupMemberships",
                schema: "public",
                columns: table => new
                {
                    AgencyId = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgencyGroupMemberships", x => new { x.AgencyId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_AgencyGroupMemberships_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalSchema: "public",
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgencyGroupMemberships_AgencyGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "public",
                        principalTable: "AgencyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillingSubscriptions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AgencyId = table.Column<long>(type: "bigint", nullable: false),
                    BillingAccountId = table.Column<long>(type: "bigint", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ProductId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PriceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Interval = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    UnitAmount = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Seats = table.Column<int>(type: "integer", nullable: false),
                    CurrentPeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TrialEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancelAtPeriodEnd = table.Column<bool>(type: "boolean", nullable: false),
                    CanceledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserDeletedId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserUpdatedId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingSubscriptions_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalSchema: "public",
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillingSubscriptions_BillingAccounts_BillingAccountId",
                        column: x => x.BillingAccountId,
                        principalSchema: "public",
                        principalTable: "BillingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillingSubscriptionItems",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubscriptionId = table.Column<long>(type: "bigint", nullable: false),
                    PriceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserDeletedId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserUpdatedId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingSubscriptionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingSubscriptionItems_BillingSubscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "public",
                        principalTable: "BillingSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Updates_AgencyId",
                schema: "public",
                table: "Updates",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_AgencyId",
                schema: "public",
                table: "Tenants",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_AgencyId_Email",
                schema: "public",
                table: "Tenants",
                columns: new[] { "AgencyId", "Email" },
                unique: true,
                filter: "\"Email\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_AgencyId_Phone",
                schema: "public",
                table: "Tenants",
                columns: new[] { "AgencyId", "Phone" },
                filter: "\"Phone\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_TenancyTenants_AgencyId",
                schema: "public",
                table: "TenancyTenants",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_TenancyTenants_TenancyId",
                schema: "public",
                table: "TenancyTenants",
                column: "TenancyId",
                unique: true,
                filter: "\"IsPrimary\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_Tenancies_AgencyId",
                schema: "public",
                table: "Tenancies",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenancies_AgencyId_Status",
                schema: "public",
                table: "Tenancies",
                columns: new[] { "AgencyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenancies_PropertyId",
                schema: "public",
                table: "Tenancies",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentReceipts_AgencyId",
                schema: "public",
                table: "RentReceipts",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentCharges_AgencyId",
                schema: "public",
                table: "RentCharges",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentCharges_TenancyId_DueDate",
                schema: "public",
                table: "RentCharges",
                columns: new[] { "TenancyId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RentCharges_TenancyId_PeriodStart_PeriodEnd",
                schema: "public",
                table: "RentCharges",
                columns: new[] { "TenancyId", "PeriodStart", "PeriodEnd" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_AgencyId",
                schema: "public",
                table: "Properties",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_AgencyId_Code",
                schema: "public",
                table: "Properties",
                columns: new[] { "AgencyId", "Code" },
                unique: true,
                filter: "\"Code\" <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_AgencyId",
                schema: "public",
                table: "MaintenanceJobs",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_PropertyId_Status",
                schema: "public",
                table: "MaintenanceJobs",
                columns: new[] { "PropertyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Landlords_AgencyId_Email",
                schema: "public",
                table: "Landlords",
                columns: new[] { "AgencyId", "Email" },
                unique: true,
                filter: "\"Email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Landlords_AgencyId_Name",
                schema: "public",
                table: "Landlords",
                columns: new[] { "AgencyId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_AgencyId",
                schema: "public",
                table: "Invoices",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_IssueDate_Status",
                schema: "public",
                table: "Invoices",
                columns: new[] { "IssueDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_AgencyId",
                schema: "public",
                table: "Documents",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLedgers_AgencyId",
                schema: "public",
                table: "ClientLedgers",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLedgers_EntryType_OccurredAt",
                schema: "public",
                table: "ClientLedgers",
                columns: new[] { "EntryType", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_Email",
                schema: "public",
                table: "Agencies",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_OrgId_Name",
                schema: "public",
                table: "Agencies",
                columns: new[] { "OrgId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_ParentAgencyId",
                schema: "public",
                table: "Agencies",
                column: "ParentAgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Agencies_Slug",
                schema: "public",
                table: "Agencies",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgencyGroupMemberships_GroupId",
                schema: "public",
                table: "AgencyGroupMemberships",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyGroups_Slug",
                schema: "public",
                table: "AgencyGroups",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgencyUsers_AgencyId",
                schema: "public",
                table: "AgencyUsers",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyUsers_UserId",
                schema: "public",
                table: "AgencyUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AgencyUsers_UserId_IsActive",
                schema: "public",
                table: "AgencyUsers",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_BillingAccounts_AgencyId",
                schema: "public",
                table: "BillingAccounts",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingAccounts_Provider_StripeCustomerId",
                schema: "public",
                table: "BillingAccounts",
                columns: new[] { "Provider", "StripeCustomerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingInvoices_AgencyId_IssuedAt",
                schema: "public",
                table: "BillingInvoices",
                columns: new[] { "AgencyId", "IssuedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BillingInvoices_Number",
                schema: "public",
                table: "BillingInvoices",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingInvoices_Provider_ExternalId",
                schema: "public",
                table: "BillingInvoices",
                columns: new[] { "Provider", "ExternalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillingSubscriptionItems_SubscriptionId",
                schema: "public",
                table: "BillingSubscriptionItems",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingSubscriptions_AgencyId",
                schema: "public",
                table: "BillingSubscriptions",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingSubscriptions_BillingAccountId",
                schema: "public",
                table: "BillingSubscriptions",
                column: "BillingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingSubscriptions_Provider_ExternalId",
                schema: "public",
                table: "BillingSubscriptions",
                columns: new[] { "Provider", "ExternalId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Agencies_AgencyId",
                schema: "public",
                table: "ClientLedgers",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Landlords_LandlordId",
                schema: "public",
                table: "ClientLedgers",
                column: "LandlordId",
                principalSchema: "public",
                principalTable: "Landlords",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Properties_PropertyId",
                schema: "public",
                table: "ClientLedgers",
                column: "PropertyId",
                principalSchema: "public",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Tenancies_TenancyId",
                schema: "public",
                table: "ClientLedgers",
                column: "TenancyId",
                principalSchema: "public",
                principalTable: "Tenancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Tenants_TenantId",
                schema: "public",
                table: "ClientLedgers",
                column: "TenantId",
                principalSchema: "public",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Agencies_AgencyId",
                schema: "public",
                table: "Documents",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Invoices_InvoiceId",
                schema: "public",
                table: "Documents",
                column: "InvoiceId",
                principalSchema: "public",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Properties_PropertyId",
                schema: "public",
                table: "Documents",
                column: "PropertyId",
                principalSchema: "public",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Tenancies_TenancyId",
                schema: "public",
                table: "Documents",
                column: "TenancyId",
                principalSchema: "public",
                principalTable: "Tenancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Tenants_TenantId",
                schema: "public",
                table: "Documents",
                column: "TenantId",
                principalSchema: "public",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Agencies_AgencyId",
                schema: "public",
                table: "Invoices",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Properties_PropertyId",
                schema: "public",
                table: "Invoices",
                column: "PropertyId",
                principalSchema: "public",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Tenancies_TenancyId",
                schema: "public",
                table: "Invoices",
                column: "TenancyId",
                principalSchema: "public",
                principalTable: "Tenancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Landlords_Agencies_AgencyId",
                schema: "public",
                table: "Landlords",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Agencies_AgencyId",
                schema: "public",
                table: "MaintenanceJobs",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Invoices_InvoiceId",
                schema: "public",
                table: "MaintenanceJobs",
                column: "InvoiceId",
                principalSchema: "public",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Properties_PropertyId",
                schema: "public",
                table: "MaintenanceJobs",
                column: "PropertyId",
                principalSchema: "public",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Agencies_AgencyId",
                schema: "public",
                table: "Properties",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RentCharges_Agencies_AgencyId",
                schema: "public",
                table: "RentCharges",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RentReceipts_Agencies_AgencyId",
                schema: "public",
                table: "RentReceipts",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenancies_Agencies_AgencyId",
                schema: "public",
                table: "Tenancies",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenancyTenants_Agencies_AgencyId",
                schema: "public",
                table: "TenancyTenants",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenancyTenants_Tenants_TenantId",
                schema: "public",
                table: "TenancyTenants",
                column: "TenantId",
                principalSchema: "public",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Agencies_AgencyId",
                schema: "public",
                table: "Tenants",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Updates_Agencies_AgencyId",
                schema: "public",
                table: "Updates",
                column: "AgencyId",
                principalSchema: "public",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Agencies_AgencyId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Landlords_LandlordId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Properties_PropertyId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Tenancies_TenancyId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientLedgers_Tenants_TenantId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Agencies_AgencyId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Invoices_InvoiceId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Properties_PropertyId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Tenancies_TenancyId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Tenants_TenantId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Agencies_AgencyId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Properties_PropertyId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Tenancies_TenancyId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Landlords_Agencies_AgencyId",
                schema: "public",
                table: "Landlords");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Agencies_AgencyId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Invoices_InvoiceId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceJobs_Properties_PropertyId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Agencies_AgencyId",
                schema: "public",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_RentCharges_Agencies_AgencyId",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_RentReceipts_Agencies_AgencyId",
                schema: "public",
                table: "RentReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenancies_Agencies_AgencyId",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropForeignKey(
                name: "FK_TenancyTenants_Agencies_AgencyId",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_TenancyTenants_Tenants_TenantId",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Agencies_AgencyId",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Updates_Agencies_AgencyId",
                schema: "public",
                table: "Updates");

            migrationBuilder.DropTable(
                name: "AgencyGroupMemberships",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AgencyUsers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BillingInvoices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BillingSubscriptionItems",
                schema: "public");

            migrationBuilder.DropTable(
                name: "AgencyGroups",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BillingSubscriptions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "BillingAccounts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Agencies",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "IX_Updates_AgencyId",
                schema: "public",
                table: "Updates");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_AgencyId",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_AgencyId_Email",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_AgencyId_Phone",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_TenancyTenants_AgencyId",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropIndex(
                name: "IX_TenancyTenants_TenancyId",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenancies_AgencyId",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropIndex(
                name: "IX_Tenancies_AgencyId_Status",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropIndex(
                name: "IX_Tenancies_PropertyId",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropIndex(
                name: "IX_RentReceipts_AgencyId",
                schema: "public",
                table: "RentReceipts");

            migrationBuilder.DropIndex(
                name: "IX_RentCharges_AgencyId",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropIndex(
                name: "IX_RentCharges_TenancyId_DueDate",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropIndex(
                name: "IX_RentCharges_TenancyId_PeriodStart_PeriodEnd",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropIndex(
                name: "IX_Properties_AgencyId",
                schema: "public",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_AgencyId_Code",
                schema: "public",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceJobs_AgencyId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceJobs_PropertyId_Status",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropIndex(
                name: "IX_Landlords_AgencyId_Email",
                schema: "public",
                table: "Landlords");

            migrationBuilder.DropIndex(
                name: "IX_Landlords_AgencyId_Name",
                schema: "public",
                table: "Landlords");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_AgencyId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_IssueDate_Status",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Documents_AgencyId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_ClientLedgers_AgencyId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropIndex(
                name: "IX_ClientLedgers_EntryType_OccurredAt",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "Updates");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Updates");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "Updates");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "Updates");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "TenancyTenants");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "Tenancies");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "RentReceipts");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "RentReceipts");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "RentReceipts");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "RentReceipts");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "RentCharges");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "MaintenanceJobs");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "Landlords");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Landlords");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "Landlords");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "Landlords");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropColumn(
                name: "UserDeletedId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.DropColumn(
                name: "UserUpdatedId",
                schema: "public",
                table: "ClientLedgers");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "public",
                table: "RentReceipts",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "CommissionDue",
                schema: "public",
                table: "RentCharges",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AmountAfterCommission",
                schema: "public",
                table: "RentCharges",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "public",
                table: "RentCharges",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                schema: "public",
                table: "MaintenanceJobs",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "VatAmount",
                schema: "public",
                table: "Invoices",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "NetAmount",
                schema: "public",
                table: "Invoices",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "GrossAmount",
                schema: "public",
                table: "Invoices",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "public",
                table: "ClientLedgers",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Email",
                schema: "public",
                table: "Tenants",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_LastName_FirstName",
                schema: "public",
                table: "Tenants",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Phone",
                schema: "public",
                table: "Tenants",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_TenancyTenants_TenancyId_IsPrimary",
                schema: "public",
                table: "TenancyTenants",
                columns: new[] { "TenancyId", "IsPrimary" },
                unique: true,
                filter: "\"IsPrimary\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_Tenancies_PropertyId_Status",
                schema: "public",
                table: "Tenancies",
                columns: new[] { "PropertyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RentReceipts_ReceivedAt",
                schema: "public",
                table: "RentReceipts",
                column: "ReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RentReceipts_TenancyId",
                schema: "public",
                table: "RentReceipts",
                column: "TenancyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentCharges_TenancyId",
                schema: "public",
                table: "RentCharges",
                column: "TenancyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentCharges_TenancyId_DueDate_Status",
                schema: "public",
                table: "RentCharges",
                columns: new[] { "TenancyId", "DueDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Code",
                schema: "public",
                table: "Properties",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceJobs_PropertyId",
                schema: "public",
                table: "MaintenanceJobs",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Landlords_LandlordId",
                schema: "public",
                table: "ClientLedgers",
                column: "LandlordId",
                principalSchema: "public",
                principalTable: "Landlords",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Properties_PropertyId",
                schema: "public",
                table: "ClientLedgers",
                column: "PropertyId",
                principalSchema: "public",
                principalTable: "Properties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Tenancies_TenancyId",
                schema: "public",
                table: "ClientLedgers",
                column: "TenancyId",
                principalSchema: "public",
                principalTable: "Tenancies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientLedgers_Tenants_TenantId",
                schema: "public",
                table: "ClientLedgers",
                column: "TenantId",
                principalSchema: "public",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Invoices_InvoiceId",
                schema: "public",
                table: "Documents",
                column: "InvoiceId",
                principalSchema: "public",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Properties_PropertyId",
                schema: "public",
                table: "Documents",
                column: "PropertyId",
                principalSchema: "public",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Tenancies_TenancyId",
                schema: "public",
                table: "Documents",
                column: "TenancyId",
                principalSchema: "public",
                principalTable: "Tenancies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Tenants_TenantId",
                schema: "public",
                table: "Documents",
                column: "TenantId",
                principalSchema: "public",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Properties_PropertyId",
                schema: "public",
                table: "Invoices",
                column: "PropertyId",
                principalSchema: "public",
                principalTable: "Properties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Tenancies_TenancyId",
                schema: "public",
                table: "Invoices",
                column: "TenancyId",
                principalSchema: "public",
                principalTable: "Tenancies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Invoices_InvoiceId",
                schema: "public",
                table: "MaintenanceJobs",
                column: "InvoiceId",
                principalSchema: "public",
                principalTable: "Invoices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceJobs_Properties_PropertyId",
                schema: "public",
                table: "MaintenanceJobs",
                column: "PropertyId",
                principalSchema: "public",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenancyTenants_Tenants_TenantId",
                schema: "public",
                table: "TenancyTenants",
                column: "TenantId",
                principalSchema: "public",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
