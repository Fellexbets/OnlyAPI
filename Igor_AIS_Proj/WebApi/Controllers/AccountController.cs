using Igor_AIS_Proj.Infrastructure.KafkaServices;
using Igor_AIS_Proj.MailServices;
using Newtonsoft.Json;

namespace Igor_AIS_Proj.WebApi.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountBusiness _accountBusiness;
        private readonly ILogger<AccountController> _logger;
        private readonly ISessionBusiness _sessionBusiness;
        private readonly IUserBusiness _userBusiness;
        private readonly IProducerHandler _producerHandler;
        private readonly IMailService _mailServices;

        public AccountController(IAccountBusiness accountBusiness, ILogger<AccountController> logger, ISessionBusiness sessionBusiness, IUserBusiness userBusiness, IProducerHandler producerHandler, IMailService mailServices)
        {
            _accountBusiness = accountBusiness;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sessionBusiness = sessionBusiness;
            _userBusiness = userBusiness;
            _producerHandler = producerHandler;
            _mailServices = mailServices;
        }

        [HttpGet]
        public List<Account> GetAll() => _accountBusiness.GetAll();


        [HttpPut]
        public Task<bool> Update(Account account) => _accountBusiness.Update(account);
        [ProducesResponseType(typeof(IEnumerable<CreateAccountResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<CreateAccountResponse>> Create(CreateAccountRequest request)
        {
            if (!BoolUseridFromClaim(out int userId))
                return Unauthorized("Access not authorized.");
            if (!BoolSessionIdFromClaim(out Guid sessionId))
                return Unauthorized("No authorization found!");

            try
            {
                (bool, CreateAccountResponse) account = await _accountBusiness.Create(request, userId);
                if (account.Item2 == null) { return BadRequest(); }
                var createdAccountResponse = new CreateAccountResponse
                {
                    AccountId = account.Item2.AccountId,
                    Balance = account.Item2.Balance,
                    Currency = account.Item2.Currency,
                    UserId = userId
                };
                return StatusCode(StatusCodes.Status201Created, createdAccountResponse);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ArgumentException: return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    default: return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

        }


        [HttpGet("{accountId}")]
        [ProducesResponseType(typeof(AccountMovims), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AccountDetails>> GetAccount(int accountId)
        {
            if (!BoolSessionIdFromClaim(out Guid sessionId))
                return Unauthorized("No authorization found!");
            var session = await _sessionBusiness.GetById(sessionId);
            if (session.Active == false)
                return Unauthorized("Session is no longer active.");
            if (!BoolUseridFromClaim(out int userId))
                return Unauthorized("Access not authorized.");
            var account = _accountBusiness.GetById(accountId);
            var userIdClaim = GetUserIdFromClaim();
            if (account.Item2 == null)
                return NotFound("Account not Found!");
            if (account.Item2.UserId != userIdClaim)
                return Unauthorized("Access not authorized");
            try
            {
                (bool, IEnumerable<Movement>?, string?) accountMovims = await _accountBusiness.GetAccount(accountId, userIdClaim);
                if (accountMovims.Item2 == null) { return NoContent(); }
                return Ok(new AccountDetails
                {
                    Account = account.Item2,

                Movs = accountMovims.Item2.Select(x => new Movim
                    {
                        Amount = x.Amount,
                        CreatedAt = x.MovimentTime
                    })

                }

                    );
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ArgumentException: return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    default: return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

        }

        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<Account>>> GetAllAccountsUser()
        {
            if (!BoolSessionIdFromClaim(out Guid sessionId))
                return Unauthorized("No authorization found!");
            int userId = GetUserIdFromClaim();
            //Guid sessionId = GetSessionIdFromClaim();
            if (sessionId == null)
                return Unauthorized("No authorization found!");
            var session = await _sessionBusiness.GetById(sessionId);
            if (session.Active == false)
                return Unauthorized("Session is no longer active.");
            //if (session.TokenAccessExpireAt > DateTime.Now)
            //    return Unauthorized("Token no longer valid");

            try
            {
                var result = await _accountBusiness.GetAllAccountsUser(userId);
                return Ok(result.Item2);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ArgumentException: return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
                    default: return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }

        }

        [ProducesResponseType(typeof(AccountMovims), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<bool>> TransferFunds(TransferRequest request)
        {

            if (!BoolSessionIdFromClaim(out Guid sessionId))
                return Unauthorized("No authorization found!");
            var sessionCheck = await _sessionBusiness.GetById(sessionId);
            if (sessionCheck.Active == false)
                return Unauthorized("Session is no longer active.");
            var userId = GetUserIdFromClaim();
            var accountTransfer = _accountBusiness.GetById(request.FromAccountId);
            if (accountTransfer.Item2 is null)
                return StatusCode(StatusCodes.Status400BadRequest);
            var userTransfer = _userBusiness.GetById(userId);
            if (accountTransfer.Item2.UserId != userTransfer.UserId)
                return StatusCode(StatusCodes.Status401Unauthorized);
            try
            {
                var newSession = await _sessionBusiness.GetById(sessionId);
                var session = await _sessionBusiness.ValidateSession(newSession);
                if (session.Item1 == false)
                    return StatusCode(StatusCodes.Status401Unauthorized, session.Item2);

                
                
                var done = await _accountBusiness.TransferFunds(request);
                if (done.Item1)
                    return StatusCode(StatusCodes.Status201Created, done.Item2);
                
                return BadRequest(done.Item2);
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

        private bool BoolSessionIdFromClaim(out Guid sessionId) =>
            Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value, out sessionId);

        //private async Task<UnauthorizedObjectResult> CheckSessionActive()
        //{
        //    Guid sessionId = GetSessionIdFromClaim();
        //    var session = await _sessionBusiness.GetById(sessionId);
        //    if (session.Active == false)
        //        return Unauthorized("Session is no longer active.");
        //}


    }
}
