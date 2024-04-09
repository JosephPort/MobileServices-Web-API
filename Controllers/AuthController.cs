using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MobileServices_Web_API.Data;
using MobileServices_Web_API.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace MobileServices_Web_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public User _user { get; set; } = null!;

        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;

        public AuthController(IConfiguration configuration, DataContext dataContext)
        {
            _configuration = configuration;
            _dataContext = dataContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO request)
        {
            _user = new User();
            var dbUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (dbUser is not null)
            {
                return BadRequest("Username is taken");
            }

            _user.Username = request.Username;
            _user.PasswordHashed = BCrypt.Net.BCrypt.HashPassword(request.Password);

            _dataContext.Users.Add(_user);
            await _dataContext.SaveChangesAsync();

            var token = CreateToken(_user);

            var response = new
            {
                token = token
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO request)
        {
            var dbUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (dbUser is null || !BCrypt.Net.BCrypt.Verify(request.Password, dbUser.PasswordHashed))
            {
                return BadRequest("Invalid username or password");
            }

            var token = CreateToken(_user);

            var response = new
            {
                token = token
            };

            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(UserDTO request)
        {
            var dbUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (dbUser is null)
            {
                return BadRequest("Invalid username");
            }

            dbUser.PasswordHashed = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _dataContext.SaveChangesAsync();

            return Ok("Password changed");
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
