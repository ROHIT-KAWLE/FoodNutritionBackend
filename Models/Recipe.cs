namespace FoodNutritionBackend.Models;

public class Recipe
{
    public int RecipeId { get; set; }

    public string? SourceRecipeKey { get; set; }

    public string RecipeName { get; set; } = string.Empty;

    public string? Source { get; set; }

    public string? Url { get; set; }

    public decimal? Servings { get; set; }

    public decimal? TotalWeightG { get; set; }

    public string? ImageUrl { get; set; }

    public string? CuisineType { get; set; }

    public string? MealType { get; set; }

    public string? DishType { get; set; }

    public string? DietLabels { get; set; }

    public string? HealthLabels { get; set; }

    public string? Cautions { get; set; }

    public string? NutritionBasis { get; set; }

    public bool HasTotalWeight { get; set; }

    public int IngredientCount { get; set; }

    public int IngredientsWithWeight { get; set; }

    public bool AllIngredientWeightsAvailable { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        = new List<RecipeIngredient>();

    public RecipeNutrition? Nutrition { get; set; }

    public ICollection<RecipeCategory> RecipeCategories { get; set; }
        = new List<RecipeCategory>();
}