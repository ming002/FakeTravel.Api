using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FakeTravel.API.Services;
using System;
using System.Linq;
using AutoMapper;
using FakeTravel.API.Dtos;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FakeTravel.API.ResourceParameters;
using FakeTravel.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using FakeTravel.API.Helper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace FakeTravel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristRoutesController : ControllerBase
    {
        private ITouristRouteReposity _touristRouteReposity;
        private readonly IMapper _mapper;
        public TouristRoutesController(
            ITouristRouteReposity touristRouteReposity,
            IMapper mapper)
        {
            _touristRouteReposity = touristRouteReposity;
            _mapper = mapper;
        }
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetTouristRoutes(
            [FromQuery] TouristRouteResourceParameters parameters)
        {

            var routes =await _touristRouteReposity.
                 GetTouristRoutesAsync(parameters.Keyword, parameters.RatingOperator, parameters.RatingValue);

            if (routes == null || routes.Count() <= 0)
            {
                return NotFound("找不到旅游路线");
            }
            var touristRouteDto = _mapper.Map<IEnumerable<TouristRouteDto>>(routes);
            return Ok(touristRouteDto);
        }

        [HttpGet("{touristRouteId}", Name = "GetTouristRouteById")]
        [HttpHead]
        public async Task<IActionResult> GetTouristRouteById(Guid touristRouteId)
        {
            var route =await _touristRouteReposity.GetTouristRouteAsync(touristRouteId);
            if (route == null)
            {
                return NotFound("未找到对应旅游路线");
            }
            var touristRouteDto = _mapper.Map<TouristRouteDto>(route);
            return Ok(touristRouteDto);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes ="Bearer")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateTouristRoute([FromBody] TouristRouteForCreationDto touristRouteForCreationDto)
        {
            var touristRouteModel = _mapper.Map<TouristRoute>(touristRouteForCreationDto);
            _touristRouteReposity.AddTouristRoute(touristRouteModel);
            await _touristRouteReposity.SaveAsync();
            var touristRouteToReturn = _mapper.Map<TouristRouteDto>(touristRouteModel);
            return CreatedAtRoute("GetTouristRouteById",
                new { touristRouteId = touristRouteModel.Id },
                touristRouteToReturn);
        }

        [HttpPut("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] TouristRouteForUpdateDto touristRouteForUpdateDto)
        {

            if (!(await _touristRouteReposity.TouristRouteExitsAsync(touristRouteId)))
            {
                return NotFound("未找到对应旅游路线");
            }
            var touristRouteFromRepo = await _touristRouteReposity.GetTouristRouteAsync(touristRouteId);
            _mapper.Map(touristRouteForUpdateDto, touristRouteFromRepo);
            await _touristRouteReposity.SaveAsync();
            return NoContent();
        }

        [HttpPatch]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PartiallyUpdateTouristRoute(
            [FromRoute] Guid touristRouteId,
            [FromBody] JsonPatchDocument<TouristRouteForUpdateDto> patchDocument)
        {
            if (!(await _touristRouteReposity.TouristRouteExitsAsync(touristRouteId)))
            {
                return NotFound("未找到对应旅游路线");
            }
            var touristRouteFromRepo =await _touristRouteReposity.GetTouristRouteAsync(touristRouteId);
            var touristRouteToPatch = _mapper.Map<TouristRouteForUpdateDto>(touristRouteFromRepo);
            patchDocument.ApplyTo(touristRouteToPatch,ModelState);
            if(!TryValidateModel(touristRouteToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(touristRouteToPatch, touristRouteFromRepo);
            await _touristRouteReposity.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{touristRouteId}")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTouristRoute([FromRoute] Guid touristRouteId)
        {
            if (!(await _touristRouteReposity.TouristRouteExitsAsync(touristRouteId)))
            {
                return NotFound("未找到对应旅游路线");
            }
            var touristRoute = await _touristRouteReposity.GetTouristRouteAsync(touristRouteId);
            _touristRouteReposity.DeleteTouristRoute(touristRoute);
            await _touristRouteReposity.SaveAsync();
            return NoContent();
        }
        [HttpDelete("({touristIDs})")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteIDs(
            [ModelBinder(BinderType =typeof(ArrayModelBinder))]
            [FromRoute]IEnumerable<Guid> touristIDs)
        {
            if (touristIDs == null)
            {
                return BadRequest();
            }
            var touristRoutesFormRepo= await _touristRouteReposity.GetTouristRoutesByIDListAsync(touristIDs);
            _touristRouteReposity.DeleteTouristRoutes(touristRoutesFormRepo);
            await _touristRouteReposity.SaveAsync();
            return NoContent();
        }
    }
}
