namespace FoodNutritionBackend.Models;

public class RecipeIngredient
{
    public int RecipeIngredientId { get; set; }

    public int RecipeId { get; set; }

    public int IngredientId { get; set; }

    public string? IngredientNameOriginal { get; set; }

    public string? OriginalText { get; set; }

    public decimal? Quantity { get; set; }

    public string? Measure { get; set; }

    public decimal? WeightG { get; set; }

    public int Position { get; set; }

    public string? WeightStatus { get; set; }

    public Recipe Recipe { get; set; } = null!;

    public Ingredient Ingredient { get; set; } = null!;
}