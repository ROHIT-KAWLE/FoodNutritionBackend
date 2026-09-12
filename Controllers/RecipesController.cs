using FoodNutritionBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FoodNutritionBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;

    private const string SearchCacheKey =
        "recipe-search-data";

    public RecipesController(
        AppDbContext db,
        IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    // ==================================================
    // GET: /api/recipes?page=1&pageSize=20
    // ==================================================

    [HttpGet]
    public async Task<IActionResult> GetRecipes(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var recipes =
            await _db.Recipes
                .AsNoTracking()
                .OrderBy(r => r.RecipeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new
                {
                    r.RecipeId,
                    r.RecipeName,
                    r.Source,
                    r.Url,
                    r.Servings,
                    r.TotalWeightG,
                    r.ImageUrl,
                    r.CuisineType,
                    r.MealType,
                    r.DishType,
                    r.DietLabels,
                    r.HealthLabels
                })
                .ToListAsync(
                    cancellationToken);

        return Ok(new
        {
            page,
            pageSize,
            count = recipes.Count,
            data = recipes
        });
    }

    // ==================================================
    // GET: /api/recipes/{id}
    // ==================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRecipe(
        int id,
        CancellationToken cancellationToken)
    {
        var recipe =
            await _db.Recipes
                .AsNoTracking()
                .Where(r =>
                    r.RecipeId == id)
                .Select(r => new
                {
                    r.RecipeId,
                    r.RecipeName,
                    r.Source,
                    r.Url,
                    r.Servings,
                    r.TotalWeightG,
                    r.ImageUrl,
                    r.CuisineType,
                    r.MealType,
                    r.DishType,
                    r.DietLabels,
                    r.HealthLabels,
                    r.Cautions,
                    r.NutritionBasis
                })
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (recipe is null)
        {
            return NotFound(new
            {
                error = "Recipe not found.",
                recipeId = id
            });
        }

        return Ok(recipe);
    }

    // ==================================================
    // SEARCH
    //
    // GET:
    // /api/recipes/search?query=chicken&page=1&pageSize=20
    // ==================================================

    [HttpGet("search")]
    public async Task<IActionResult> SearchRecipes(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest(new
            {
                error =
                    "Search query is required."
            });
        }

        query = query.Trim();

        if (query.Length < 2)
        {
            return BadRequest(new
            {
                error =
                    "Search query must contain at least 2 characters."
            });
        }

        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        if (pageSize > 20)
        {
            pageSize = 20;
        }

        var searchableRecipes =
            await GetSearchDataAsync(
                cancellationToken);

        var matches =
            searchableRecipes
                .Where(r =>
                    r.RecipeName.Contains(
                        query,
                        StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.RecipeName)
                .ThenBy(r => r.RecipeId)
                .ToList();

        var totalResults =
            matches.Count;

        var results =
            matches
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

        return Ok(new
        {
            query,
            page,
            pageSize,
            count = results.Count,
            totalResults,
            totalPages =
                (int)Math.Ceiling(
                    totalResults /
                    (double)pageSize),
            data = results
        });
    }

    // ==================================================
    // FILTER
    // ==================================================

    [HttpGet("filter")]
    public async Task<IActionResult> FilterRecipes(
        [FromQuery] string? query = null,
        [FromQuery] string? cuisine = null,
        [FromQuery] string? mealType = null,
        [FromQuery] string? dishType = null,
        [FromQuery] string? diet = null,
        [FromQuery] string? healthLabel = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        if (pageSize > 50)
        {
            pageSize = 50;
        }

        var recipes =
            await GetSearchDataAsync(
                cancellationToken);

        IEnumerable<RecipeSearchItem> filtered =
            recipes;

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim();

            filtered =
                filtered.Where(r =>
                    r.RecipeName.Contains(
                        query,
                        StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(cuisine))
        {
            cuisine = cuisine.Trim();

            filtered =
                filtered.Where(r =>
                    string.Equals(
                        r.CuisineType,
                        cuisine,
                        StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(mealType))
        {
            mealType = mealType.Trim();

            filtered =
                filtered.Where(r =>
                    string.Equals(
                        r.MealType,
                        mealType,
                        StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(dishType))
        {
            dishType = dishType.Trim();

            filtered =
                filtered.Where(r =>
                    string.Equals(
                        r.DishType,
                        dishType,
                        StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(diet))
        {
            diet = diet.Trim();

            filtered =
                filtered.Where(r =>
                    ContainsPipeSeparatedValue(
                        r.DietLabels,
                        diet));
        }

        if (!string.IsNullOrWhiteSpace(healthLabel))
        {
            healthLabel =
                healthLabel.Trim();

            filtered =
                filtered.Where(r =>
                    ContainsPipeSeparatedValue(
                        r.HealthLabels,
                        healthLabel));
        }

        var filteredList =
            filtered
                .OrderBy(r => r.RecipeName)
                .ThenBy(r => r.RecipeId)
                .ToList();

        var totalResults =
            filteredList.Count;

        var results =
            filteredList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

        return Ok(new
        {
            filters = new
            {
                query,
                cuisine,
                mealType,
                dishType,
                diet,
                healthLabel
            },

            page,
            pageSize,
            count = results.Count,
            totalResults,

            totalPages =
                (int)Math.Ceiling(
                    totalResults /
                    (double)pageSize),

            data = results
        });
    }

    // ==================================================
    // GET: /api/recipes/{id}/ingredients
    // ==================================================

    [HttpGet("{id:int}/ingredients")]
    public async Task<IActionResult> GetRecipeIngredients(
        int id,
        CancellationToken cancellationToken)
    {
        var recipeExists =
            await _db.Recipes
                .AsNoTracking()
                .AnyAsync(
                    r => r.RecipeId == id,
                    cancellationToken);

        if (!recipeExists)
        {
            return NotFound(new
            {
                error = "Recipe not found.",
                recipeId = id
            });
        }

        var ingredients =
            await _db.RecipeIngredients
                .AsNoTracking()
                .Where(ri =>
                    ri.RecipeId == id)
                .OrderBy(ri => ri.Position)
                .Select(ri => new
                {
                    ri.RecipeIngredientId,
                    ri.IngredientId,

                    IngredientName =
                        ri.Ingredient.IngredientName,

                    ri.IngredientNameOriginal,
                    ri.OriginalText,
                    ri.Quantity,
                    ri.Measure,
                    ri.WeightG,
                    ri.Position,
                    ri.WeightStatus
                })
                .ToListAsync(
                    cancellationToken);

        return Ok(new
        {
            recipeId = id,
            count = ingredients.Count,
            data = ingredients
        });
    }

    // ==================================================
    // GET:
    // /api/recipes/{id}/nutrition
    // /api/recipes/{id}/nutrition?grams=250
    // ==================================================

    [HttpGet("{id:int}/nutrition")]
    public async Task<IActionResult> GetRecipeNutrition(
        int id,
        [FromQuery] decimal? grams = null,
        CancellationToken cancellationToken = default)
    {
        if (grams.HasValue &&
            grams.Value <= 0)
        {
            return BadRequest(new
            {
                error =
                    "grams must be greater than 0."
            });
        }

        var nutrition =
            await _db.RecipeNutrition
                .AsNoTracking()
                .Where(n =>
                    n.RecipeId == id)
                .Select(n => new
                {
                    n.RecipeId,
                    n.CaloriesPer100g,
                    n.ProteinPer100g,
                    n.CarbohydratesPer100g,
                    n.NetCarbohydratesPer100g,
                    n.FatPer100g,
                    n.SaturatedFatPer100g,
                    n.TransFatPer100g,
                    n.MonounsaturatedFatPer100g,
                    n.PolyunsaturatedFatPer100g,
                    n.FiberPer100g,
                    n.SugarPer100g,
                    n.AddedSugarPer100g,
                    n.CholesterolPer100g,
                    n.SodiumPer100g,
                    n.PotassiumPer100g,
                    n.CalciumPer100g,
                    n.IronPer100g,
                    n.MagnesiumPer100g,
                    n.PhosphorusPer100g,
                    n.ZincPer100g,
                    n.VitaminAPer100g,
                    n.VitaminCPer100g,
                    n.VitaminDPer100g,
                    n.VitaminEPer100g,
                    n.VitaminKPer100g,
                    n.VitaminB1Per100g,
                    n.VitaminB2Per100g,
                    n.VitaminB3Per100g,
                    n.VitaminB6Per100g,
                    n.VitaminB12Per100g,
                    n.FolatePer100g,
                    n.WaterPer100g
                })
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (nutrition is null)
        {
            return NotFound(new
            {
                error =
                    "Nutrition data not found.",
                recipeId = id
            });
        }

        if (!grams.HasValue)
        {
            return Ok(new
            {
                recipeId =
                    nutrition.RecipeId,

                basis =
                    "Per100g",

                nutrition.CaloriesPer100g,
                nutrition.ProteinPer100g,
                nutrition.CarbohydratesPer100g,
                nutrition.NetCarbohydratesPer100g,
                nutrition.FatPer100g,
                nutrition.SaturatedFatPer100g,
                nutrition.TransFatPer100g,
                nutrition.MonounsaturatedFatPer100g,
                nutrition.PolyunsaturatedFatPer100g,
                nutrition.FiberPer100g,
                nutrition.SugarPer100g,
                nutrition.AddedSugarPer100g,
                nutrition.CholesterolPer100g,
                nutrition.SodiumPer100g,
                nutrition.PotassiumPer100g,
                nutrition.CalciumPer100g,
                nutrition.IronPer100g,
                nutrition.MagnesiumPer100g,
                nutrition.PhosphorusPer100g,
                nutrition.ZincPer100g,
                nutrition.VitaminAPer100g,
                nutrition.VitaminCPer100g,
                nutrition.VitaminDPer100g,
                nutrition.VitaminEPer100g,
                nutrition.VitaminKPer100g,
                nutrition.VitaminB1Per100g,
                nutrition.VitaminB2Per100g,
                nutrition.VitaminB3Per100g,
                nutrition.VitaminB6Per100g,
                nutrition.VitaminB12Per100g,
                nutrition.FolatePer100g,
                nutrition.WaterPer100g
            });
        }

        var scale =
            grams.Value / 100m;

        return Ok(new
        {
            recipeId =
                nutrition.RecipeId,

            basis =
                "RequestedGrams",

            requestedGrams =
                grams.Value,

            scaleFactor =
                scale,

            calories =
                Scale(
                    nutrition.CaloriesPer100g,
                    scale),

            protein =
                Scale(
                    nutrition.ProteinPer100g,
                    scale),

            carbohydrates =
                Scale(
                    nutrition.CarbohydratesPer100g,
                    scale),

            netCarbohydrates =
                Scale(
                    nutrition.NetCarbohydratesPer100g,
                    scale),

            fat =
                Scale(
                    nutrition.FatPer100g,
                    scale),

            saturatedFat =
                Scale(
                    nutrition.SaturatedFatPer100g,
                    scale),

            transFat =
                Scale(
                    nutrition.TransFatPer100g,
                    scale),

            monounsaturatedFat =
                Scale(
                    nutrition.MonounsaturatedFatPer100g,
                    scale),

            polyunsaturatedFat =
                Scale(
                    nutrition.PolyunsaturatedFatPer100g,
                    scale),

            fiber =
                Scale(
                    nutrition.FiberPer100g,
                    scale),

            sugar =
                Scale(
                    nutrition.SugarPer100g,
                    scale),

            addedSugar =
                Scale(
                    nutrition.AddedSugarPer100g,
                    scale),

            cholesterol =
                Scale(
                    nutrition.CholesterolPer100g,
                    scale),

            sodium =
                Scale(
                    nutrition.SodiumPer100g,
                    scale),

            potassium =
                Scale(
                    nutrition.PotassiumPer100g,
                    scale),

            calcium =
                Scale(
                    nutrition.CalciumPer100g,
                    scale),

            iron =
                Scale(
                    nutrition.IronPer100g,
                    scale),

            magnesium =
                Scale(
                    nutrition.MagnesiumPer100g,
                    scale),

            phosphorus =
                Scale(
                    nutrition.PhosphorusPer100g,
                    scale),

            zinc =
                Scale(
                    nutrition.ZincPer100g,
                    scale),

            vitaminA =
                Scale(
                    nutrition.VitaminAPer100g,
                    scale),

            vitaminC =
                Scale(
                    nutrition.VitaminCPer100g,
                    scale),

            vitaminD =
                Scale(
                    nutrition.VitaminDPer100g,
                    scale),

            vitaminE =
                Scale(
                    nutrition.VitaminEPer100g,
                    scale),

            vitaminK =
                Scale(
                    nutrition.VitaminKPer100g,
                    scale),

            vitaminB1 =
                Scale(
                    nutrition.VitaminB1Per100g,
                    scale),

            vitaminB2 =
                Scale(
                    nutrition.VitaminB2Per100g,
                    scale),

            vitaminB3 =
                Scale(
                    nutrition.VitaminB3Per100g,
                    scale),

            vitaminB6 =
                Scale(
                    nutrition.VitaminB6Per100g,
                    scale),

            vitaminB12 =
                Scale(
                    nutrition.VitaminB12Per100g,
                    scale),

            folate =
                Scale(
                    nutrition.FolatePer100g,
                    scale),

            water =
                Scale(
                    nutrition.WaterPer100g,
                    scale)
        });
    }

    // ==================================================
    // Search cache
    // ==================================================

    private async Task<
        List<RecipeSearchItem>>
        GetSearchDataAsync(
            CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(
                SearchCacheKey,
                out List<RecipeSearchItem>? cached) &&
            cached is not null)
        {
            return cached;
        }

        var recipes =
            await _db.Recipes
                .AsNoTracking()
                .Select(r => new RecipeSearchItem
                {
                    RecipeId =
                        r.RecipeId,

                    RecipeName =
                        r.RecipeName,

                    Servings =
                        r.Servings,

                    TotalWeightG =
                        r.TotalWeightG,

                    ImageUrl =
                        r.ImageUrl,

                    CuisineType =
                        r.CuisineType,

                    MealType =
                        r.MealType,

                    DishType =
                        r.DishType,

                    DietLabels =
                        r.DietLabels,

                    HealthLabels =
                        r.HealthLabels
                })
                .ToListAsync(
                    cancellationToken);

        var cacheOptions =
            new MemoryCacheEntryOptions
            {
                SlidingExpiration =
                    TimeSpan.FromHours(1),

                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromHours(6)
            };

        _cache.Set(
            SearchCacheKey,
            recipes,
            cacheOptions);

        return recipes;
    }

    // ==================================================
    // Helpers
    // ==================================================

    private static bool ContainsPipeSeparatedValue(
        string? source,
        string value)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return false;
        }

        return source
            .Split(
                '|',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries)
            .Any(item =>
                string.Equals(
                    item,
                    value,
                    StringComparison.OrdinalIgnoreCase));
    }

    private static decimal? Scale(
        decimal? value,
        decimal scale)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return value.Value * scale;
    }

    // ==================================================
    // Search model
    // ==================================================

    private sealed class RecipeSearchItem
    {
        public int RecipeId { get; set; }

        public string RecipeName { get; set; }
            = string.Empty;

        public decimal? Servings { get; set; }

        public decimal? TotalWeightG { get; set; }

        public string? ImageUrl { get; set; }

        public string? CuisineType { get; set; }

        public string? MealType { get; set; }

        public string? DishType { get; set; }

        public string? DietLabels { get; set; }

        public string? HealthLabels { get; set; }
    }
}