using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCategoriesAndSharedBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "BudgetEntry",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BudgetEntryCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetEntryCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BudgetUser",
                columns: table => new
                {
                    SharedBudgetsId = table.Column<int>(type: "integer", nullable: false),
                    UsersWithSharedAccessId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetUser", x => new { x.SharedBudgetsId, x.UsersWithSharedAccessId });
                    table.ForeignKey(
                        name: "FK_BudgetUser_Budget_SharedBudgetsId",
                        column: x => x.SharedBudgetsId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetUser_User_UsersWithSharedAccessId",
                        column: x => x.UsersWithSharedAccessId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetEntry_CategoryId",
                table: "BudgetEntry",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetUser_UsersWithSharedAccessId",
                table: "BudgetUser",
                column: "UsersWithSharedAccessId");

            migrationBuilder.AddForeignKey(
                name: "FK_BudgetEntry_BudgetEntryCategory_CategoryId",
                table: "BudgetEntry",
                column: "CategoryId",
                principalTable: "BudgetEntryCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BudgetEntry_BudgetEntryCategory_CategoryId",
                table: "BudgetEntry");

            migrationBuilder.DropTable(
                name: "BudgetEntryCategory");

            migrationBuilder.DropTable(
                name: "BudgetUser");

            migrationBuilder.DropIndex(
                name: "IX_BudgetEntry_CategoryId",
                table: "BudgetEntry");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "BudgetEntry");
        }
    }
}
