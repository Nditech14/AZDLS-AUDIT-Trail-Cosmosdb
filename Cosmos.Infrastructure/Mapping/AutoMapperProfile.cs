using AutoMapper;
using Cosmos.Application.Entities;
using Cosmos.Core.DTO;

namespace Cosmos.Infrastructure.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {


            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<SubCategory, SubCategoryDto>().ReverseMap();

            CreateMap<UpdateCategoryDto, Category>().ReverseMap();

            CreateMap<UpdateProductDto, Product>().ReverseMap();
            CreateMap<UpdateCategoryDto, CategoryDto>().ReverseMap();
            CreateMap<ProductResponseDto, Product>().ReverseMap();

            CreateMap<UpdateSubCategoryDto, SubCategory>().ReverseMap();
            CreateMap<CategoryCreationDto, Category>().ReverseMap();
            CreateMap<CategoryCreationDto, CategoryDto>().ReverseMap();
            CreateMap<ProductCreationDto, Product>().ReverseMap();
            CreateMap<SubCategoryCreationDto, SubCategory>().ReverseMap();
            CreateMap<ProductSpecificationDto, ProductSpecification>().ReverseMap();
            CreateMap<ProductVariantDto, ProductVariant>().ReverseMap();

            CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.MediaPaths, opt => opt.MapFrom(src => src.MediaPaths));

            CreateMap<ProductCreationDto, Product>()
    .ForMember(dest => dest.MediaPaths, opt => opt.MapFrom(src => src.MediaPaths));  // Ensure the types match




        }
    }
}
