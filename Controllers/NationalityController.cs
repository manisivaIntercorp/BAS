using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Eventing.Reader;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalityController : ApiBaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //public readonly IUowNationality _repo;
        // private readonly IConfiguration _configuration;
        //public NationalityController(IUowNationality repo,IConfiguration configuration):base(configuration)
        //{
        //    this._repo = repo;

        //}

        private readonly ILogger<NationalityController> _logger;
        
        public NationalityController(ILogger<NationalityController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            //var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            //NLog.GlobalDiagnosticsContext.Set("LoggingDirectory", logPath);
        }
        // private readonly IConfiguration _config;
        //public NationalityController()
        //{
        //   // IUowNationality _repo = new UowNationality(_httpContextAccessor);
        //}

        [HttpGet("getAllNationality")]
        public async Task<IActionResult> GetAllNationality()
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    var lstNationalityModel = await _repo.NationalityDALRepo.GetAllNationality();
                    if (lstNationalityModel != null)
                    {
                        return Ok(lstNationalityModel);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpGet("getNationalityById/{code}")]
        public async Task<IActionResult> GetNationalityById(int code)
        {
            try
            {
                int i = 0;
                var j = 1 / i;
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    var objNationalityModel = await _repo.NationalityDALRepo.GetNationalityById(code);
                    if (objNationalityModel != null)
                    {
                        return Ok(objNationalityModel);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpPost("insertUpdateNationality")]
        public async Task<IActionResult> InsertUpdateNationality(NationalityModel objModel)
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    var result = await _repo.NationalityDALRepo.InsertUpdateNationality(objModel);
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateNationality function in Nationality api controller");
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex )
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpGet("deleteNationality/{code}")]
        public async Task<IActionResult> DeleteNationality(int code)
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    var result = await _repo.NationalityDALRepo.DeleteNationality(code);
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the DeleteNationality function in Nationality api controller");
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  "+ ex.StackTrace);
                throw;
            }
        }
    }
}
