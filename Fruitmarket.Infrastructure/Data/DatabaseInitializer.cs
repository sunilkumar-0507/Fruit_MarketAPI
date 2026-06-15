using Fruitmarket.Application.Abstractions;
using Fruitmarket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fruitmarket.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // The DB (esp. a fresh MySQL container) may not accept connections the instant the
            // API boots, so retry the migration a few times before giving up. Applying migrations
            // here is what creates every table (Users, Products, Orders, Baskets, Farmers, …) — if
            // this fails, those tables won't exist and every data endpoint returns 500.
            const int maxAttempts = 12;
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Database migrations applied successfully.");
                    break;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    logger.LogWarning("Database not ready (attempt {Attempt}/{Max}): {Message}. Retrying in 5s…", attempt, maxAttempts, ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }

            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole is null) return;

            var adminExists = await context.Users
                .AnyAsync(u => u.Roles.Any(r => r.Name == "Admin"));

            if (!adminExists)
            {
                var password = config["SeedAdmin:Password"] ?? "Admin@123456";
                var email = config["SeedAdmin:Email"] ?? "admin@fruitmarket.com";

                var admin = new User
                {
                    FullName = "System Admin",
                    Email = email.ToLowerInvariant(),
                    EmailConfirmed = true,
                    PasswordHash = hasher.Hash(password),
                    Roles = [adminRole]
                };
                context.Users.Add(admin);
                context.Cart.Add(new Cart { User = admin });
                await context.SaveChangesAsync();

                logger.LogInformation("Admin user seeded: {Email}", email);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database initialization FAILED after retries. The API will start, but endpoints that need the database will return 500. " +
                "Check the connection string (ConnectionStrings:DefaultConnection) and that the DB user has permission to create tables.");
        }
    }
}
