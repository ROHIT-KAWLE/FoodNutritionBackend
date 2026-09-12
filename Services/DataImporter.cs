using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using FoodNutritionBackend.Data;
using FoodNutritionBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodNutritionBackend.Services;

public class DataImporter
{
    private const int BatchSize = 500;

    private readonly AppDbContext _db;

    public DataImporter(AppDbContext db)
    {
        _db = db;
    }

    public async Task ImportCategoriesAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Categories CSV file was not found.",
                filePath);
        }

        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("IMPORTING CATEGORIES");
        Console.WriteLine("======================================");

        var totalRows = 0;

        using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.ReadWrite,
            bufferSize: 64 * 1024,
            options: FileOptions.SequentialScan);

        using var textReader = new StreamReader(
            stream,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true);

        var configuration = new CsvConfiguration(
            CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            IgnoreBlankLines = true,
            BadDataFound = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.None
        };

        using var csv = new CsvReader(
            textReader,
            configuration);

        await csv.ReadAsync();
        csv.ReadHeader();

        var batch =
            new List<CategoryCsvRow>(BatchSize);

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fields = csv.Parser.Record;

            if (fields is null || fields.Length != 4)
            {
                Console.WriteLine(
                    "Skipping malformed category row.");

                continue;
            }

            var categoryId =
                ParseInt(fields[0]);

            var categoryType =
                NullIfEmpty(fields[1]);

            var categoryName =
                NullIfEmpty(fields[2]);

            var categoryNameNormalized =
                NullIfEmpty(fields[3]);

            if (categoryId <= 0 ||
                string.IsNullOrWhiteSpace(categoryType) ||
                string.IsNullOrWhiteSpace(categoryName))
            {
                Console.WriteLine(
                    "Skipping invalid category row.");

                continue;
            }

            batch.Add(
                new CategoryCsvRow
                {
                    CategoryId =
                        categoryId,

                    CategoryType =
                        categoryType,

                    CategoryName =
                        categoryName,

                    CategoryNameNormalized =
                        categoryNameNormalized
                            ?? NormalizeCategoryName(
                                categoryName)
                });

            totalRows++;

            if (batch.Count >= BatchSize)
            {
                await SaveCategoryBatchAsync(
                    batch,
                    cancellationToken);

                Console.WriteLine(
                    $"Categories imported: {totalRows:N0}");

                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await SaveCategoryBatchAsync(
                batch,
                cancellationToken);

            batch.Clear();
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Categories imported: {totalRows:N0}");

        Console.WriteLine(
            "CATEGORY IMPORT COMPLETE");
    }

    public async Task BuildRecipeCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine();
        Console.WriteLine("======================================");
        Console.WriteLine("BUILDING RECIPE-CATEGORY RELATIONSHIPS");
        Console.WriteLine("======================================");

        var categories =
            await _db.Categories
                .AsNoTracking()
                .ToListAsync(
                    cancellationToken);

        var categoryLookup =
            categories.ToDictionary(
                c => BuildCategoryKey(
                    c.CategoryType,
                    c.CategoryNameNormalized),
                c => c.CategoryId,
                StringComparer.OrdinalIgnoreCase);

        Console.WriteLine(
            $"Categories loaded: {categoryLookup.Count:N0}");

        var recipes =
            await _db.Recipes
                .AsNoTracking()
                .Select(r => new
                {
                    r.RecipeId,
                    r.CuisineType,
                    r.MealType,
                    r.DishType
                })
                .ToListAsync(
                    cancellationToken);

        var relationships =
            new List<RecipeCategory>(
                BatchSize);

        var totalRelationships = 0;

        foreach (var recipe in recipes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            AddRelationship(
                relationships,
                categoryLookup,
                recipe.RecipeId,
                recipe.CuisineType,
                "cuisine_type");

            AddRelationship(
                relationships,
                categoryLookup,
                recipe.RecipeId,
                recipe.MealType,
                "meal_type");

            AddRelationship(
                relationships,
                categoryLookup,
                recipe.RecipeId,
                recipe.DishType,
                "dish_type");

            if (relationships.Count >= BatchSize)
            {
                await SaveRecipeCategoryBatchAsync(
                    relationships,
                    cancellationToken);

                totalRelationships +=
                    relationships.Count;

                Console.WriteLine(
                    $"Recipe-category relationships inserted: {totalRelationships:N0}");

                relationships.Clear();
                _db.ChangeTracker.Clear();
            }
        }

        if (relationships.Count > 0)
        {
            var remaining =
                relationships.Count;

            await SaveRecipeCategoryBatchAsync(
                relationships,
                cancellationToken);

            totalRelationships += remaining;

            relationships.Clear();
            _db.ChangeTracker.Clear();
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Recipe-category relationships inserted: {totalRelationships:N0}");

        Console.WriteLine(
            "RECIPE-CATEGORY IMPORT COMPLETE");
    }

    private async Task SaveCategoryBatchAsync(
        List<CategoryCsvRow> rows,
        CancellationToken cancellationToken)
    {
        var entities =
            rows.Select(row =>
                new Category
                {
                    CategoryId =
                        row.CategoryId,

                    CategoryType =
                        row.CategoryType,

                    CategoryName =
                        row.CategoryName,

                    CategoryNameNormalized =
                        row.CategoryNameNormalized
                })
                .ToList();

        await _db.Categories.AddRangeAsync(
            entities,
            cancellationToken);

        await _db.SaveChangesAsync(
            cancellationToken);

        _db.ChangeTracker.Clear();
    }

    private async Task SaveRecipeCategoryBatchAsync(
        List<RecipeCategory> relationships,
        CancellationToken cancellationToken)
    {
        if (relationships.Count == 0)
        {
            return;
        }

        await _db.RecipeCategories.AddRangeAsync(
            relationships,
            cancellationToken);

        await _db.SaveChangesAsync(
            cancellationToken);
    }

    private static void AddRelationship(
        List<RecipeCategory> relationships,
        Dictionary<string, int> categoryLookup,
        int recipeId,
        string? value,
        string categoryType)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var values =
            value.Split(
                '|',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries);

        foreach (var valuePart in values)
        {
            var normalized =
                NormalizeCategoryName(valuePart);

            if (string.IsNullOrWhiteSpace(normalized))
            {
                continue;
            }

            var key =
                BuildCategoryKey(
                    categoryType,
                    normalized);

            if (!categoryLookup.TryGetValue(
                    key,
                    out var categoryId))
            {
                Console.WriteLine(
                    $"WARNING: Category not found: {categoryType} / {normalized}");

                continue;
            }

            relationships.Add(
                new RecipeCategory
                {
                    RecipeId =
                        recipeId,

                    CategoryId =
                        categoryId
                });
        }
    }

    private static string BuildCategoryKey(
        string categoryType,
        string categoryName)
    {
        return
            $"{categoryType.Trim().ToLowerInvariant()}|" +
            $"{categoryName.Trim().ToLowerInvariant()}";
    }

    private static string NormalizeCategoryName(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return string.Join(
            " ",
            value
                .Trim()
                .ToLowerInvariant()
                .Split(
                    Array.Empty<char>(),
                    StringSplitOptions.RemoveEmptyEntries));
    }

    private static string? NullIfEmpty(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim();
    }

    private static int ParseInt(
        string? value)
    {
        if (int.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var result))
        {
            return result;
        }

        return 0;
    }

    private sealed class CategoryCsvRow
    {
        public int CategoryId { get; set; }

        public string CategoryType { get; set; }
            = string.Empty;

        public string CategoryName { get; set; }
            = string.Empty;

        public string CategoryNameNormalized { get; set; }
            = string.Empty;
    }
}