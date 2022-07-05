using CityInformation.API.DbContexts;
using CityInformation.API.Entities;
using CityInformation.API.Interfaces;
using CityInformation.API.Metadata;
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

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context
                .PointsOfInterest
                .FirstOrDefaultAsync(c => c.CityId == cityId && c.Id == pointOfInterestId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
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

        public async Task<bool> CityExitsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public async Task<(IEnumerable<City>, Pagination)> GetCitiesAsync(
            string? name, 
            string? searchQuery,
            int pageNumber,
            int pageSize)
        {
            var cities = _context.Cities as IQueryable<City>;
            if (!string.IsNullOrWhiteSpace(name))
            {
                cities = cities.Where(c => c.Name == name);
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                cities = cities.Where(c => 
                                    c.Name.Contains(searchQuery)
                                    || (c.Description != null && c.Description.Contains(searchQuery)));
            }

            var totalItemCount = await cities.CountAsync();
            var pagination = new Pagination(totalItemCount, pageSize, pageNumber);
            var filetred = await cities
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .OrderBy(c => c.Name).ToListAsync();

            Console.WriteLine("filetred pagination");

            return (filetred, pagination);
        }

        public async Task<bool> CityNameMatchesCityIdAsync(int cityId, string? cityName)
        {
            return await _context.Cities.AnyAsync(c => c.Id==cityId && c.Name == cityName);
        }
    }
}
