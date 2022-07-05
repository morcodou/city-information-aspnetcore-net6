using CityInformation.API.Entities;
using CityInformation.API.Interfaces;
using CityInformation.API.Metadata;

namespace CityInformation.API.Decorators
{
    public class LoggerCityRepositoryDecorator : ICityRepository
    {
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<LoggerCityRepositoryDecorator> _logger;

        public LoggerCityRepositoryDecorator(ICityRepository cityRepository, ILogger<LoggerCityRepositoryDecorator> logger)
        {
            _cityRepository = cityRepository;
            _logger = logger;
        }
        public Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            return _cityRepository.AddPointOfInterestForCityAsync(cityId, pointOfInterest);
        }

        public Task<bool> CityExitsAsync(int cityId)
        {
            return _cityRepository.CityExitsAsync(cityId);
        }

        public Task<bool> CityNameMatchesCityIdAsync(int cityId, string? cityName)
        {
            return _cityRepository.CityNameMatchesCityIdAsync(cityId, cityName);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _cityRepository.DeletePointOfInterest(pointOfInterest);
        }

        public Task<IEnumerable<City>> GetCitiesAsync()
        {
            _logger.LogInformation("START GetCitiesAsync");
            var citiesTask =  _cityRepository.GetCitiesAsync();
            _logger.LogInformation("END GetCitiesAsync");

            return citiesTask;
        }

        public Task<(IEnumerable<City>, Pagination)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
        {
            _logger.LogInformation("START GetCitiesAsync Pagination");
            var citiesTask = _cityRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);
            _logger.LogInformation("END GetCitiesAsync Pagination");
            return citiesTask;

        }

        public Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest = false)
        {
            return _cityRepository.GetCityAsync(cityId, includePointsOfInterest);
        }

        public Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return _cityRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        }

        public Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return _cityRepository.GetPointsOfInterestForCityAsync(cityId);
        }

        public Task<bool> SaveChangesAsync()
        {
            return _cityRepository.SaveChangesAsync();
        }
    }
}
