using Microsoft.AspNetCore.Mvc;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;
using FakeTravel.API.Models;
using FakeTravel.API.Services;
using FakeTravel.API.Dtos;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FakeTravel.API.Controllers
{
    [Route("api/touristRoutes/{touristRouteId}/pictures")]
    [ApiController]
    public class TouristRoutePictureController : ControllerBase
    {
        private ITouristRouteReposity _touristRouteReposity;
        private IMapper _mapper;
        public TouristRoutePictureController(
            ITouristRouteReposity touristRouteReposity,
            IMapper mapper)
        {
            _touristRouteReposity = touristRouteReposity;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetPictureListForTouristRoute(Guid touristRouteId)
        {
            if (!(await _touristRouteReposity.TouristRouteExitsAsync(touristRouteId)))
            {
                return NotFound("旅游路线不存在");
            }
            var picturesFromRepo =await _touristRouteReposity.GetPicturesByTouristRouteIdAsync(touristRouteId);
            if (picturesFromRepo == null || picturesFromRepo.Count() <= 0)
            {
                return NotFound("照片不存在");
            }
            var pictureDto = _mapper.Map<IEnumerable<TouristRoutePictureDto>>(picturesFromRepo);
            return Ok(pictureDto);
        }
        [HttpGet("{pictureId}", Name = "GetPicture")]
        public async Task<IActionResult> GetPicture(Guid touristRouteId, int pictureId)
        {
            if (!(await _touristRouteReposity.TouristRouteExitsAsync(touristRouteId)))
            {
                return NotFound("旅游路线不存在");
            }
            var pictureFromRepo = await _touristRouteReposity.GetPictureAsync(pictureId);
            if (pictureFromRepo == null)
            {
                return NotFound("照片不存在");
            }
            var pictureDto = _mapper.Map<TouristRoutePictureDto>(pictureFromRepo);
            return Ok(pictureDto);
        }
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTouristRoutePicture(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRoutePictureForCreationDto touristRoutePictureForCreationDto)
        {
            if (!(await _touristRouteReposity.TouristRouteExitsAsync(touristRouteId)))
            {
                return NotFound("旅游路线不存在");
            }
            var pictureModel = _mapper.Map<TouristRoutePicture>(touristRoutePictureForCreationDto);
            _touristRouteReposity.AddTouristRoutePicture(touristRouteId, pictureModel);
            await _touristRouteReposity.SaveAsync();
            var touristRoutePicrureToReturn = _mapper.Map<TouristRoutePictureDto>(pictureModel);
            return CreatedAtRoute("GetPicture",
                new { touristRouteId = touristRouteId, pictureID = pictureModel.Id },
                touristRoutePicrureToReturn);
        }

        [HttpDelete("{pictureId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePicture(
            [FromRoute] Guid touristRouteId,
            [FromRoute] int pictureId)
        {
            if (!(await _touristRouteReposity.TouristRouteExitsAsync(touristRouteId)))
            {
                return NotFound("未找到对应旅游路线");
            }
            var picture = await _touristRouteReposity.GetPictureAsync(pictureId);
            _touristRouteReposity.DeleteTouristPictureRoute(picture);
            await _touristRouteReposity.SaveAsync();

            return NoContent();
        }
    }
}
