using AutoMapper;
using NutritionWebClient.Dtos.Doe.Request;
using NutritionWebClient.Dtos.Doe.Response;
using NutritionWebClient.Model.Doe;
using NutritionWebClient.Model.Meal;
using NutritionWebClient.Model.Product;

namespace NutritionWebClient.MapperProfiles
{
    public class WebClientProfile : Profile
    {
        public WebClientProfile()
        {
            // Source --> Target
            CreateMap<DoeResponseDto, Doe>();
            CreateMap<DoeResponseDto, DoeResponseDto>();
            CreateMap<SingleEntryDto, SingleEntryDto>();
            CreateMap<SingleProductDto, SingleProductDto>();
            CreateMap<SingleMealDto, SingleMealDto>();
            CreateMap<ProductResponseDto, ProductResponseDto>();
            CreateMap<MealResponseDto, MealResponseDto>();
            CreateMap<IngredientsResponseDto, IngredientsResponseDto>();
            
            CreateMap<ProductModel, SingleProductDto>()
                .ForMember(d => d.Product, opt => opt.MapFrom(x => x));
            CreateMap<ProductModel, ProductResponseDto>();
            
            CreateMap<MealModel, MealResponseDto>();
            CreateMap<ProductModel, IngredientsResponseDto>();
            CreateMap<MealModel, SingleMealDto>()
                .ForMember(c => c.Weight, opt => opt.MapFrom(src => 100))
                .ForMember(c => c.Meal, opt => opt.MapFrom(src => src));
                
            
        }
    }
}