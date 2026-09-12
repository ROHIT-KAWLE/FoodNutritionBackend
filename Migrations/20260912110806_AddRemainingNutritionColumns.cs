using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodNutritionBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingNutritionColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FolatePer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MagnesiumPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PhosphorusPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminAPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminB12Per100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminB1Per100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminB2Per100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminB3Per100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminB6Per100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminCPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminDPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminEPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VitaminKPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WaterPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ZincPer100g",
                table: "RecipeNutrition",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FolatePer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "MagnesiumPer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "PhosphorusPer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminAPer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminB12Per100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminB1Per100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminB2Per100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminB3Per100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminB6Per100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminCPer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminDPer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminEPer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "VitaminKPer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "WaterPer100g",
                table: "RecipeNutrition");

            migrationBuilder.DropColumn(
                name: "ZincPer100g",
                table: "RecipeNutrition");
        }
    }
}
