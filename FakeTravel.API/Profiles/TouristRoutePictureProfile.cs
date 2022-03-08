using AutoMapper;
using FakeTravel.API.Models;
using FakeTravel.API.Dtos;
namespace FakeTravel.API.Profiles
{
    public class TouristRoutePictureProfile:Profile
    {
        public TouristRoutePictureProfile()
        {
            CreateMap<TouristRoutePicture,TouristRoutePictureDto>();
            CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();
            CreateMap<TouristRoutePicture, TouristRoutePictureForCreationDto>();
        }
    }
}
