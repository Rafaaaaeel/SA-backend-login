namespace Sa.Login.Api.Controllers;

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
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        await _service.Register(request);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<Token>> Login(LoginRequest request)
    {
        Token response = await _service.Login(request);

        return Ok(response);
    }

    [HttpPost("confirm/{token}")]
    public async Task<ActionResult> ConfirmUser([FromRoute] int token)
    {
        await _service.Confirm(token);

        return Ok();
    }

    [HttpPost("token")]
    public ActionResult<RefreshTokenResponse> ValidateToken(Token token)
    {
        RefreshTokenResponse response = _service.RefreshToken(token);

        return Ok(response);
    }

}