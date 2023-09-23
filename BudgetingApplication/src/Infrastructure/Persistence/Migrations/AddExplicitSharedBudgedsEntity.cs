using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitSharedBudgedsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetUser_Budget_SharedBudgetsId",
                table: "BudgetUser");

            migrationBuilder.RenameColumn(
                name: "SharedBudgetsId",
                table: "BudgetUser",
                newName: "BudgetsSharedWithThisUserId");

            migrationBuilder.CreateTable(
                name: "SharedBudget",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    BudgetId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedBudget", x => new { x.BudgetId, x.UserId });
                    table.ForeignKey(
                        name: "FK_SharedBudget_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SharedBudget_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharedBudget_UserId",
                table: "SharedBudget",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetUser_Budget_BudgetsSharedWithThisUserId",
                table: "BudgetUser",
                column: "BudgetsSharedWithThisUserId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetUser_Budget_BudgetsSharedWithThisUserId",
                table: "BudgetUser");

            migrationBuilder.DropTable(
                name: "SharedBudget");

            migrationBuilder.RenameColumn(
                name: "BudgetsSharedWithThisUserId",
                table: "BudgetUser",
                newName: "SharedBudgetsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetUser_Budget_SharedBudgetsId",
                table: "BudgetUser",
                column: "SharedBudgetsId",
                principalTable: "Budget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
