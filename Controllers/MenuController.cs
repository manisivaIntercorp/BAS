using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class MenuController : ApiBaseController
    {

        private readonly IUowMenu _repository;
        private readonly ILogger<MenuController> _logger;
        private readonly IAuditLogService _auditLogService;

        public MenuController(ILogger<MenuController> logger, IConfiguration configuration, IUowMenu repository, IAuditLogService auditLogService) : base(configuration)
        {
            _logger = logger;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _auditLogService = auditLogService;
        }

        //[HttpGet("getModules")]
        //public async Task<IActionResult> GetModules()
        //{
        //    try
        //    {

        //        string token = string.Empty;
        //        string userGuid = string.Empty;

        //        // Retrieve session values if session exists

        //        var lsOrganisation = await _repository.MenuDALRepo.GetAllMenu();

        //        await _auditLogService.LogAction(userGuid, "GetModules", token);

        //        if (lsOrganisation != null)
        //        {
        //            return Ok(lsOrganisation);
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message + " " + ex.StackTrace);
        //        return StatusCode(500);
        //    }
        //}

        [HttpGet("Get-GlobalMenu")]
        public async Task<IActionResult> GetMenu()
        {
            try
            {

                string UserName  = HttpContext.Session.GetString(Common.SessionVariables.UserName);
                string ClientCode = "";
                int OrgID = 0;

                string token = string.Empty;
                string userGuid = string.Empty;

                // Retrieve session values if session exists

                var lsOrganisation = await _repository.MenuDALRepo.GetMenu(UserName,ClientCode,OrgID);

                await _auditLogService.LogAction(userGuid, "GetModules", token);

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
    }
}
