﻿using CityInformation.API.Entities;

namespace CityInformation.API.Interfaces
{
    public interface ICityRepository
    {
        Task<bool> CityExitsAsync(int cityId);
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest = false);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
    }
}
