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

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            CitiesDataStore citiesDataStore
            )
        {
            _logger = logger;
            _mailService = mailService;
            _citiesDataStore = citiesDataStore;
        } 

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");
                    return NotFound();
                }

                return Ok(city.PointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                return StatusCode(500, "A problem happened while handling your request");
            }

        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int id)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)  {
                _logger.LogInformation($"City with id {cityId} wasn't found when accessing point of interest");
                  return  NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
            if (pointOfInterest == null) return NotFound();
            return Ok(pointOfInterest);
        }

        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(
            int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null) return NotFound();

            var id = _citiesDataStore.Cities
                .SelectMany(x => x.PointsOfInterest)
                .Max(x => x.Id);

            var newPointOfInterest = new PointOfInterestDto()
            {
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description,
                Id = id + 1,
            };

            city.PointsOfInterest.Add(newPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    id = newPointOfInterest.Id,
                },
                newPointOfInterest
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
            if(!ModelState.IsValid) return BadRequest(ModelState);
            if(!TryValidateModel(pointOfInterest)) return BadRequest(ModelState);

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
