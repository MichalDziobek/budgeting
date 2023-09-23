using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixDbModelIssueFromLackOfPropertiesInDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_User_OwnerId",
                table: "Budget");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetEntry_BudgetEntryCategory_CategoryId",
                table: "BudgetEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetEntry_Budget_BudgetId",
                table: "BudgetEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetUser_Budget_BudgetsSharedWithThisUserId",
                table: "BudgetUser");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetUser_User_UsersWithSharedAccessId",
                table: "BudgetUser");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedBudget_Budget_BudgetId",
                table: "SharedBudget");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedBudget_User_UserId",
                table: "SharedBudget");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedBudget",
                table: "SharedBudget");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetEntry",
                table: "BudgetEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budget",
                table: "Budget");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "SharedBudget",
                newName: "SharedBudgets");

            migrationBuilder.RenameTable(
                name: "BudgetEntry",
                newName: "BudgetEntries");

            migrationBuilder.RenameTable(
                name: "Budget",
                newName: "Budgets");

            migrationBuilder.RenameIndex(
                name: "IX_SharedBudget_UserId",
                table: "SharedBudgets",
                newName: "IX_SharedBudgets_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetEntry_CategoryId",
                table: "BudgetEntries",
                newName: "IX_BudgetEntries_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetEntry_BudgetId",
                table: "BudgetEntries",
                newName: "IX_BudgetEntries_BudgetId");

            migrationBuilder.RenameIndex(
                name: "IX_Budget_OwnerId",
                table: "Budgets",
                newName: "IX_Budgets_OwnerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedBudgets",
                table: "SharedBudgets",
                columns: new[] { "BudgetId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetEntries",
                table: "BudgetEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetEntries_BudgetEntryCategory_CategoryId",
                table: "BudgetEntries",
                column: "CategoryId",
                principalTable: "BudgetEntryCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetEntries_Budgets_BudgetId",
                table: "BudgetEntries",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_Users_OwnerId",
                table: "Budgets",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetUser_Budgets_BudgetsSharedWithThisUserId",
                table: "BudgetUser",
                column: "BudgetsSharedWithThisUserId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetUser_Users_UsersWithSharedAccessId",
                table: "BudgetUser",
                column: "UsersWithSharedAccessId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedBudgets_Budgets_BudgetId",
                table: "SharedBudgets",
                column: "BudgetId",
                principalTable: "Budgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedBudgets_Users_UserId",
                table: "SharedBudgets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetEntries_BudgetEntryCategory_CategoryId",
                table: "BudgetEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetEntries_Budgets_BudgetId",
                table: "BudgetEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Users_OwnerId",
                table: "Budgets");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetUser_Budgets_BudgetsSharedWithThisUserId",
                table: "BudgetUser");

            migrationBuilder.DropForeignKey(
                name: "FK_BudgetUser_Users_UsersWithSharedAccessId",
                table: "BudgetUser");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedBudgets_Budgets_BudgetId",
                table: "SharedBudgets");

            migrationBuilder.DropForeignKey(
                name: "FK_SharedBudgets_Users_UserId",
                table: "SharedBudgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SharedBudgets",
                table: "SharedBudgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Budgets",
                table: "Budgets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BudgetEntries",
                table: "BudgetEntries");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "SharedBudgets",
                newName: "SharedBudget");

            migrationBuilder.RenameTable(
                name: "Budgets",
                newName: "Budget");

            migrationBuilder.RenameTable(
                name: "BudgetEntries",
                newName: "BudgetEntry");

            migrationBuilder.RenameIndex(
                name: "IX_SharedBudgets_UserId",
                table: "SharedBudget",
                newName: "IX_SharedBudget_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Budgets_OwnerId",
                table: "Budget",
                newName: "IX_Budget_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetEntries_CategoryId",
                table: "BudgetEntry",
                newName: "IX_BudgetEntry_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_BudgetEntries_BudgetId",
                table: "BudgetEntry",
                newName: "IX_BudgetEntry_BudgetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SharedBudget",
                table: "SharedBudget",
                columns: new[] { "BudgetId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Budget",
                table: "Budget",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BudgetEntry",
                table: "BudgetEntry",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_User_OwnerId",
                table: "Budget",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetEntry_BudgetEntryCategory_CategoryId",
                table: "BudgetEntry",
                column: "CategoryId",
                principalTable: "BudgetEntryCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetEntry_Budget_BudgetId",
                table: "BudgetEntry",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetUser_Budget_BudgetsSharedWithThisUserId",
                table: "BudgetUser",
                column: "BudgetsSharedWithThisUserId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetUser_User_UsersWithSharedAccessId",
                table: "BudgetUser",
                column: "UsersWithSharedAccessId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedBudget_Budget_BudgetId",
                table: "SharedBudget",
                column: "BudgetId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SharedBudget_User_UserId",
                table: "SharedBudget",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
