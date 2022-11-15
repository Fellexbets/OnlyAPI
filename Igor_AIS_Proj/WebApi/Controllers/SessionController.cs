namespace Igor_AIS_Proj.WebApi.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionBusiness _sessionBusiness;
        private readonly ILogger<SessionController> _logger;

        public SessionController(ISessionBusiness sessionBusiness, ILogger<SessionController> logger)
        {
            _sessionBusiness = sessionBusiness;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpDelete]
        public async Task<bool> Delete(Session session) => await _sessionBusiness.Delete(session);

        [HttpGet]
        public List<Session> GetAll() => _sessionBusiness.GetAll();

        [HttpPut]
        public Task<bool> Update(Session session) => _sessionBusiness.Update(session);

        [HttpPost]
        public async Task<Session> Create(Session session) => await _sessionBusiness.Create(session);

        [HttpPost]
        public string? DeleteInactiveSessions()
        {
            _sessionBusiness.DeleteInactiveSessions();
            return "Inactive Sessions deleted!";
        }
    }

}

