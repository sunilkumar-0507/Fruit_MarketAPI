using FluentValidation;
using Fruitmarket.Application.Mapping;
using Fruitmarket.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Fruitmarket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }
}
