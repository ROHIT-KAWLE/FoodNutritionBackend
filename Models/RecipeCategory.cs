namespace FoodNutritionBackend.Models;

public class RecipeCategory
{
    public int RecipeCategoryId { get; set; }

    public int RecipeId { get; set; }

    public int CategoryId { get; set; }

    public Recipe Recipe { get; set; } = null!;

    public Category Category { get; set; } = null!;
}