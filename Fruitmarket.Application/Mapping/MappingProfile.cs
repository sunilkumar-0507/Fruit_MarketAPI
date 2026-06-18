using AutoMapper;
using Fruitmarket.Application.DTOs;
using Fruitmarket.Domain.Entities;

namespace Fruitmarket.Application.Mapping;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<Product, ProductDto>()
            // Explicit admin flag OR depleted stock both surface as out-of-stock to the storefront.
            .ForCtorParam("IsOutOfStock", o => o.MapFrom(p => p.IsOutOfStock || p.StockQuantity <= 0))
            .ForCtorParam("Rating", o => o.MapFrom(p => p.Rating));
        CreateMap<Address, AddressDto>();
        CreateMap<Farmer, FarmerDto>();
        CreateMap<Basket, BasketDto>();
    }
}
