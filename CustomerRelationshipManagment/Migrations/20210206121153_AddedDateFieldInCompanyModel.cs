using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerRelationshipManagment.Migrations
{
    public partial class AddedDateFieldInCompanyModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WhenAdded",
                table: "Companies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhenAdded",
                table: "Companies");
        }
    }
}
