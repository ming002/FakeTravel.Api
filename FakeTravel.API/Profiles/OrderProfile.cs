using AutoMapper;
using FakeTravel.API.Dtos;
using FakeTravel.API.Models;

namespace FakeTravel.API.Profiles
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(
                dest => dest.State,
                opt => {
                    opt.MapFrom(src => src.State.ToString());
                });
        }
    }
}
