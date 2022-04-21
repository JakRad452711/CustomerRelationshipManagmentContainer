using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerRelationshipManagment.Migrations
{
    public partial class ChangedSomeColumnNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssociatedCompany",
                table: "TradeNotes",
                newName: "AssociatedCompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssociatedCompanyId",
                table: "TradeNotes",
                newName: "AssociatedCompany");
        }
    }
}
