using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodNutritionBackend.Migrations
{
    /// <inheritdoc />
    public partial class MakeCategoryIdNonIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            // RecipeCategories currently has a foreign key
            // pointing to Categories.CategoryId.
            // Drop that FK before rebuilding Categories.

            migrationBuilder.DropForeignKey(
                name: "FK_RecipeCategories_Categories_CategoryId",
                table: "RecipeCategories");

            // Categories is currently empty, so it is safe to
            // rebuild the table with CategoryId as a normal INT
            // instead of an IDENTITY column.

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId =
                        table.Column<int>(
                            type: "int",
                            nullable: false),

                    CategoryType =
                        table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: false),

                    CategoryName =
                        table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: false),

                    CategoryNameNormalized =
                        table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Categories",
                        x => x.CategoryId);
                });

            // Recreate the foreign key.
            migrationBuilder.AddForeignKey(
                name: "FK_RecipeCategories_Categories_CategoryId",
                table: "RecipeCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeCategories_Categories_CategoryId",
                table: "RecipeCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId =
                        table.Column<int>(
                            type: "int",
                            nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    CategoryType =
                        table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: false),

                    CategoryName =
                        table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: false),

                    CategoryNameNormalized =
                        table.Column<string>(
                            type: "nvarchar(max)",
                            nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Categories",
                        x => x.CategoryId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeCategories_Categories_CategoryId",
                table: "RecipeCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}