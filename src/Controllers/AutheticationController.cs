namespace Sa.Login.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _service;

    public AuthenticationController(IAuthenticationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Register an user at Redis cache before it confirms its emails by token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Register(RegisterRequest request)
    {
        await _service.Register(request);

        return Created();
    }

    /// <summary>
    /// Process login and generate session token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Token>> Login(LoginRequest request)
    {
        Token response = await _service.Login(request);

        return Ok(response);
    }

    /// <summary>
    /// It'll confirm user and effetive save him on database of users
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("confirm/{token}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> Confirm([FromRoute] int token)
    {
        await _service.Confirm(token);

        return Created();
    }

    /// <summary>
    /// Checks token status
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("token")]
    [ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public ActionResult<RefreshTokenResponse> ValidateToken(Token token)
    {
        RefreshTokenResponse response = _service.RefreshToken(token);

        return Ok(response);
    }

}