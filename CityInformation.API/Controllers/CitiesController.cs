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
        public async Task<ActionResult<CityWithoutPointsOfInterestDto>> GetCity(int id)
        {
            var city = await _cityRepository.GetCityAsync(id);
            if (city == null) return NotFound();
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
