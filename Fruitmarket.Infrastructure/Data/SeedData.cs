using Fruitmarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fruitmarket.Infrastructure.Data;

public static class SeedData
{
    public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid CustomerRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid FruitsCategoryId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid VegetablesCategoryId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid MangoId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static readonly Guid BananaId = Guid.Parse("66666666-6666-6666-6666-666666666666");

    public static void Seed(ModelBuilder modelBuilder)
    {
        var created = new DateTime(2026, 05, 28, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = AdminRoleId, Name = "Admin", CreatedAtUtc = created },
            new Role { Id = CustomerRoleId, Name = "Customer", CreatedAtUtc = created });
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = FruitsCategoryId, NameEn = "Fruits", NameTa = "பழங்கள்", Slug = "fruits", DescriptionEn = "Fresh seasonal fruits", DescriptionTa = "புதிய பருவ பழங்கள்", CreatedAtUtc = created },
            new Category { Id = VegetablesCategoryId, NameEn = "Vegetables", NameTa = "காய்கறிகள்", Slug = "vegetables", DescriptionEn = "Farm fresh vegetables", DescriptionTa = "பண்ணை புதிய காய்கறிகள்", CreatedAtUtc = created });
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = MangoId, NameEn = "Alphonso Mango", NameTa = "அல்போன்சோ மாம்பழம்", Slug = "alphonso-mango", DescriptionEn = "Sweet premium mangoes", DescriptionTa = "இனிப்பு தரமான மாம்பழங்கள்", Price = 180, StockQuantity = 100, CategoryId = FruitsCategoryId, CreatedAtUtc = created },
            new Product { Id = BananaId, NameEn = "Nendran Banana", NameTa = "நேந்திரன் வாழைப்பழம்", Slug = "nendran-banana", DescriptionEn = "Fresh bananas by dozen", DescriptionTa = "புதிய வாழைப்பழங்கள்", Price = 70, StockQuantity = 250, CategoryId = FruitsCategoryId, CreatedAtUtc = created });
        modelBuilder.Entity<ProductImage>().HasData(
            new ProductImage { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), ProductId = MangoId, Url = "https://images.unsplash.com/photo-1553279768-865429fa0078", AltText = "Mangoes", IsPrimary = true, CreatedAtUtc = created },
            new ProductImage { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), ProductId = BananaId, Url = "https://images.unsplash.com/photo-1603833665858-e61d17a86224", AltText = "Bananas", IsPrimary = true, CreatedAtUtc = created });
        modelBuilder.Entity<Coupon>().HasData(
            new Coupon { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Code = "WELCOME10", DiscountAmount = 10, MinimumOrderAmount = 100, StartsAtUtc = created, EndsAtUtc = created.AddYears(5), CreatedAtUtc = created });
        modelBuilder.Entity<Farmer>().HasData(
            new Farmer { Id = Guid.Parse("fa000001-0000-0000-0000-000000000001"), Name = "Murugesan P.", Village = "Tenkasi", Produce = "Mango, Banana", WeeklySupplyKg = 800, Rating = 4.9, Phone = "+91 94433 00001", IsActive = true, CreatedAtUtc = created },
            new Farmer { Id = Guid.Parse("fa000002-0000-0000-0000-000000000002"), Name = "Rajan T.", Village = "Courtallam", Produce = "Guava, Papaya", WeeklySupplyKg = 450, Rating = 4.7, Phone = "+91 94433 00002", IsActive = true, CreatedAtUtc = created },
            new Farmer { Id = Guid.Parse("fa000003-0000-0000-0000-000000000003"), Name = "Selvam K.", Village = "Alangulam", Produce = "Banana, Jackfruit", WeeklySupplyKg = 600, Rating = 4.8, Phone = "+91 94433 00003", IsActive = true, CreatedAtUtc = created },
            new Farmer { Id = Guid.Parse("fa000004-0000-0000-0000-000000000004"), Name = "Lakshmi A.", Village = "Kadayanallur", Produce = "Pomegranate, Grapes", WeeklySupplyKg = 300, Rating = 4.6, Phone = "+91 94433 00004", IsActive = false, CreatedAtUtc = created },
            new Farmer { Id = Guid.Parse("fa000005-0000-0000-0000-000000000005"), Name = "Kumar M.", Village = "Sankarankovil", Produce = "Watermelon, Pineapple", WeeklySupplyKg = 500, Rating = 4.5, Phone = "+91 94433 00005", IsActive = true, CreatedAtUtc = created },
            new Farmer { Id = Guid.Parse("fa000006-0000-0000-0000-000000000006"), Name = "Pandian S.", Village = "Tirunelveli", Produce = "Dry Fruits, Seasonal", WeeklySupplyKg = 250, Rating = 4.7, Phone = "+91 94433 00006", IsActive = true, CreatedAtUtc = created });
        modelBuilder.Entity<Basket>().HasData(
            new Basket { Id = Guid.Parse("ba000001-0000-0000-0000-000000000001"), Name = "Pongal Festival Basket", Description = "Handcrafted festival hamper with premium mangoes, bananas, and pomegranates.", Price = 1450, Images = ["/images/categories/fruit-baskets.jpg", "/images/products/mangoes.jpeg", "/images/categories/p-pomegranate.jpg"], Items = "Mango × 4, Banana × 6, Pomegranate × 2", IsActive = true, CreatedAtUtc = created },
            new Basket { Id = Guid.Parse("ba000002-0000-0000-0000-000000000002"), Name = "Exotic Mix Combo", Description = "Imported tropical selection — rambutan, dragon fruit, and mangosteen.", Price = 850, Images = ["/images/products/wa2-rambutan.jpeg", "/images/products/wa2-dragon-fruit.jpeg", "/images/products/wa2-mangosteen.jpeg", "/images/categories/p-pomegranate.jpg"], Items = "Rambutan × 6, Dragon Fruit × 2, Mangosteen × 3", IsActive = true, CreatedAtUtc = created });
    }
}
