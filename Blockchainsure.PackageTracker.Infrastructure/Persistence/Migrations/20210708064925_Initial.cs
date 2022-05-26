using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Blockchainsure.PackageTracker.Infrastructure.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Zip = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAddresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingNumber = table.Column<string>(type: "text", nullable: false),
                    Carrier = table.Column<string>(type: "text", nullable: false),
                    FromId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToId = table.Column<Guid>(type: "uuid", nullable: true),
                    Eta = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OriginalEta = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Raw = table.Column<string>(type: "jsonb", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_ShippingAddresses_FromId",
                        column: x => x.FromId,
                        principalTable: "ShippingAddresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Packages_ShippingAddresses_ToId",
                        column: x => x.ToId,
                        principalTable: "ShippingAddresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PackageActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Substatus = table.Column<string>(type: "text", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    StatusDetails = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Modified = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageActivities_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageActivities_ShippingAddresses_LocationId",
                        column: x => x.LocationId,
                        principalTable: "ShippingAddresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageActivities_LocationId",
                table: "PackageActivities",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageActivities_PackageId",
                table: "PackageActivities",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_FromId",
                table: "Packages",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ToId",
                table: "Packages",
                column: "ToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageActivities");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "ShippingAddresses");
        }
    }
}
