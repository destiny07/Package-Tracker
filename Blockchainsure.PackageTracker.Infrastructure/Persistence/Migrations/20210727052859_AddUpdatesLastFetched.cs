using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Blockchainsure.PackageTracker.Infrastructure.Persistence.Migrations
{
    public partial class AddUpdatesLastFetched : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatesLastFetched",
                table: "Packages",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatesLastFetched",
                table: "Packages");
        }
    }
}
