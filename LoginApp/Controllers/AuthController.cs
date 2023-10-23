using System.IdentityModel.Tokens.Jwt;
using LoginApp.Dtos;
using LoginApp.Models;
using LoginApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto request)
        {
            var response = await _service.Register(request);

            if (response.Data == null)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            var response = await _service.Login(request);

            if (response.Data == null)
            {
                return NotFound();
            }

            return Ok(response.Token);
        }

        [HttpPost("token")]
        public ActionResult<RefreshTokenDto> ValidateToken(Token token)
        {
            var response = _service.RefreshToken(token);

            if (response == null) return BadRequest();

            if (response.Error == true) 
            {
                return StatusCode(410);
            }

            return Ok(response.Data);
        }


    }
}