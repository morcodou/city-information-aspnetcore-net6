using AutoMapper;
using CityInformation.API.Interfaces;
using CityInformation.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInformation.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cities = await _cityRepository.GetCitiesAsync();
            var citiesDto = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities);
            return Ok(citiesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityRepository.GetCityAsync(id, includePointsOfInterest);
            if (city == null) return NotFound();

            if(includePointsOfInterest)
                return Ok(_mapper.Map<CityDto>(city));

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
