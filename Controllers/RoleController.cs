using Azure.Core;
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
    public class RoleController : ApiBaseController
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RoleController(ILogger<RoleController> logger, IConfiguration configuration,IHttpContextAccessor httpContextAccessor) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            //var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            //NLog.GlobalDiagnosticsContext.Set("LoggingDirectory", logPath);
        }
        [HttpGet("getAllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                using (IUowRole _repo = new UowRole(_httpContextAccessor))
                {
                    var lstRoleModel = await _repo.RoleDALRepo.GetAllRole();
                    if (lstRoleModel != null)
                    {
                        return Ok(lstRoleModel);
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
       
        // This Method while Showing the Module Records Based UserID
        [HttpGet("getModulesBasedonRole/{Roleid}/{IsPayrollAccessible}/{UserID}")]
        public async Task<IActionResult> getModulesBasedonRole(long Roleid,string IsPayrollAccessible, long UserID )
        {
            try
            {
                using (IUowRole _repo = new UowRole(_httpContextAccessor))
                {
                    var objRoleModel = await _repo.RoleDALRepo.getModulesBasedonRole(Roleid, IsPayrollAccessible, UserID);
                    if (objRoleModel.ModuleDatatable == null || objRoleModel.ModuleDatatable != null)
                    {
                        var response = new GetRoleUpdateRequest
                        {
                            RoleModel = objRoleModel.rolemodel,
                            ModuleDatatable = objRoleModel.ModuleDatatable,

                        };
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest("No Records Found");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        
        [HttpPost("InsertUpdateRole")]
        public async Task<IActionResult> InsertUpdateRole(RoleUpdateRequest objModel)
        {
            try
            {
                string responsemsg = string.Empty;
                if (objModel == null || objModel.RoleModel == null || objModel.ModuleDatatable == null)
                {
                    return BadRequest("Invalid input data.");
                }

                else
                {
                    using (IUowRole _repo = new UowRole(_httpContextAccessor))
                    {
                        DataTable dataTable = objModel.RoleModel.ConvertToDataTable(objModel.ModuleDatatable);
                        var result = await _repo.RoleDALRepo.InsertUpdateRole(objModel.RoleModel);
                        var msg = "Role Inserted Successfully";
                        _repo.Commit();
                        if (result.roleModels!=null)
                        {
                            switch (result.RetVal)
                            {
                                case >= 1://Success
                                    responsemsg = msg;
                                    break;
                                case -1:
                                    responsemsg = result.Msg??string.Empty;
                                    break;
                                case 0:
                                    responsemsg = result.Msg??string.Empty;
                                    break;
                                default:
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateRole function in Role api controller");
                                    return BadRequest();
                            }
                            
                        }
                    }
                }
                return Ok(responsemsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        // Update User Based on UserID
        [HttpPut("updateUserRole/{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] RoleModel roleModel)
        {
            if (roleModel == null || id != roleModel.RoleId)
            {
                return BadRequest("Invalid input data.");
            }

            else
            {
                try
                {
                    string responsemsg = string.Empty;
                    using (IUowRole _repo = new UowRole(_httpContextAccessor))
                    {
                        
                        var result = await _repo.RoleDALRepo.UpdateRoleAsync(id, roleModel);
                        var msg = "Role updated successfully.";
                        _repo.Commit();
                        if (result.roleModels != null)
                        {
                            switch (result.RetVal)
                            {
                                case >= 1:
                                    responsemsg = msg;
                                    break;

                                case -1:
                                    responsemsg=result.Msg??string.Empty;
                                    break;

                                default:
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
                                    return BadRequest();
                                }
                            
                        }
                        
                        
                    }
                    return Ok(responsemsg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }
        [HttpGet("deleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                using (IUowRole _repo = new UowRole(_httpContextAccessor))
                {
                    var result = await _repo.RoleDALRepo.DeleteRole(id);
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the DeleteRole function in Role api controller");
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
