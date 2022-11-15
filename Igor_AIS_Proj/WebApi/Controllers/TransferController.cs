namespace Igor_AIS_Proj.WebApi.Controllers
{
    [ApiController, Route("[controller]/[action]")]
    public class TransferController : ControllerBase
    {
        private readonly ITransferBusiness _transferBusiness;
        private readonly ILogger<TransferController> _logger;
        public TransferController(ITransferBusiness transferBusiness, ILogger<TransferController> logger)
        {
            _transferBusiness = transferBusiness;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}")]
        public async Task<Transfer> GetById(int id) => await _transferBusiness.GetById(id);

        [HttpDelete("{id}")]
        public async Task<bool> Delete(int id) => await _transferBusiness.Delete(id);

        [HttpGet]
        public List<Transfer> GetAll() => _transferBusiness.GetAll();

        [HttpPut]
        public Task<bool> Update(Transfer transfer) => _transferBusiness.Update(transfer);

        [HttpPost]
        public async Task<Transfer> Create(Transfer transfer) => await _transferBusiness.Create(transfer);

        //[ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        //[HttpGet("{id}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<ActionResult<List<Transfer>>> GetAllTransfersAccount(int id)
        //{
        //    if (id != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
        //    {
        //        var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "You can only access information about your account!" };
        //        throw new System.Web.Http.HttpResponseException(msg);
        //    }
        //    try
        //    {
        //        _transferBusiness.GetById(id);
        //        return await _transferBusiness.GetAllTransfersAccount(id);
        //    }
        //    catch (Exception ex)
        //    {
        //        switch (ex)
        //        {
        //            case ArgumentException: return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
        //            default: return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //        }
        //    }

        //}

    }
}
