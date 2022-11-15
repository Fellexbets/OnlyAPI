using Microsoft.Extensions.Primitives;

namespace Igor_AIS_Proj.WebApi.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IUserBusiness _userBusiness;
        private readonly ILogger<UserController> _logger;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IJwtServices _jwtServices;

        public UserController(IUserBusiness userBusiness, ILogger<UserController> logger, ISessionBusiness sessionBusiness, IJwtServices jwtServices)
        {
            _userBusiness = userBusiness;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sessionBusiness = sessionBusiness;
            _jwtServices = jwtServices;
        }


        [HttpGet("{id}")]
        public User GetById(int id) => _userBusiness.GetById(id);

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id) => await _userBusiness.Delete(id);

        [HttpGet]
        public List<User> GetAll() => _userBusiness.GetAll();

        [HttpPut]
        public Task<bool> Update(User user) => _userBusiness.Update(user);

        [HttpPost, AllowAnonymous]
        [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginUserResponse>> Authenticate(LoginUserRequest model)
        {
            try
            {
                User user = new User
                {
                    Email = model.Email,
                    UserPassword = model.UserPassword
                };
                var result = await _userBusiness.Authenticate(model);
                if (result.Item1)
                {
                    LoginUserResponse response = new LoginUserResponse
                    {
                        SessionId = result.Item4.SessionId.ToString(),
                        AccessToken = result.Item4.TokenAccess,
                        AccessTokenExpiresAt = result.Item4.TokenAccessExpireAt,
                        RefreshToken = result.Item4.RefreshToken,
                        RefreshTokenExpiresAt = result.Item4.Refresh_Token_expire_At,
                        User = new CreateUserResponse
                        {
                            UserId = result.Item3.UserId,
                            Username = result.Item3.Username,
                            Email = result.Item3.Email,
                            FullName = result.Item3.FullName,
                            CreatedAt = result.Item3.CreatedAt
                        }
                    };
                    return Ok(response);
                }
                return Unauthorized(result.Item2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return Problem(ex.Message);
            }
        }


        [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpPost, AllowAnonymous]

        public async Task<ActionResult<CreateUserResponse>> Create(CreateUserRequest request)
        {
            var user = await _userBusiness.Create(request);
            if (user == null) { return BadRequest("User could not be created"); }
            return Ok(user);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string?>> Logout()
        {
            try
            {
                //if (!BoolUseridFromClaim(out int userId))
                //    return Unauthorized("No authorization found!");
                if (!Request.Headers.TryGetValue("Authorization", out StringValues authToken))
                    return BadRequest("No authorization found.");
                int userId = GetUserIdFromClaim();
                if (userId == null)
                    return BadRequest("User Id not found.");
                Guid sessionId = GetSessionIdFromClaim();
                if (sessionId == null)
                    return BadRequest("Session Id not found.");
                var session = new Session
                {
                    SessionId = sessionId,
                    UserId = userId,
                    TokenAccess = authToken
                };
                var result = await _userBusiness.Logout(session);
                if (result.Item1)
                    return Ok("Logout completed!");
                return BadRequest(result.Item2);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoginUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LoginUserResponse>> RenewLogin(RenewLoginRequest tokenRefresh)
        {

            if (!Request.Headers.TryGetValue("Authorization", out StringValues authToken))
                return Unauthorized("Missing Authorization Info.");
            try
            {
                Guid sessionId = GetSessionIdFromClaim();
                int userId = GetUserIdFromClaim();
                if (userId == null)
                    return BadRequest("No user info on token");
                if (sessionId == null)
                    return BadRequest("No session info on token");
                if (tokenRefresh == null)
                    return BadRequest("Token Refresh is null");
                var tokenClean = _jwtServices.CleanToken(authToken);
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(tokenClean.Item2);
                var session = new Session
                {
                    SessionId = sessionId,
                    UserId = userId,
                    TokenAccess = tokenClean.Item2,
                    TokenAccessExpireAt = securityToken.ValidTo
                };
                var verifiedSession = await _userBusiness.VerifySession(session);
                if (!verifiedSession.Item1)
                    return Unauthorized(verifiedSession.Item2);
                var result = await _userBusiness.RenewLogin(verifiedSession.Item3, tokenRefresh.RefreshToken);
                if (result.Item1)
                {
                    LoginUserResponse response = new LoginUserResponse
                    {
                        AccessToken = result.Item4.TokenAccess,
                        AccessTokenExpiresAt = result.Item4.TokenAccessExpireAt,
                        RefreshToken = result.Item4.RefreshToken,
                        RefreshTokenExpiresAt = result.Item4.Refresh_Token_expire_At,
                        SessionId = result.Item4.SessionId.ToString(),
                        User = new CreateUserResponse
                        {
                            UserId = result.Item3.UserId,
                            Username = result.Item3.Username,
                            Email = result.Item3.Email,
                            FullName = result.Item3.FullName,
                            CreatedAt = result.Item3.CreatedAt
                        }
                    };
                    return Ok(response);
                }
                return Unauthorized(result.Item2);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.InnerException);
                return Problem(ex.Message);
            }



        }

        private bool BoolUseridFromClaim(out int userId) =>
            int.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out userId);

        private int GetUserIdFromClaim() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        private Guid GetSessionIdFromClaim() => Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);
    }
}
