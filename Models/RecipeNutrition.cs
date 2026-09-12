namespace FoodNutritionBackend.Models;

public class RecipeNutrition
{
    public int RecipeNutritionId { get; set; }

    public int RecipeId { get; set; }

    public string? NutritionBasis { get; set; }

    public decimal? CaloriesPer100g { get; set; }

    public decimal? ProteinPer100g { get; set; }

    public decimal? CarbohydratesPer100g { get; set; }

    public decimal? NetCarbohydratesPer100g { get; set; }

    public decimal? FatPer100g { get; set; }

    public decimal? SaturatedFatPer100g { get; set; }

    public decimal? TransFatPer100g { get; set; }

    public decimal? MonounsaturatedFatPer100g { get; set; }

    public decimal? PolyunsaturatedFatPer100g { get; set; }

    public decimal? FiberPer100g { get; set; }

    public decimal? SugarPer100g { get; set; }

    public decimal? AddedSugarPer100g { get; set; }

    public decimal? CholesterolPer100g { get; set; }

    public decimal? SodiumPer100g { get; set; }

    public decimal? PotassiumPer100g { get; set; }

    public decimal? CalciumPer100g { get; set; }

    public decimal? IronPer100g { get; set; }

    public decimal? MagnesiumPer100g { get; set; }

    public decimal? PhosphorusPer100g { get; set; }

    public decimal? ZincPer100g { get; set; }

    public decimal? VitaminAPer100g { get; set; }

    public decimal? VitaminCPer100g { get; set; }

    public decimal? VitaminDPer100g { get; set; }

    public decimal? VitaminEPer100g { get; set; }

    public decimal? VitaminKPer100g { get; set; }

    public decimal? VitaminB1Per100g { get; set; }

    public decimal? VitaminB2Per100g { get; set; }

    public decimal? VitaminB3Per100g { get; set; }

    public decimal? VitaminB6Per100g { get; set; }

    public decimal? VitaminB12Per100g { get; set; }

    public decimal? FolatePer100g { get; set; }

    public decimal? WaterPer100g { get; set; }

    public Recipe Recipe { get; set; } = null!;
}