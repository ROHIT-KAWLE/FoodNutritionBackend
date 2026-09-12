using FoodNutritionBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodNutritionBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public DbSet<Ingredient> Ingredients =>
        Set<Ingredient>();

    public DbSet<RecipeIngredient> RecipeIngredients =>
        Set<RecipeIngredient>();

    public DbSet<RecipeNutrition> RecipeNutrition =>
        Set<RecipeNutrition>();

    public DbSet<Category> Categories =>
        Set<Category>();

    public DbSet<RecipeCategory> RecipeCategories =>
        Set<RecipeCategory>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ==================================================
        // Recipe
        // ==================================================

        modelBuilder.Entity<Recipe>()
            .HasKey(r => r.RecipeId);

        // RecipeId comes from recipes.csv.
        modelBuilder.Entity<Recipe>()
            .Property(r => r.RecipeId)
            .ValueGeneratedNever();

        // ==================================================
        // Ingredient
        // ==================================================

        modelBuilder.Entity<Ingredient>()
            .HasKey(i => i.IngredientId);

        modelBuilder.Entity<Ingredient>()
            .HasIndex(i => i.NormalizedName)
            .IsUnique();

        // ==================================================
        // Category
        // ==================================================

        modelBuilder.Entity<Category>()
            .HasKey(c => c.CategoryId);

        // CategoryId comes from categories.csv.
        modelBuilder.Entity<Category>()
            .Property(c => c.CategoryId)
            .ValueGeneratedNever();

        // ==================================================
        // RecipeIngredient
        // ==================================================

        modelBuilder.Entity<RecipeIngredient>()
            .HasKey(ri => ri.RecipeIngredientId);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeIngredient>()
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);

        // ==================================================
        // RecipeNutrition
        // ==================================================

        modelBuilder.Entity<RecipeNutrition>()
            .HasKey(rn => rn.RecipeNutritionId);

        modelBuilder.Entity<RecipeNutrition>()
            .HasOne(rn => rn.Recipe)
            .WithOne(r => r.Nutrition)
            .HasForeignKey<RecipeNutrition>(
                rn => rn.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeNutrition>()
            .HasIndex(rn => rn.RecipeId)
            .IsUnique();

        // ==================================================
        // RecipeCategory
        // ==================================================

        modelBuilder.Entity<RecipeCategory>()
            .HasKey(rc => rc.RecipeCategoryId);

        modelBuilder.Entity<RecipeCategory>()
            .HasOne(rc => rc.Recipe)
            .WithMany(r => r.RecipeCategories)
            .HasForeignKey(rc => rc.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeCategory>()
            .HasOne(rc => rc.Category)
            .WithMany(c => c.RecipeCategories)
            .HasForeignKey(rc => rc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecipeCategory>()
            .HasIndex(rc => new
            {
                rc.RecipeId,
                rc.CategoryId
            })
            .IsUnique();

        // ==================================================
        // Decimal precision
        // ==================================================

        foreach (var property in modelBuilder.Model
                     .GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p =>
                         p.ClrType == typeof(decimal) ||
                         p.ClrType == typeof(decimal?)))
        {
            property.SetPrecision(18);
            property.SetScale(6);
        }
    }
}