using FoodNutritionBackend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodNutritionBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NutritionController : ControllerBase
{
    private readonly AppDbContext _db;

    public NutritionController(AppDbContext db)
    {
        _db = db;
    }

    // ==================================================
    // POST: /api/nutrition/calculate
    //
    // Example request:
    //
    // {
    //   "items": [
    //     {
    //       "recipeId": 1,
    //       "grams": 250
    //     },
    //     {
    //       "recipeId": 5,
    //       "grams": 100
    //     }
    //   ]
    // }
    // ==================================================

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateNutrition(
        [FromBody] NutritionCalculationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Items is null ||
            request.Items.Count == 0)
        {
            return BadRequest(new
            {
                message =
                    "At least one food item is required."
            });
        }

        if (request.Items.Count > 50)
        {
            return BadRequest(new
            {
                message =
                    "A maximum of 50 items can be calculated at once."
            });
        }

        foreach (var item in request.Items)
        {
            if (item.RecipeId <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Every recipeId must be greater than 0."
                });
            }

            if (item.Grams <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Every grams value must be greater than 0."
                });
            }
        }

        var recipeIds =
            request.Items
                .Select(item => item.RecipeId)
                .Distinct()
                .ToList();

        var nutritionRows =
            await _db.RecipeNutrition
                .AsNoTracking()
                .Where(n =>
                    recipeIds.Contains(n.RecipeId))
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
                .ToListAsync(cancellationToken);

        var nutritionLookup =
            nutritionRows.ToDictionary(
                row => row.RecipeId);

        var missingRecipeIds =
            recipeIds
                .Where(id =>
                    !nutritionLookup.ContainsKey(id))
                .ToList();

        if (missingRecipeIds.Count > 0)
        {
            return NotFound(new
            {
                message =
                    "Nutrition data was not found for one or more recipes.",
                missingRecipeIds
            });
        }

        decimal calories = 0m;
        decimal protein = 0m;
        decimal carbohydrates = 0m;
        decimal netCarbohydrates = 0m;
        decimal fat = 0m;
        decimal saturatedFat = 0m;
        decimal transFat = 0m;
        decimal monounsaturatedFat = 0m;
        decimal polyunsaturatedFat = 0m;
        decimal fiber = 0m;
        decimal sugar = 0m;
        decimal addedSugar = 0m;
        decimal cholesterol = 0m;
        decimal sodium = 0m;
        decimal potassium = 0m;
        decimal calcium = 0m;
        decimal iron = 0m;
        decimal magnesium = 0m;
        decimal phosphorus = 0m;
        decimal zinc = 0m;
        decimal vitaminA = 0m;
        decimal vitaminC = 0m;
        decimal vitaminD = 0m;
        decimal vitaminE = 0m;
        decimal vitaminK = 0m;
        decimal vitaminB1 = 0m;
        decimal vitaminB2 = 0m;
        decimal vitaminB3 = 0m;
        decimal vitaminB6 = 0m;
        decimal vitaminB12 = 0m;
        decimal folate = 0m;
        decimal water = 0m;

        var itemResults =
            new List<object>();

        foreach (var item in request.Items)
        {
            var nutrition =
                nutritionLookup[item.RecipeId];

            var scale =
                item.Grams / 100m;

            var itemCalories =
                Scale(
                    nutrition.CaloriesPer100g,
                    scale);

            var itemProtein =
                Scale(
                    nutrition.ProteinPer100g,
                    scale);

            var itemCarbohydrates =
                Scale(
                    nutrition.CarbohydratesPer100g,
                    scale);

            var itemNetCarbohydrates =
                Scale(
                    nutrition.NetCarbohydratesPer100g,
                    scale);

            var itemFat =
                Scale(
                    nutrition.FatPer100g,
                    scale);

            var itemSaturatedFat =
                Scale(
                    nutrition.SaturatedFatPer100g,
                    scale);

            var itemTransFat =
                Scale(
                    nutrition.TransFatPer100g,
                    scale);

            var itemMonounsaturatedFat =
                Scale(
                    nutrition.MonounsaturatedFatPer100g,
                    scale);

            var itemPolyunsaturatedFat =
                Scale(
                    nutrition.PolyunsaturatedFatPer100g,
                    scale);

            var itemFiber =
                Scale(
                    nutrition.FiberPer100g,
                    scale);

            var itemSugar =
                Scale(
                    nutrition.SugarPer100g,
                    scale);

            var itemAddedSugar =
                Scale(
                    nutrition.AddedSugarPer100g,
                    scale);

            var itemCholesterol =
                Scale(
                    nutrition.CholesterolPer100g,
                    scale);

            var itemSodium =
                Scale(
                    nutrition.SodiumPer100g,
                    scale);

            var itemPotassium =
                Scale(
                    nutrition.PotassiumPer100g,
                    scale);

            var itemCalcium =
                Scale(
                    nutrition.CalciumPer100g,
                    scale);

            var itemIron =
                Scale(
                    nutrition.IronPer100g,
                    scale);

            var itemMagnesium =
                Scale(
                    nutrition.MagnesiumPer100g,
                    scale);

            var itemPhosphorus =
                Scale(
                    nutrition.PhosphorusPer100g,
                    scale);

            var itemZinc =
                Scale(
                    nutrition.ZincPer100g,
                    scale);

            var itemVitaminA =
                Scale(
                    nutrition.VitaminAPer100g,
                    scale);

            var itemVitaminC =
                Scale(
                    nutrition.VitaminCPer100g,
                    scale);

            var itemVitaminD =
                Scale(
                    nutrition.VitaminDPer100g,
                    scale);

            var itemVitaminE =
                Scale(
                    nutrition.VitaminEPer100g,
                    scale);

            var itemVitaminK =
                Scale(
                    nutrition.VitaminKPer100g,
                    scale);

            var itemVitaminB1 =
                Scale(
                    nutrition.VitaminB1Per100g,
                    scale);

            var itemVitaminB2 =
                Scale(
                    nutrition.VitaminB2Per100g,
                    scale);

            var itemVitaminB3 =
                Scale(
                    nutrition.VitaminB3Per100g,
                    scale);

            var itemVitaminB6 =
                Scale(
                    nutrition.VitaminB6Per100g,
                    scale);

            var itemVitaminB12 =
                Scale(
                    nutrition.VitaminB12Per100g,
                    scale);

            var itemFolate =
                Scale(
                    nutrition.FolatePer100g,
                    scale);

            var itemWater =
                Scale(
                    nutrition.WaterPer100g,
                    scale);

            calories += itemCalories ?? 0m;
            protein += itemProtein ?? 0m;
            carbohydrates +=
                itemCarbohydrates ?? 0m;
            netCarbohydrates +=
                itemNetCarbohydrates ?? 0m;
            fat += itemFat ?? 0m;
            saturatedFat +=
                itemSaturatedFat ?? 0m;
            transFat +=
                itemTransFat ?? 0m;
            monounsaturatedFat +=
                itemMonounsaturatedFat ?? 0m;
            polyunsaturatedFat +=
                itemPolyunsaturatedFat ?? 0m;
            fiber += itemFiber ?? 0m;
            sugar += itemSugar ?? 0m;
            addedSugar += itemAddedSugar ?? 0m;
            cholesterol += itemCholesterol ?? 0m;
            sodium += itemSodium ?? 0m;
            potassium += itemPotassium ?? 0m;
            calcium += itemCalcium ?? 0m;
            iron += itemIron ?? 0m;
            magnesium += itemMagnesium ?? 0m;
            phosphorus += itemPhosphorus ?? 0m;
            zinc += itemZinc ?? 0m;
            vitaminA += itemVitaminA ?? 0m;
            vitaminC += itemVitaminC ?? 0m;
            vitaminD += itemVitaminD ?? 0m;
            vitaminE += itemVitaminE ?? 0m;
            vitaminK += itemVitaminK ?? 0m;
            vitaminB1 += itemVitaminB1 ?? 0m;
            vitaminB2 += itemVitaminB2 ?? 0m;
            vitaminB3 += itemVitaminB3 ?? 0m;
            vitaminB6 += itemVitaminB6 ?? 0m;
            vitaminB12 += itemVitaminB12 ?? 0m;
            folate += itemFolate ?? 0m;
            water += itemWater ?? 0m;

            itemResults.Add(
                new
                {
                    recipeId =
                        item.RecipeId,

                    grams =
                        item.Grams,

                    scaleFactor =
                        scale,

                    calories =
                        itemCalories,

                    protein =
                        itemProtein,

                    carbohydrates =
                        itemCarbohydrates,

                    netCarbohydrates =
                        itemNetCarbohydrates,

                    fat =
                        itemFat,

                    fiber =
                        itemFiber
                });
        }

        return Ok(
            new
            {
                basis = "RequestedGrams",

                totalItems =
                    request.Items.Count,

                totalGrams =
                    request.Items.Sum(
                        item => item.Grams),

                totals = new
                {
                    calories,
                    protein,
                    carbohydrates,
                    netCarbohydrates,
                    fat,
                    saturatedFat,
                    transFat,
                    monounsaturatedFat,
                    polyunsaturatedFat,
                    fiber,
                    sugar,
                    addedSugar,
                    cholesterol,
                    sodium,
                    potassium,
                    calcium,
                    iron,
                    magnesium,
                    phosphorus,
                    zinc,
                    vitaminA,
                    vitaminC,
                    vitaminD,
                    vitaminE,
                    vitaminK,
                    vitaminB1,
                    vitaminB2,
                    vitaminB3,
                    vitaminB6,
                    vitaminB12,
                    folate,
                    water
                },

                items = itemResults
            });
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
}

// ==================================================
// Request models
// ==================================================

public class NutritionCalculationRequest
{
    public List<NutritionCalculationItem> Items { get; set; }
        = new();
}

public class NutritionCalculationItem
{
    public int RecipeId { get; set; }

    public decimal Grams { get; set; }
}