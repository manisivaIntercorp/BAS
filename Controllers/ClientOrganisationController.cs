using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.Services.Interface;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientOrganisationController : ApiBaseController
    {
        private readonly IUowClientOrganisation _repository;
        private readonly ILogger<ClientOrganisationController> _logger;
        private readonly IAuditLogService _auditLogService;

            string token = string.Empty;
            string userGuid = string.Empty;
        public ClientOrganisationController(ILogger<ClientOrganisationController> logger, IConfiguration configuration, IUowClientOrganisation repository, IAuditLogService auditLogService) : base(configuration)
        {
            _logger = logger;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _auditLogService = auditLogService;
        }

        [HttpGet("getOrganisation")]
        public async Task<IActionResult> GetAllOrganisaion()
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;

                // Retrieve session values if session exists

                var lsOrganisation = await _repository.ClientOrganisationDALRepo.GetClientOrganisation();

                await _auditLogService.LogAction(userGuid, "GetAllOrganisaion", token);

                if (lsOrganisation != null)
                {
                    return Ok(lsOrganisation);
                }
                else
                {
                    return BadRequest();
                }
                _logger.LogError("test");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpGet("getOrganisationLevelInfo")]
        public async Task<IActionResult> GetOrganisationLevelInfo()
        {
            try
            {
                string token = string.Empty;
                string userGuid = string.Empty;

                var organisationLevelModel = new OrganisationLevelModel
                {
                    Mode = "GET_PARENT"//,
                    //LevelID = !string.IsNullOrEmpty(LevelID) ? Convert.ToInt64(LevelID) : (long?)null,
                    //LevelCode = LevelCode,
                    //LevelDesc = LevelDesc,
                    //ParentLevelID = !string.IsNullOrEmpty(ParentLevelID) ? Convert.ToInt64(ParentLevelID) : (long?)null,
                    //IsProject = IsProject,
                    //ModifiedBy = !string.IsNullOrEmpty(ModifiedBy) ? Convert.ToInt64(ModifiedBy) : (long?)null
                };

                var lsOrganisation = await _repository.ClientOrganisationDALRepo.GetOrganisationLevelInfo(organisationLevelModel);
                await _auditLogService.LogAction(userGuid, "GetOrganisationLevelInfo", token);

                if (lsOrganisation != null)
                {
                    return Ok(lsOrganisation);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }


        [HttpGet("SelectOrganisationLevelInfo")]
        public async Task<IActionResult> SelectOrganisationLevelInfo([FromQuery] string? LevelGuid)
        {
            try
            {
                string token = string.Empty;
                string userGuid = string.Empty;

                var organisationLevelModel = new OrganisationLevelModel
                {
                    Mode = "GET",
                    LevelGuid = LevelGuid
                };

                var lsOrganisation = await _repository.ClientOrganisationDALRepo.SelectOrganisationLevelInfo(organisationLevelModel);
                await _auditLogService.LogAction(userGuid, "GetOrganisationLevelInfo", token);

                if (lsOrganisation != null)
                {
                    return Ok(lsOrganisation);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }


        [HttpGet("OrganisationLevelEdit")]
        public async Task<IActionResult> OrganisationLevelEdit(
        [FromQuery] string? LevelCode,
        [FromQuery] string? LevelGuid,
        [FromQuery] string? LevelDesc,
        [FromQuery] string? IsProject,
        [FromQuery] string? ParentLevelGuid)
        {
            try
            {
                string token = string.Empty;
                string struserGuid = string.Empty;
                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid);
                }
                var organisationLevelModel = new OrganisationLevelModel
                {
                    Mode = "EDIT",
                    LevelCode  = LevelCode,
                    LevelDesc = LevelDesc,
                    LevelGuid = LevelGuid,
                    ParentLevelGuid = ParentLevelGuid,
                    IsProject = IsProject,
                    UserGuid = struserGuid
                    //LevelID = !string.IsNullOrEmpty(LevelID) ? Convert.ToInt64(LevelID) : (long?)null,
                    //LevelCode = LevelCode,
                    //LevelDesc = LevelDesc,
                    //ParentLevelID = !string.IsNullOrEmpty(ParentLevelID) ? Convert.ToInt64(ParentLevelID) : (long?)null,
                    //IsProject = IsProject,
                    //ModifiedBy = !string.IsNullOrEmpty(ModifiedBy) ? Convert.ToInt64(ModifiedBy) : (long?)null
                };

                var lsOrganisation = await _repository.ClientOrganisationDALRepo.OrganisationLevelEdit(organisationLevelModel);
                await _auditLogService.LogAction(userGuid, "GetOrganisationLevelInfo", token);

                if (lsOrganisation != null)
                {
                    return Ok(lsOrganisation);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                return StatusCode(500);
            }
        }

        [HttpGet("OrganisationLevelDelete")]
        public async Task<IActionResult> DeleteOrganisationLevel(List<LevelInfoDetails> levelInfoDetails)
        {
            try
            {
                string strUserGuid = HttpContext?.Session?.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                string strMode = "DELETE";

                // Ensure levelInfoDetails is a collection
                List<DeleteResultModel> lstDelete = await _repository.ClientOrganisationDALRepo.DeleteOrganisationLevel(levelInfoDetails, strUserGuid, strMode);
                await _auditLogService.LogAction(strUserGuid, "GetOrganisationLevelInfo", string.Empty);

                if (lstDelete != null && lstDelete.Count > 0)
                {
                    return Ok(lstDelete);
                }
                else
                {
                    return BadRequest("No records found to delete.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteOrganisationLevel: {ex.Message} {ex.StackTrace}");
                return StatusCode(500, "An error occurred while deleting the organisation level.");
            }
        }

    }
}
