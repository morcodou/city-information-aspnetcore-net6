using CityInformation.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInformation.API.DbContexts
{
    public class CityInformationContext : DbContext
    {
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        public CityInformationContext(DbContextOptions<CityInformationContext> options) 
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("connectionString");
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
