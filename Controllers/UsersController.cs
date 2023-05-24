using DiplomaWeb.Db;
using DiplomaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DiplomaWeb.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase {
        private readonly ApplicationContext _context;



        public UsersController(ApplicationContext context) {
            _context = context;
        }



        [HttpGet("Ping")]
        public async Task<ActionResult<string>> Ping() {
            return new ObjectResult("Pong.");
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> PostUser(User inputUser) {
            User? dbUser = await _context.Users.FirstOrDefaultAsync(user => user.Login == inputUser.Login);
            if (dbUser != null) {
                return Conflict();
            }

            if (inputUser.Login == null || inputUser.Login == "" || inputUser.Password == null || inputUser.Password == "") {
                return BadRequest("Empty login or password.");
            }

            User userPuttingInDb = new() {
                Login = inputUser.Login,
                Password = HashPassword(inputUser.Password)
            };

            _context.Users.Add(userPuttingInDb);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostUser), CreateJWTToken(new Credentials(inputUser.Login, inputUser.Password)));
        }

        [HttpGet("Login")]
        public async Task<ActionResult<string>> Auth(Credentials credentials) {
            bool indentificated = await Identification(credentials!.Login);
            if (!indentificated) {
                return NotFound();
            }

            bool authenticated = await Authentication(credentials);
            if (!authenticated) {
                return Unauthorized();
            }

            string token = CreateJWTToken(credentials);

            return token;
        }



        private static string CreateJWTToken(Credentials credentials) {
            List<Claim> claims = new() {
                new Claim(ClaimTypes.Name, credentials.Login)
            };

            JwtSecurityToken jwt = new(
                issuer: "MyAuthServer",
                audience: "MyAuthClient",
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(1)),
                // TODO: key from file.
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("server_key123!!_server_key123!!")), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        
        private async Task<bool> Identification(string name) {
            User? user = await _context.Users.FirstOrDefaultAsync(user => user.Login == name);
            return user != null;
        }
        
        private async Task<bool> Authentication(Credentials credentials) {
            User user = await _context.Users.FirstAsync(user => user.Login == credentials.Login);
            return VerifyHashedPassword(user.Password!, credentials.Password);
        }

        private static string HashPassword(string password) {
            byte[] hash;
            byte[] salt = new byte[8] {
                0b0,
                0b1,
                0b10,
                0b11,
                0b100,
                0b101,
                0b110,
                0b111
            };
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, salt, 256, HashAlgorithmName.SHA256)) {
                hash = bytes.GetBytes(24);
            }

            return Convert.ToBase64String(hash);
        }
        private static bool VerifyHashedPassword(string hashedPassword, string password) {
            return HashPassword(password) == hashedPassword;
        }

    }
}
