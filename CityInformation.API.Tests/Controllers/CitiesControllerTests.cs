using CityInformation.API.Controllers;
using CityInformation.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityInformation.API.Tests.Controllers
{
    [TestClass]
    public class CitiesControllerTests
    {
        private readonly CitiesController _controller;

        public CitiesControllerTests()
        {
            _controller = new CitiesController(new CitiesDataStore());
        }

        [TestMethod]
        public void GetCities_Should_Return_Ok()
        {
            var result = _controller.GetCities();
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result as OkObjectResult);
        }
    }
}
