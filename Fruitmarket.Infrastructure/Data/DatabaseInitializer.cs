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
            await context.Database.MigrateAsync();

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
            logger.LogWarning(ex, "Database initialization failed — API will start without a database connection");
        }
    }
}
