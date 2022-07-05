using CityInformation.API.Entities;
using CityInformation.API.Metadata;

namespace CityInformation.API.Interfaces
{
    public interface ICityRepository
    {
        Task<bool> SaveChangesAsync();
        Task<bool> CityExitsAsync(int cityId);
        Task<bool> CityNameMatchesCityIdAsync(int cityId, string? cityName);
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, Pagination)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest = false);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
    }
}
