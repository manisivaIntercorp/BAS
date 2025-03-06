using Azure.Core;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using WebApi.Services;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ApiBaseController
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;
        private SessionService _sessionService;
        private GUID _guid;
        
        public RoleController(ILogger<RoleController> logger, IConfiguration configuration,IHttpContextAccessor httpContextAccessor, SessionService sessionService, GUID guid,IAuditLogService auditLogService) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _guid = guid;
            _auditLogService = auditLogService;
           
        }
        [HttpGet("getAllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                using (IUowRole _repo = new UowRole(_httpContextAccessor))
                {
                    //string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    //if (!string.IsNullOrEmpty(response))
                    //{
                        await _auditLogService.LogAction("", "getAllRole", "");
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        var lstRoleModel = await _repo.RoleDALRepo.GetAllRole(userId);
                        if (lstRoleModel != null && lstRoleModel.Count>0)
                        {
                            return Ok(lstRoleModel);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
                        }
                    //}
                    //else
                    //{
                    //    return BadRequest(Common.Messages.Login);
                    //}
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
       
        // This Method while Showing the Module Records Based UserID
        [HttpGet("getModulesBasedOnRole/{RoleGUID}/{IsPayrollAccessible}")]
        public async Task<IActionResult> getModulesBasedOnRole(string RoleGUID,string IsPayrollAccessible)
        {
            try
            {
                using (IUowRole _repo = new UowRole(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    //string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    //if (!string.IsNullOrEmpty(response))
                    //{
                        await _auditLogService.LogAction("", "getModulesBasedOnRole", "");
                        string? guidresp = await _guid.GetGUIDBasedOnUserRoleGuid(RoleGUID);
                        if (guidresp == RoleGUID)
                        {
                            var objRoleModel = await _repo.RoleDALRepo.getModulesBasedOnRole(RoleGUID, IsPayrollAccessible, userId);
                            if (objRoleModel.ModuleDatatable == null || objRoleModel.ModuleDatatable != null)
                            {
                                var responseUpdate = new GetRoleUpdateRequest
                                {
                                    RoleModel = objRoleModel.rolemodel,
                                    ModuleDatatable = objRoleModel.ModuleDatatable,

                                };
                                return Ok(responseUpdate);
                            }
                            else
                            {
                                return BadRequest(Common.Messages.NoRecordsFound);
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Role Guid");
                        }
                    //}
                    //else
                    //{
                    //    return BadRequest(Common.Messages.Login);
                    //}
                    
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        
        [HttpPost("InsertRole")]
        public async Task<IActionResult> InsertRole(RoleUpdateRequest objModel)
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
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            objModel.RoleModel.CreatedBy = userId;
                            await _auditLogService.LogAction("", "InsertRole", "");
                            DataTable dataTable = objModel.RoleModel.ConvertToDataTable(objModel.ModuleDatatable);
                            var result = await _repo.RoleDALRepo.InsertUpdateRole(objModel.RoleModel);
                            var msg = "Role Inserted Successfully";
                            _repo.Commit();
                            if (result.roleModels != null)
                            {
                                switch (result.RetVal)
                                {
                                    case >= 1://Success
                                        responsemsg = msg;
                                        break;
                                    case -1:
                                        responsemsg = result.Msg ?? string.Empty;
                                        break;
                                    case 0:
                                        responsemsg = result.Msg ?? string.Empty;
                                        break;
                                    default:
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateRole function in Role api controller");
                                        return BadRequest();
                                }

                            }
                            else {
                                return BadRequest(Common.Messages.Login);
                            }
                        }
                    }
                    return Ok(responsemsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        // Update User Based on UserID
        [HttpPut("EditUpdateUserRole")]
        public async Task<IActionResult> EditUpdateUserRole([FromBody] GetRoleModel roleModel)
        {
            if (roleModel == null)
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
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            await _auditLogService.LogAction("", "EditUpdateUserRole", "");
                            string guidResp = await _guid.GetGUIDBasedOnUserRoleGuid(roleModel.RoleGuid);
                            if(roleModel.RoleGuid== guidResp)
                            {
                                var result = await _repo.RoleDALRepo.EditUpdateRoleAsync(roleModel);
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
                                            responsemsg = result.Msg ?? string.Empty;
                                            break;

                                        default:
                                            _logger.LogError(Environment.NewLine);
                                            _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
                                            return BadRequest();

                                    }
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check Role Guid");
                            }
                        }
                        else
                        {
                            return BadRequest(Common.Messages.Login);
                        }
                        return Ok(responsemsg);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole(RoleInsertUpdateRequest objModel)
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
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        //if (!string.IsNullOrEmpty(response))
                        //{
                            objModel.RoleModel.CreatedBy = userId;
                            await _auditLogService.LogAction("", "UpdateRole", "");
                            string guidResp = await _guid.GetGUIDBasedOnUserRoleGuid(objModel.RoleModel.RoleGuid);
                            if (objModel.RoleModel.RoleGuid == guidResp)
                            {
                                DataTable dataTable = objModel.RoleModel.ConvertToDataTable(objModel.ModuleDatatable);
                                var result = await _repo.RoleDALRepo.UpdateRole(objModel.RoleModel);
                                var msg = "Role Updated Successfully";
                                _repo.Commit();
                                if (result.roleModels != null)
                                {
                                    switch (result.RetVal)
                                    {
                                        case >= 1://Success
                                            responsemsg = msg;
                                            break;
                                        case -1:
                                            responsemsg = result.Msg ?? string.Empty;
                                            break;
                                        case 0:
                                            responsemsg = result.Msg ?? string.Empty;
                                            break;
                                        default:
                                            _logger.LogError(Environment.NewLine);
                                            _logger.LogError("Bad Request occurred while accessing the InsertUpdateRole function in Role api controller");
                                            return BadRequest();
                                    }
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check Role Guid");
                            }
                        //}
                        //else
                        //{
                        //    return BadRequest(Common.Messages.Login);
                        //}
                    }
                    return Ok(responsemsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpDelete("deleteRole")]
        
        public async Task<IActionResult> DeleteRole(RolesDelete RoleDelete)
        {
            try
            {
                using (IUowRole _repo = new UowRole(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    //if (!string.IsNullOrEmpty(response))
                    //{
                        await _auditLogService.LogAction("", "deleteRole", "");
                        foreach (var guid in RoleDelete.DeleteRoleNames)
                        {
                            string? GuidResp = await _guid.GetGUIDBasedOnUserRoleGuid(guid.RoleGUID);
                            if (GuidResp == guid.RoleGUID)
                            {
                                DataTable? deleteTable = RoleDelete?.ConvertToDataTable(RoleDelete.DeleteRoleNames ?? new List<RolesDeleteInList>());
                                var result = await _repo.RoleDALRepo.DeleteRole(RoleDelete, userId);
                                _repo.Commit();
                                if (result.DeleteRole == true || result.DeleteRole == false)
                                {
                                    return Ok(result.deleteRoleInformation);
                                }
                                else
                                {
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the DeleteRole function in Role api controller");
                                    return BadRequest();
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check Role GUID");
                            }
                        }
                    //}
                    //else
                    //{
                    //    return BadRequest(Common.Messages.Login);
                    //}
                    return Ok();
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
