using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace Igor_AIS_Proj.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        //private readonly PostgresContext _context;
        private readonly IUploadResultPersistence _resultPersistence;

        public FileController(IWebHostEnvironment env, /*PostgresContext context*/
IUploadResultPersistence resultPersistence)
        {
            _env = env;
            _resultPersistence = resultPersistence;
            //_context = context;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (!BoolUseridFromClaim(out int userId))
                return Unauthorized("Access not authorized.");
            if (!BoolSessionIdFromClaim(out Guid sessionId))
                return Unauthorized("No authorization found!");

            var uploadResult = await _resultPersistence.DownloadFile(fileName);

            var path = Path.Combine(_env.ContentRootPath, "uploads", fileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, uploadResult.ContentType, Path.GetFileName(path));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<List<UploadResult>>> UploadFile(List<IFormFile> files)
        {
            if (!BoolUseridFromClaim(out int userId))
                return Unauthorized("Access not authorized.");
            if (!BoolSessionIdFromClaim(out Guid sessionId))
                return Unauthorized("No authorization found!");
            List<UploadResult> uploadResults = new List<UploadResult>();

            foreach (var file in files)
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                //var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

                trustedFileNameForFileStorage = Path.GetRandomFileName();
                var path = Path.Combine(_env.ContentRootPath, "uploads", trustedFileNameForFileStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                uploadResult.ContentType = file.ContentType;
                uploadResult.UserId = userId;
                uploadResults.Add(uploadResult);
                _resultPersistence.Create(uploadResult);
            }

            return Ok(uploadResults);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<UploadResult>>> GetAllUploadsUser()
        {
            if (!BoolUseridFromClaim(out int userId))
                return Unauthorized("Access not authorized.");
            if (!BoolSessionIdFromClaim(out Guid sessionId))
                return Unauthorized("No authorization found!");
            try
            {
                var result = await _resultPersistence.GetAllUploads(userId);
                return Ok(result);
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





        private bool BoolUseridFromClaim(out int userId) =>
           int.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value, out userId);

        private int GetUserIdFromClaim() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        private Guid GetSessionIdFromClaim() => Guid.Parse(User.FindFirst(ClaimTypes.Sid)?.Value);

        private bool BoolSessionIdFromClaim(out Guid sessionId) =>
            Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value, out sessionId);
    }
}
