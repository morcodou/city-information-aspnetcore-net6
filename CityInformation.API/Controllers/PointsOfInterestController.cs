using CityInformation.API.Models;
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

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger) => _logger = logger;

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null) return NotFound();

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null) return NotFound();

            var id = CitiesDataStore.Current.Cities
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
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
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null) return NotFound();

            var dbPointOfInterest = city.PointsOfInterest.FirstOrDefault(x => x.Id == id);
            if (dbPointOfInterest == null) return NotFound();

            city.PointsOfInterest.Remove(dbPointOfInterest);

            return NoContent();
        }

    }
}
