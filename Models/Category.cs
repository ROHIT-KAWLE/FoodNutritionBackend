namespace FoodNutritionBackend.Models;

public class Category
{
    public int CategoryId { get; set; }

    public string CategoryType { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public string CategoryNameNormalized { get; set; } = string.Empty;

    public ICollection<RecipeCategory> RecipeCategories { get; set; }
        = new List<RecipeCategory>();
}