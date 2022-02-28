using AutoFixture;
using AutoMapper;
using CityInformation.API.Controllers;
using CityInformation.API.Entities;
using CityInformation.API.Interfaces;
using CityInformation.API.Metadata;
using CityInformation.API.Models;
using CityInformation.API.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInformation.API.Tests.Controllers
{
    [TestClass]
    public class CitiesControllerTests
    {
        private readonly CitiesController _controller;
        private readonly ICityRepository _cityRepository;
        private static IMapper _mapper = null!;
        private static Fixture _fixture = null!;

        public CitiesControllerTests()
        {
            if (_mapper == null)
            {
                var profile = new CityProfile();
                var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(profile));
                _mapper = mappingConfig.CreateMapper();
            }

            if (_fixture == null)
            {
                _fixture = new Fixture();
            }

            _cityRepository = Mock.Of<ICityRepository>();
            _controller = new CitiesController(_cityRepository, _mapper)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [TestMethod]
        public async Task GetCities_Should_Return_Ok()
        {
            var cities = new List<City>()
            {
                new City("UT") {Id = 1, Description = "Desc-UT"},
                new City("UT2") {Id = 2, Description = "Desc-UT2"},
            };
            var pagination = new Pagination(2, 10, 1);
            Mock.Get(_cityRepository)
                .Setup(repository => repository.GetCitiesAsync(null, null, 1, 10))
                .ReturnsAsync((cities, pagination));

            var actionResult = await _controller.GetCities(null, null);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var citiesDto = okObjectResult?.Value as IEnumerable<CityWithoutPointsOfInterestDto>;

            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(okObjectResult);
            Assert.IsNotNull(citiesDto);
            Assert.AreEqual(citiesDto.Count(), cities.Count());
        }
    }
}
