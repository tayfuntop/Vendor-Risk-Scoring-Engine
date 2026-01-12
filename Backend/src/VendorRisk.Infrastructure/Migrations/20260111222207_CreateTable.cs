using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VendorRisk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendorProfiles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    financialhealth = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    slauptime = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    majorincidents = table.Column<int>(type: "integer", nullable: false),
                    securitycerts = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendorprofiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "documentstatus",
                columns: table => new
                {
                    VendorProfileId = table.Column<int>(type: "integer", nullable: false),
                    ContractValid = table.Column<bool>(type: "boolean", nullable: false),
                    PrivacyPolicyValid = table.Column<bool>(type: "boolean", nullable: false),
                    PentestReportValid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documentstatus", x => x.VendorProfileId);
                    table.ForeignKey(
                        name: "FK_documentstatus_VendorProfiles_VendorProfileId",
                        column: x => x.VendorProfileId,
                        principalTable: "VendorProfiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiskAssessments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vendorid = table.Column<int>(type: "integer", nullable: false),
                    riskscore = table.Column<decimal>(type: "numeric(5,4)", precision: 5, scale: 4, nullable: false),
                    risklevel = table.Column<string>(type: "text", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: false),
                    assessedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_riskassessments", x => x.id);
                    table.ForeignKey(
                        name: "FK_RiskAssessments_VendorProfiles_vendorid",
                        column: x => x.vendorid,
                        principalTable: "VendorProfiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_assessedat",
                table: "RiskAssessments",
                column: "assessedat");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_risklevel",
                table: "RiskAssessments",
                column: "risklevel");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessments_vendorid_assessedat",
                table: "RiskAssessments",
                columns: new[] { "vendorid", "assessedat" });

            migrationBuilder.CreateIndex(
                name: "ix_riskassessments_vendorid",
                table: "RiskAssessments",
                column: "vendorid");

            migrationBuilder.CreateIndex(
                name: "IX_VendorProfiles_name",
                table: "VendorProfiles",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "documentstatus");

            migrationBuilder.DropTable(
                name: "RiskAssessments");

            migrationBuilder.DropTable(
                name: "VendorProfiles");
        }
    }
}
