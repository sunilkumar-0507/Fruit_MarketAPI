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
            .ForCtorParam("IsOutOfStock", o => o.MapFrom(p => p.StockQuantity <= 0))
            .ForCtorParam("Rating", o => o.MapFrom(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0));
        CreateMap<Address, AddressDto>();
    }
}
