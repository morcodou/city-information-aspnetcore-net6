using AutoMapper;
using CityInformation.API.Entities;
using CityInformation.API.Interfaces;
using CityInformation.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInformation.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofinterest")]
    [ApiController]
    //[Authorize(Policy ="MustBeFromLaval")]
    [ApiVersion("1.0")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityRepository cityRepository,
            IMapper mapper
            )
        {
            _logger = logger;
            _mailService = mailService;
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
            var cityName = User.Claims.FirstOrDefault(x => x.Type == "city")?.Value;
            if(! await _cityRepository.CityNameMatchesCityIdAsync(cityId, cityName))
            {
                return Forbid();
            }

            if (!await _cityRepository.CityExitsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");
                return NotFound();
            }

            var pointsOfInterest = await _cityRepository.GetPointsOfInterestForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest));
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int id)
        {
            if (!await _cityRepository.CityExitsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");
                return NotFound();
            }

            var pointOfInterest = await _cityRepository.GetPointOfInterestForCityAsync(cityId, id);
            if (pointOfInterest == null)
            {
                _logger.LogInformation($"Point of interest with id {id} and city id {cityId} wasn't found");
                return NotFound();
            }
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
            int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityRepository.CityExitsAsync(cityId))
            {
                return NotFound();
            }

            var newPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);
            await _cityRepository.AddPointOfInterestForCityAsync(cityId, newPointOfInterest);
            await _cityRepository.SaveChangesAsync();
            var pointOfInterestDto = _mapper.Map<PointOfInterestDto>(newPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    id = pointOfInterestDto.Id,
                },
                pointOfInterestDto
           );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePointOfInterest(
        int cityId,
        int id,
        PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityRepository.CityExitsAsync(cityId))
            {
                return NotFound();
            }

            var dbPointOfInterest = await _cityRepository.GetPointOfInterestForCityAsync(cityId, id);
            if (dbPointOfInterest == null) return NotFound();

            _mapper.Map(pointOfInterest, dbPointOfInterest);
            await _cityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdatePointOfInterest(
        int cityId,
        int id,
        JsonPatchDocument<PointOfInterestForUpdateDto> partialPointOfInterest)
        {
            if (!await _cityRepository.CityExitsAsync(cityId))
            {
                return NotFound();
            }

            var dbPointOfInterest = await _cityRepository.GetPointOfInterestForCityAsync(cityId, id);
            if (dbPointOfInterest == null) return NotFound();


            var pointOfInterest = _mapper.Map<PointOfInterestForUpdateDto>(dbPointOfInterest);

            partialPointOfInterest.ApplyTo(pointOfInterest, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TryValidateModel(pointOfInterest)) return BadRequest(ModelState);

            _mapper.Map(pointOfInterest, dbPointOfInterest);
            await _cityRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePointOfInterest(
         int cityId,
         int id)
        {
            if (!await _cityRepository.CityExitsAsync(cityId))
            {
                return NotFound();
            }

            var dbPointOfInterest = await _cityRepository.GetPointOfInterestForCityAsync(cityId, id);
            if (dbPointOfInterest == null) return NotFound();

            _cityRepository.DeletePointOfInterest(dbPointOfInterest);
            await _cityRepository.SaveChangesAsync();

            _mailService.Send("Point of interest deleted", $"Point of interest with id {id} and name {dbPointOfInterest.Name}");
            return NoContent();
        }

    }
}
