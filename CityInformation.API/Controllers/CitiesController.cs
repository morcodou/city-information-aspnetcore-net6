using AutoMapper;
using CityInformation.API.Interfaces;
using CityInformation.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CityInformation.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    [Authorize]
    public class CitiesController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;
        private const int _MAX_CITIES_PAGE_SIZE = 20;

        public CitiesController(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>>
            GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (pageSize > _MAX_CITIES_PAGE_SIZE)
            {
                pageSize = _MAX_CITIES_PAGE_SIZE;
            }
            var (cities, pagination) = await _cityRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            var citiesDto = _mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cities);
            var serialized = JsonSerializer.Serialize(pagination);
            Response.Headers.Add("X-Pagination", serialized);
            return Ok(citiesDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityRepository.GetCityAsync(id, includePointsOfInterest);
            if (city == null) return NotFound();

            if (includePointsOfInterest)
                return Ok(_mapper.Map<CityDto>(city));

            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
