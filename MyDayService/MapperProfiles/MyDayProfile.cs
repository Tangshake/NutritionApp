using AutoMapper;
using MyDayService.Dtos.Request;
using MyDayService.Dtos.Response;
using MyDayService.Entity;

namespace MyDayService.MapperProfiles
{
    public class MyDayProfile : Profile
    {
        public MyDayProfile()
        {
            // Source --> Target
            CreateMap<DoeUpdateRequestDto, Doe>();
            CreateMap<SingleEntryRequestDto, SingleEntry>();
            CreateMap<SingleMealRequestDto, SingleMeal>();
            CreateMap<SingleProductRequestDto, SingleProduct>();
            CreateMap<Doe, DoeNoId>();
        }
    }
}