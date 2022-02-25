using AutoMapper;
using CityInformation.API.Entities;
using CityInformation.API.Interfaces;
using CityInformation.API.Models;
using CityInformation.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CityInformation.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _citiesDataStore;
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityRepository cityRepository,
            IMapper mapper,
            CitiesDataStore citiesDataStore
            )
        {
            _logger = logger;
            _mailService = mailService;
            _cityRepository = cityRepository;
            _mapper = mapper;
            _citiesDataStore = citiesDataStore;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {
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
        public ActionResult UpdatePointOfInterest(
        int cityId,
        int id,
        PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null) return NotFound();

            var dbPointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
            if (dbPointOfInterest == null) return NotFound();

            dbPointOfInterest.Name = pointOfInterest.Name;
            dbPointOfInterest.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult<PointOfInterestDto> PartialUpdatePointOfInterest(
        int cityId,
        int id,
        JsonPatchDocument<PointOfInterestForUpdateDto> partialPointOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null) return NotFound();

            var dbPointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
            if (dbPointOfInterest == null) return NotFound();

            var pointOfInterest = new PointOfInterestForUpdateDto()
            {
                Name = dbPointOfInterest.Name,
                Description = dbPointOfInterest.Description,
            };

            partialPointOfInterest.ApplyTo(pointOfInterest, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!TryValidateModel(pointOfInterest)) return BadRequest(ModelState);

            dbPointOfInterest.Name = pointOfInterest.Name;
            dbPointOfInterest.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeletePointOfInterest(
         int cityId,
         int id)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null) return NotFound();

            var dbPointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
            if (dbPointOfInterest == null) return NotFound();

            city.PointsOfInterest.Remove(dbPointOfInterest);

            _mailService.Send("Point of interest deleted", $"Point of interest with id {id} and name {dbPointOfInterest.Name}");
            return NoContent();
        }

    }
}
