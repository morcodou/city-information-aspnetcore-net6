using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInformation.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        public class CityUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }
            public CityUser(
                int userId,
                string userName,
                string fisrtName,
                string lastName,
                string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = fisrtName;
                LastName = lastName;
                City = city;
            }
        }

        private readonly IConfiguration _configuration;
        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody body)
        {
            var user = ValidateUserCredentials(body.UserName, body.Password);
            if (user == null) return Unauthorized();

            var secret = _configuration["Authentification:SecretForKey"];
            var issuer = _configuration["Authentification:Issuer"];
            var audience = _configuration["Authentification:Audience"];
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim("sub", $"{user.UserId}"),
                new Claim("given_name", user.FirstName),
                new Claim("family_name", user.LastName),
                new Claim("city", user.City),
            };

            var jwtSecurityToken = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(token);
        }

        private CityUser ValidateUserCredentials(string? userName, string? password)
        {
            return new CityUser(1, userName ?? "", "from", "lrom", "Laval");
        }
    }
}
