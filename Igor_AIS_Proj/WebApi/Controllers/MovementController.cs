namespace Igor_AIS_Proj.WebApi.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class MovementController : ControllerBase
    {
        private readonly IMovementBusiness _movementBusiness;
        private readonly ILogger<MovementController> _logger;

        public MovementController(IMovementBusiness movementBusiness, ILogger<MovementController> logger)
        {
            _movementBusiness = movementBusiness;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        [HttpGet("{id}")]
        public async Task<Movement> GetById(int id) => await _movementBusiness.GetById(id);

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id) => await _movementBusiness.Delete(id);

        [HttpGet]
        public List<Movement> GetAll() => _movementBusiness.GetAll();

        [HttpPut]
        public Task<bool> Update(Movement movement) => _movementBusiness.Update(movement);

        [HttpPost]
        public async Task<Movement> Create(Movement movement) => await _movementBusiness.Create(movement);
    }

}

