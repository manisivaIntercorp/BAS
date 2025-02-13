using DataAccessLayer.Model;
using DataAccessLayer.Interface;
using DataAccessLayer.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Eventing.Reader;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ApiBaseController
    {
        private readonly IUowOrganisation _repository;
        private readonly ILogger<OrganisationController> _logger;

        public OrganisationController(ILogger<OrganisationController> logger,IConfiguration configuration,IUowOrganisation repository) : base(configuration)
        {
            _logger = logger;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpPost("InsertOrganisation")]
        public async Task<IActionResult> InsertOrganisation(OrganisationModel orgModel)
        {
            try
            {
                var result = await _repository.OrganisationDALRepo.InsertOrganisation(orgModel);
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
                    var lsOrganisation = await _repository.OrganisationDALRepo.GetAllOrganisation();
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
