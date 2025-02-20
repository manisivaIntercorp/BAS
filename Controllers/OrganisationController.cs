using DataAccessLayer.Model;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Eventing.Reader;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using WebApi.Services.Interface;
using Newtonsoft.Json.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ApiBaseController
    {
        private readonly IUowOrganisation _repository;
        private readonly ILogger<OrganisationController> _logger;
        private readonly IAuditLogService _auditLogService;
        string token = string.Empty;
        string userGuid = string.Empty;
        public OrganisationController(ILogger<OrganisationController> logger,IConfiguration configuration,IUowOrganisation repository, IAuditLogService auditLogService) : base(configuration)
        {
            _logger = logger;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _auditLogService = auditLogService;
        }

        [HttpPost("InsertOrganisation")]
        public async Task<IActionResult> InsertOrganisation(OrganisationModel orgModel)
        {
            try
            {
                //if (HttpContext?.Session != null)
                //{
                //    HttpContext.Session.TryGetValue(Common.SessionVariables.Token, out var tokenBytes);
                //    HttpContext.Session.TryGetValue(Common.SessionVariables.Guid, out var guidBytes);

                //    string token = tokenBytes != null ? System.Text.Encoding.UTF8.GetString(tokenBytes) : string.Empty;
                //    string userGuid = guidBytes != null ? System.Text.Encoding.UTF8.GetString(guidBytes) : string.Empty;

                //    await _auditLogService.LogAction(userGuid, "GetDDlModules", token);
                //}

                var result = await _repository.OrganisationDALRepo.InsertOrganisation(orgModel);
                await _auditLogService.LogAction(userGuid, "InsertOrganisation", token);

                var msg = "Organisation Inserted Successfully";

                if (result == "1")
                {
                    _repository.Commit();
                    return Ok($"{msg} {result}");
                }

                _logger.LogError("Bad Request while accessing InsertOrganisation.");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} {ex.StackTrace}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("getOrganisation")]
        public async Task<IActionResult> GetAllOrganisaion()
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;

                // Retrieve session values if session exists
         
                var lsOrganisation = await _repository.OrganisationDALRepo.GetAllOrganisation();

                await _auditLogService.LogAction(userGuid, "GetAllOrganisaion", token);

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

        [HttpGet("getOrganisationById/{Guid}")]
        public async Task<IActionResult> GetOrganisationById(string Guid)
        {
            try
            {
                

                var objOrganisationModel = await _repository.OrganisationDALRepo.GetOrganisationById(Guid);
                // Log the action before returning response
                await _auditLogService.LogAction(userGuid, "GetAllOrganisation", token);
                if (objOrganisationModel != null)
                {
                    return Ok(objOrganisationModel);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }


        [HttpPut("UpdateOrganisation")]
        public async Task<IActionResult> UpdateOrganisation(string Guid, [FromBody] OrganisationModel Org)
        {

            if (Guid == null || Guid != Org.CustomerGuid)
            {
                return BadRequest("Invalid data.");
            }

            try
            {

                var result = await _repository.OrganisationDALRepo.UpdateOrganisation(Org);
                await _auditLogService.LogAction(userGuid, "UpdateOrganisation", token);
                var msg = "User account updated successfully.";
                if (result == "1")
                {
                    Ok(result);
                    Ok(msg);

                }
                else
                {
                    _logger.LogError(Environment.NewLine);
                    _logger.LogError("Bad Request occurred while accessing the update Organisation function in Organisation Update api controller");
                    return BadRequest();
                }
                return Ok(msg + result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;

            }
        }


        [HttpPost("DeleteOrganisation")]
        public async Task<IActionResult> Deleterganisation([FromBody] List<DeleteRecord> dltOrg)
        {
            try
            {
                var objOrganisationModel = await _repository.OrganisationDALRepo.DeleteOrganisation(dltOrg);
                await _auditLogService.LogAction(userGuid, "Deleterganisation", token);

                if (objOrganisationModel != null)
                {
                    return Ok(objOrganisationModel);
                }
                else
                {
                    return BadRequest();
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
