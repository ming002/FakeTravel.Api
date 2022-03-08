using AutoMapper;
using FakeTravel.API.Dtos;
using FakeTravel.API.Models;

namespace FakeTravel.API.Profiles
{
    public class ShoppingCartProfile:Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();
        }
    }
}
