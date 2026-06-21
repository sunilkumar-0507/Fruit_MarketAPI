using FluentValidation;
using Fruitmarket.Application.DTOs;

namespace Fruitmarket.Application.Validators;

internal static class ValidationPatterns
{
    // 10-digit Indian mobile number (starts 6–9).
    public const string IndianMobile = @"^[6-9]\d{9}$";
    public const string IndianMobileMessage = "Enter a valid 10-digit mobile number.";
}

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(ValidationPatterns.IndianMobile).WithMessage(ValidationPatterns.IndianMobileMessage);
        // Email is optional now; validate the format only when one is supplied.
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Password).MinimumLength(8);
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(ValidationPatterns.IndianMobile).WithMessage(ValidationPatterns.IndianMobileMessage);
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class ProductUpsertRequestValidator : AbstractValidator<ProductUpsertRequest>
{
    public ProductUpsertRequestValidator()
    {
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameTa).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0m);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

public sealed class ProductDiscountRequestValidator : AbstractValidator<ProductDiscountRequest>
{
    public ProductDiscountRequestValidator()
    {
        RuleFor(x => x.Percentage).InclusiveBetween(0, 99);
    }
}

public sealed class CategoryUpsertRequestValidator : AbstractValidator<CategoryUpsertRequest>
{
    public CategoryUpsertRequestValidator()
    {
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(160);
        RuleFor(x => x.NameTa).NotEmpty().MaximumLength(160);
    }
}

public sealed class CartItemRequestValidator : AbstractValidator<AddCartItemRequest>
{
    public CartItemRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        // Decimal quantities for gram-based ordering (e.g. 0.25 = 250g); must be > 0.
        RuleFor(x => x.Quantity).GreaterThan(0m).LessThanOrEqualTo(99m);
    }
}

public sealed class ReviewRequestValidator : AbstractValidator<ReviewRequest>
{
    public ReviewRequestValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(1000);
    }
}

public sealed class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(x => x.Line1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Line2).MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateCartItemRequestValidator : AbstractValidator<UpdateCartItemRequest>
{
    public UpdateCartItemRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0m).LessThanOrEqualTo(99m);
    }
}

public sealed class CouponUpsertRequestValidator : AbstractValidator<CouponUpsertRequest>
{
    public CouponUpsertRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50).Matches(@"^[A-Z0-9_\-]+$").WithMessage("Code must be alphanumeric uppercase.");
        RuleFor(x => x.DiscountAmount).GreaterThan(0);
        RuleFor(x => x.MinimumOrderAmount).GreaterThan(0).When(x => x.MinimumOrderAmount.HasValue);
        RuleFor(x => x.EndsAtUtc).GreaterThan(x => x.StartsAtUtc).WithMessage("End date must be after start date.");
    }
}

public sealed class FarmerUpsertRequestValidator : AbstractValidator<FarmerUpsertRequest>
{
    public FarmerUpsertRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.Village).MaximumLength(160);
        RuleFor(x => x.Produce).MaximumLength(300);
        RuleFor(x => x.Phone).MaximumLength(40);
        RuleFor(x => x.WeeklySupplyKg).GreaterThanOrEqualTo(0).When(x => x.WeeklySupplyKg.HasValue);
        RuleFor(x => x.Rating).InclusiveBetween(0, 5).When(x => x.Rating.HasValue);
    }
}

public sealed class BasketUpsertRequestValidator : AbstractValidator<BasketUpsertRequest>
{
    public BasketUpsertRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Items).NotEmpty().MaximumLength(1000);
    }
}

public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword).MinimumLength(8);
    }
}
