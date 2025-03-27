using DataAccessLayer.Model;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class DepartmentController : ApiBaseController
    {
        public readonly IUowDepartment _repository;
        private readonly ILogger<DepartmentController> _logger;
        private readonly IAuditLogService _auditLogService;

        string token = string.Empty;
        string userGuid = string.Empty;

        public DepartmentController(ILogger<DepartmentController> logger, IConfiguration configuration, IUowDepartment repository, IAuditLogService auditLogService) : base(configuration)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
            _auditLogService = auditLogService;
        }
        
        [HttpGet("GetAllDepartment")]
        public async Task<IActionResult> GetAllDepartment([FromQuery] string? LevelDetailGUID = "067427BF-2613-4C17-89DB-B1D00704AD15")
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;

                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    if (string.IsNullOrEmpty(LevelDetailGUID))
                    {
                        strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    }
                    else
                    {
                        strLevelDetailGUID = LevelDetailGUID;
                    }
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }

                var departmentInput = new DepartmentInput
                {
                    Mode = "GET_DETAIL",
                     UpdatedGuidBy = struserGuid,
                    LevelDetailGUID = strLevelDetailGUID,
                    DeptGUID ="",
                    Function = "",
                    //TimeZoneID =0

                 };
                // Retrieve session values if session exists
                var lsOrganisation = await _repository.DepartmentDALRepo.GetDepartment(departmentInput);

                await _auditLogService.LogAction(userGuid, "GetAllDepartment", token);

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

        [HttpGet("GetDepartmentDetails")]
        public async Task<IActionResult> GetDepartmentDetails([FromQuery] string DepartmentGuid,[FromQuery] string? LevelDetailGUID = "067427BF-2613-4C17-89DB-B1D00704AD15")
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;

                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    if (string.IsNullOrEmpty(LevelDetailGUID))
                    {
                        strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    }
                    else
                    {
                        strLevelDetailGUID = LevelDetailGUID;
                    }
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }

                var departmentInput = new DepartmentInput
                {
                    Mode = "GET_DETAIL",
                    UpdatedGuidBy = struserGuid,
                    LevelDetailGUID = strLevelDetailGUID,
                    DeptGUID = DepartmentGuid,
                    Function = "",
                    //TimeZoneID =0

                };
                // Retrieve session values if session exists
                var lsOrganisation = await _repository.DepartmentDALRepo.GetDepartment(departmentInput);

                await _auditLogService.LogAction(userGuid, "GetAllDepartment", token);

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
        [HttpGet("ViewDepartmentDetails")]
        public async Task<IActionResult> ViewDepartmentDetails([FromQuery] string DepartmentGuid, [FromQuery] string? LevelDetailGUID = "067427BF-2613-4C17-89DB-B1D00704AD15")
        {
            try
            {

                string token = string.Empty;
                string userGuid = string.Empty;

                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    if (string.IsNullOrEmpty(LevelDetailGUID))
                    {
                        strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    }
                    else
                    {
                        strLevelDetailGUID = LevelDetailGUID;
                    }
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }

                var departmentInput = new DepartmentInput
                {
                    Mode = "VIEW",
                    UpdatedGuidBy = struserGuid,
                    LevelDetailGUID = strLevelDetailGUID,
                    DeptGUID = DepartmentGuid,
                    Function = "",
                    //TimeZoneID =0

                };
                // Retrieve session values if session exists
                var lsOrganisation = await _repository.DepartmentDALRepo.ViewDepartment(departmentInput);

                await _auditLogService.LogAction(userGuid, "GetAllDepartment", token);

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

        [HttpPost("InsertDepartmentDetails")]
        public async Task<IActionResult> InsertDepartmentDetails(
            
            [FromQuery] string DepartmentCode,
            [FromQuery] string DepartmentDescription,
            [FromQuery] long? ReferenceID,
            [FromQuery] string? ColourCode,
            [FromBody] List<DepartmentDetail> lstDepartmentDetail,
            [FromQuery] string? LevelDetailGUID = "067427BF-2613-4C17-89DB-B1D00704AD15"
            //, // Change to FromBody
            //[FromHeader(Name = "UserGuid")] string userGuid,
            //[FromHeader(Name = "LevelDetailGUID")] string levelDetailGuid,
            //[FromHeader(Name = "LevelGUID")] string levelGuid,
            //[FromHeader(Name = "TimeZoneID")] string? timeZoneID
            )
        {
            try
            {
     
                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
                if(String.IsNullOrEmpty(strLevelDetailGUID))
                {
                    strLevelDetailGUID = LevelDetailGUID;
                }
                var departmentInput = new DepartmentInput
                {
                    DeptCode = DepartmentCode,
                    DeptDesc = DepartmentDescription,
                    ReferenceID = ReferenceID,
                    ColourCode = ColourCode,
                    LevelDetailGUID = strLevelDetailGUID,
                    LevelGUID = strLevelGUID,
                    UpdatedGuidBy = userGuid,
                    lstDepart = lstDepartmentDetail,
                    TimeZoneID = int.TryParse(strTimeZoneID, out int tz) ? tz : 0
                };

                var result = await _repository.DepartmentDALRepo.InsertDepartmentDetails(departmentInput);
                return result == "1"
                    ? Ok(new { Message = "Department details inserted successfully." })
                    : BadRequest(new { Message = "Insertion failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting department details.");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        [HttpPost("UpdateDepartmentDetails")]
        public async Task<IActionResult> UpdateDepartmentDetails(
         [FromQuery] string DeptGuid,
         [FromQuery] string DepartmentCode,
         [FromQuery] string DepartmentDescription,
         [FromQuery] long? ReferenceID,
         [FromQuery] string? ColourCode,
         [FromBody] List<DepartmentDetail> lstDepartmentDetail,
         [FromQuery] string? LevelDetailGUID = "067427BF-2613-4C17-89DB-B1D00704AD15")
        {
            try
            {
                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
                if (String.IsNullOrEmpty(strLevelDetailGUID))
                {
                    strLevelDetailGUID = LevelDetailGUID;
                }
                var departmentInput = new DepartmentInput
                {
                    Mode = "EDIT",
                    DeptCode = DepartmentCode,
                    DeptGUID = DeptGuid,
                    DeptDesc = DepartmentDescription,
                    ReferenceID = ReferenceID,
                    ColourCode = ColourCode,
                    LevelDetailGUID = strLevelDetailGUID,
                    LevelGUID = strLevelGUID,
                    UpdatedGuidBy = struserGuid,
                    lstDepart = lstDepartmentDetail,
                    TimeZoneID = string.IsNullOrEmpty(strTimeZoneID) ? 0 : Convert.ToInt32(strTimeZoneID)
                };

                var result = await _repository.DepartmentDALRepo.UpdateDepartmentDetails(departmentInput);
                return result == "1"
                 ? Ok(new { Message = "Department details update successfully." })
                 : BadRequest(new { Message = "Insertion failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting department details.");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


    [HttpPost("DeleteDepartmentDetails")]
    public async Task<IActionResult> DeleteDepartmentDetails(
    [FromBody] List<DepartmentDetail> lstDepartmentDetail,
    [FromQuery] string? LevelDetailGUID = "067427BF-2613-4C17-89DB-B1D00704AD15")
    {
        try
        {

                string struserGuid = string.Empty;
                string strLevelDetailGUID = string.Empty;
                string strLevelGUID = string.Empty;
                string strTimeZoneID = string.Empty;

                if (HttpContext?.Session != null)
                {
                    struserGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid) ?? string.Empty;
                    strLevelDetailGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelDetailGUID) ?? string.Empty;
                    strLevelGUID = HttpContext.Session.GetString(Common.SessionVariables.LevelGUID) ?? string.Empty;
                    strTimeZoneID = HttpContext.Session.GetString(Common.SessionVariables.TimeZoneID) ?? string.Empty;
                }
                if (String.IsNullOrEmpty(strLevelDetailGUID))
                {
                    strLevelDetailGUID = LevelDetailGUID;
                }
                var departmentInput = new DepartmentInput
            {
                LevelDetailGUID = strLevelDetailGUID,
                LevelGUID = strLevelGUID,
                UpdatedGuidBy = userGuid,
                lstDepart = lstDepartmentDetail,
                TimeZoneID = string.IsNullOrEmpty(strTimeZoneID) ? 0 : Convert.ToInt32(strTimeZoneID),
                Mode = "DELETE"
            };

            var result = await _repository.DepartmentDALRepo.DeleteDepartmentDetails(departmentInput);

            // Convert to ArrayList for flexibility
            ArrayList resultList = new ArrayList(result);

            switch (resultList.Count)
            {
                case > 0:
                    return Ok(new { Message = "Department details deleted successfully.", Details = result });
                default:
                    return BadRequest(new { Message = "Deletion failed." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting department details.");
            return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
        }
    }



}
}
