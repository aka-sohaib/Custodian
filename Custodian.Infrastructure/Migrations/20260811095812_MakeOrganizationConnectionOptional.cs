using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Custodian.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeOrganizationConnectionOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_OrganizationConnectionId_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationConnectionId",
                table: "Invoices",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "UnregisteredVendorName",
                table: "Invoices",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrganizationConnectionId_InvoiceNumber",
                table: "Invoices",
                columns: new[] { "OrganizationConnectionId", "InvoiceNumber" },
                unique: true,
                filter: "\"OrganizationConnectionId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_OrganizationConnectionId_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UnregisteredVendorName",
                table: "Invoices");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganizationConnectionId",
                table: "Invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_OrganizationConnectionId_InvoiceNumber",
                table: "Invoices",
                columns: new[] { "OrganizationConnectionId", "InvoiceNumber" },
                unique: true);
        }
    }
}
