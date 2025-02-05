using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics.Eventing.Reader;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController : ApiBaseController
    {
        private readonly ILogger<LanguageController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LanguageController(ILogger<LanguageController> logger, IConfiguration configuration,IHttpContextAccessor httpContextAccessor) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            
        }
        [HttpGet("getAllLanguageinDropdown")]
        public async Task<IActionResult> GetAllLanguageinDropdown([FromQuery] LanguageNameEnum language)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var lstLanguageModel = await _repo.LanguageDALRepo.GetAllLanguageinDropdown();
                    if (lstLanguageModel != null)
                    {
                        var languages = LanguageName.GetAllLanguages();
                        return Ok(lstLanguageModel);
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

        [HttpPost("getAllLanguageinDropdown")]
        public async Task<IActionResult> InsertAllLanguageinDropdown([FromQuery] LanguageNameEnum language)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var lstLanguageModel = await _repo.LanguageDALRepo.GetAllLanguageinDropdown();
                    if (lstLanguageModel != null)
                    {
                        var languages = LanguageName.GetAllLanguages();
                        return Ok(lstLanguageModel);
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

        [HttpGet("getAllLanguage")]
        public async Task<IActionResult> GetAllLanguage()
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var lstLanguageModel = await _repo.LanguageDALRepo.GetAllLanguage();
                    if (lstLanguageModel != null)
                    {
                        return Ok(lstLanguageModel);
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
        [HttpGet("getLanguageById/{id}")]
        public async Task<IActionResult> GetLanguageById(int id)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var objLanguageModel = await _repo.LanguageDALRepo.GetLanguageById(id);
                    if (objLanguageModel != null)
                    {
                        return Ok(objLanguageModel);
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
        [HttpPost("insertUpdateLanguage")]
        public async Task<IActionResult> InsertUpdateLanguage(LanguageModel objModel)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var result = await _repo.LanguageDALRepo.InsertUpdateLanguage(objModel);
                    var msg = "Language Inserted Successfully";
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(msg);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateLanguage function in Language api controller");
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
        [HttpPut("UpdateLanguage/{id}")]
        public async Task<IActionResult> UpdateLanguage(int id, LanguageModel objModel)
        {

            if (objModel == null || id != objModel.LanguageID)
            {
                return BadRequest("Invalid data.");
            }
            else
            {
                try
                {
                    using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                    {
                        var result = await _repo.LanguageDALRepo.UpdateLanguageAsync(id, objModel);
                        var msg = "User account updated successfully.";
                        _repo.Commit();
                        if (result)
                        {
                            return Ok(msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
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
        }
        [HttpGet("deleteLanguage/{id}")]
        public async Task<IActionResult> DeleteLanguage(int id)
        {
            try
            {
                using (IUowLanguage _repo = new UowLanguage(_httpContextAccessor))
                {
                    var result = await _repo.LanguageDALRepo.DeleteLanguage(id);
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the DeleteLanguage function in Language api controller");
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

    }
}
