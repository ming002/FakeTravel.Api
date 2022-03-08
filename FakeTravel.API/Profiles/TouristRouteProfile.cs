using AutoMapper;
using FakeTravel.API.Dtos;
using FakeTravel.API.Models;
using System;

namespace FakeTravel.API.Profiles
{
    public class TouristRouteProfile : Profile
    {
        public TouristRouteProfile()
        {
            CreateMap<TouristRoute, TouristRouteDto>()
                .ForMember(
                dest => dest.Price,
                opt => opt.MapFrom(src => src.OriginPrice * (decimal)(src.DiscountPresent ?? 1))
                )
                .ForMember(
                dest => dest.TravelDays,
                opt => opt.MapFrom(src => src.TravelDays.ToString())
                )
                .ForMember(
                    dest => dest.TripType,
                    opt => opt.MapFrom(src => src.TripType.ToString())
                )
                .ForMember(
                dest=>dest.DepatureCity,
                opt=>opt.MapFrom(src=>src.DepatureCity.ToString()));

            CreateMap<TouristRouteForCreationDto, TouristRoute>()
                .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => Guid.NewGuid()
                ));
            CreateMap<TouristRouteForUpdateDto, TouristRoute>();
            CreateMap<TouristRoute, TouristRouteForUpdateDto>();
        }
    }
}
