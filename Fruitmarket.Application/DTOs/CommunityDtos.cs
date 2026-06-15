namespace Fruitmarket.Application.DTOs;

// ── Farmers (admin partner directory) ───────────────────────────────────────────
public sealed record FarmerDto(Guid Id, string Name, string? Village, string? Produce, int? WeeklySupplyKg, double? Rating, string? Phone, bool IsActive);
public sealed record FarmerUpsertRequest(string Name, string? Village, string? Produce, int? WeeklySupplyKg, double? Rating, string? Phone, bool IsActive);

// ── Baskets (curated storefront hampers) ────────────────────────────────────────
public sealed record BasketDto(Guid Id, string Name, string Description, decimal Price, IReadOnlyList<string> Images, string Items);
public sealed record BasketUpsertRequest(string Name, string Description, decimal Price, IReadOnlyList<string>? Images, string Items, bool IsActive = true);
