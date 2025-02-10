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
        [HttpGet("getNationalityById/{id}")]
        public async Task<IActionResult> GetNationalityById(int id)
        {
            try
            {
               
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    var objNationalityModel = await _repo.NationalityDALRepo.GetNationalityById(id);
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
                    if (result.Insertnationality==true || result.Insertnationality == false)
                    {
                        switch (result.RetVal)
                        {
                            case 1://Success
                                return Ok(result.Msg);
                            case -2:// Failure in Nationality Name
                                return Ok(result.Msg);
                            case -1:// Failure in Nationality Name
                                return Ok(result.Msg);
                            default:
                                _logger.LogError(Environment.NewLine);
                                _logger.LogError("Bad Request occurred while accessing the InsertUpdateNationality function in Nationality api controller");
                                return BadRequest();
                        }
                        
                    }
                    
                }
                return Ok();
            }
            catch (Exception ex )
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        [HttpPut("UpdateNationality")]
        public async Task<IActionResult> UpdateNationality(NationalityModel objModel)
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    var result = await _repo.NationalityDALRepo.UpdateNationality(objModel);
                    _repo.Commit();
                    if (result.Updatenationality == true || result.Updatenationality == false)
                    {
                        switch (result.RetVal)
                        {
                            case 1://Success
                                return Ok(result.Msg);
                            case -2:// Failure in Nationality Name
                                return Ok(result.Msg);
                            case -1:// Failure in Nationality Name
                                return Ok(result.Msg);
                            default:
                                _logger.LogError(Environment.NewLine);
                                _logger.LogError("Bad Request occurred while accessing the InsertUpdateNationality function in Nationality api controller");
                                return BadRequest();
                        }
                    }

                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpDelete("deleteNationality/{id}")]
        public async Task<IActionResult> DeleteNationality(int id)
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    var result = await _repo.NationalityDALRepo.DeleteNationality(id);
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
