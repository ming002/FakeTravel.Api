using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeTravel.API.Migrations
{
    public partial class secondmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DepartureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginPrice", "Title", "UpdateTime" },
                values: new object[] { new Guid("2ef334d4-f6ca-4b12-8140-e207cfc9e216"), new DateTime(2022, 3, 1, 16, 32, 50, 822, DateTimeKind.Local).AddTicks(4024), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "shouming", null, null, null, null, 0m, "ceshititle", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("2ef334d4-f6ca-4b12-8140-e207cfc9e216"));
        }
    }
}
