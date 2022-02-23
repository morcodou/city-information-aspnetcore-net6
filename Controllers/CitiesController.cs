using Microsoft.AspNetCore.Mvc;

namespace CityInformation.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        [HttpGet()]
        public JsonResult Cities()
        {
            return new JsonResult(
                new List<object>()
                {
                    new { id = 1, name  = "name1"},
                    new { id = 2, name  = "name2"},
                }
            );
        }
    }
}
