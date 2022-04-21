using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerRelationshipManagment.Migrations
{
    public partial class EditedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Users_UserModel",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactPeople_Users_UserModel",
                table: "ContactPeople");

            migrationBuilder.DropForeignKey(
                name: "FK_TradeNotes_Users_UserModel",
                table: "TradeNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleModel",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleModel",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TradeNotes_UserModel",
                table: "TradeNotes");

            migrationBuilder.DropIndex(
                name: "IX_ContactPeople_UserModel",
                table: "ContactPeople");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserModel",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "RoleModel",
                table: "Users",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "UserModel",
                table: "TradeNotes",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "UserModel",
                table: "ContactPeople",
                newName: "InviterId");

            migrationBuilder.RenameColumn(
                name: "UserModel",
                table: "Companies",
                newName: "CreatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Users",
                newName: "RoleModel");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "TradeNotes",
                newName: "UserModel");

            migrationBuilder.RenameColumn(
                name: "InviterId",
                table: "ContactPeople",
                newName: "UserModel");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Companies",
                newName: "UserModel");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleModel",
                table: "Users",
                column: "RoleModel");

            migrationBuilder.CreateIndex(
                name: "IX_TradeNotes_UserModel",
                table: "TradeNotes",
                column: "UserModel");

            migrationBuilder.CreateIndex(
                name: "IX_ContactPeople_UserModel",
                table: "ContactPeople",
                column: "UserModel");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UserModel",
                table: "Companies",
                column: "UserModel");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Users_UserModel",
                table: "Companies",
                column: "UserModel",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactPeople_Users_UserModel",
                table: "ContactPeople",
                column: "UserModel",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TradeNotes_Users_UserModel",
                table: "TradeNotes",
                column: "UserModel",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleModel",
                table: "Users",
                column: "RoleModel",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
