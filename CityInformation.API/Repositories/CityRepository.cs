﻿using CityInformation.API.DbContexts;
using CityInformation.API.Entities;
using CityInformation.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CityInformation.API.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly CityInformationContext _context;

        public CityRepository(CityInformationContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest = false)
        {
            var cities = _context.Cities.Where(c => c.Id == cityId);
            if (includePointsOfInterest)
            {
                cities = cities.Include(c => c.PointsOfInterest);
            }

            return await cities.FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestAsync(int cityId, int pointOfInterestId)
        {
            return await _context
                .PointsOfInterest
                .FirstOrDefaultAsync(c => c.CityId == cityId && c.Id == pointOfInterestId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId)
        {
            return await _context
                          .PointsOfInterest
                          .Where(c => c.CityId == cityId)
                          .ToListAsync();
        }

        public Task<IEnumerable<PointOfInterest>> GetPointsOfInterestAsync(int cityId, bool includePointsOfInterest = false)
        {
            throw new NotImplementedException();
        }
    }
}
