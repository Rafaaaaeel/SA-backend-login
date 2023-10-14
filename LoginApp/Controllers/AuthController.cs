using LoginApp.Models;
using LoginApp.Dtos;
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
        public async Task<ActionResult> Login(LoginDto request)
        {
            var response = await _service.Login(request);

            if (response.Data == null)
            {
                return BadRequest();
            }

            return NoContent();
        }

    }
}