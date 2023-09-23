using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRedundantRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetUser",
                columns: table => new
                {
                    BudgetsSharedWithThisUserId = table.Column<int>(type: "integer", nullable: false),
                    UsersWithSharedAccessId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetUser", x => new { x.BudgetsSharedWithThisUserId, x.UsersWithSharedAccessId });
                    table.ForeignKey(
                        name: "FK_BudgetUser_Budgets_BudgetsSharedWithThisUserId",
                        column: x => x.BudgetsSharedWithThisUserId,
                        principalTable: "Budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetUser_Users_UsersWithSharedAccessId",
                        column: x => x.UsersWithSharedAccessId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetUser_UsersWithSharedAccessId",
                table: "BudgetUser",
                column: "UsersWithSharedAccessId");
        }
    }
}
