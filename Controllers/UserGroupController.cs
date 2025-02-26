using Azure;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo;
using System.Diagnostics.Eventing.Reader;
using WebApi.Services;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ApiBaseController
    {
        private readonly ILogger<UserGroupController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private SessionService _sessionService;
        private GUID _guid;
        private readonly IAuditLogService _auditLogService;

        public UserGroupController(ILogger<UserGroupController> logger,IHttpContextAccessor httpContextAccessor ,IConfiguration configuration, SessionService sessionService,GUID gUID, IAuditLogService auditLogService) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _guid = gUID;
            _auditLogService = auditLogService;

        }
        [HttpGet("getAllUserPolicy")]
        public async Task<IActionResult> GetAllUserPolicy()
        {
            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    //if (!string.IsNullOrEmpty(response)) {
                        await _auditLogService.LogAction("", "getAllUserPolicy", "");
                        var lstUserGroupModel = await _repo.UserGroupDALRepo.GetAllUserPolicy();
                        if (lstUserGroupModel != null && lstUserGroupModel.Count>0)
                        {
                            return Ok(lstUserGroupModel);
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
        [HttpGet("getUserPolicyByGuId/{GUId}")]
        public async Task<IActionResult> GetUserGroupByGUId(string GUId)
        {
            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(_httpContextAccessor))
                {

                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response)) {
                        await _auditLogService.LogAction("", "getUserPolicyByGuId", "");
                        string GuidUserPolicy = await _guid.GetGUIDBasedOnUserPolicy(GUId);
                        if(GuidUserPolicy==GUId)
                        {
                            var objUserGroupModel = await _repo.UserGroupDALRepo.GetUserPolicyByGUId(GUId);
                            if (objUserGroupModel != null)
                            {
                                return Ok(objUserGroupModel);
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Guid");
                        }
                        
                    } 
                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpPost("insertUserPolicy")]
        public async Task<IActionResult> InsertUserGroup(UserGroupModel objModel)
        {
            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response)) 
                    {
                        await _auditLogService.LogAction("", "insertUserPolicy", "");
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        objModel.CreatedBy = userId;

                        var result = await _repo.UserGroupDALRepo.InsertUpdateUserPolicy(objModel);
                        var msg = "User Policy Inserted Successfully";
                        _repo.Commit();
                        if (result.InsertUserGroup==true || result.InsertUserGroup==false)
                        {
                            switch (result.RetVal)
                            {
                                case 1:// Success
                                    return Ok(msg);
                                case 0:// Exists
                                    return Ok(result.Msg);
                                default:
                                    
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserGroup function in User Group api controller");
                                        return NotFound("User Policy Already Exists");
                                }
                            }
                        
                    } 
                    else 
                    { 
                        return BadRequest(Common.Messages.Login);
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

        [HttpPut("updateUserPolicy")]
        public async Task<IActionResult> UpdateUserGroup( [FromBody] UpdateUserGroupModel UserGroup)
        {

            if (UserGroup == null)
            {
                return BadRequest(Common.Messages.InvalidData);
            }

            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "updateUserPolicy", "");
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string UserPolicyGuid = await _guid.GetGUIDBasedOnUserPolicy(UserGroup.UserPolicyGuid);
                        if (UserPolicyGuid == UserGroup.UserPolicyGuid)
                        {
                            UserGroup.CreatedBy = userId;
                            var result = await _repo.UserGroupDALRepo.UpdateUserPolicyAsync(UserGroup);
                            var msg = "User Group updated successfully.";
                            _repo.Commit();
                            if (result.UpdateUserGroup == true || result.UpdateUserGroup == false)
                            {
                                switch (result.RetVal)
                                {
                                    case 1:// Success
                                        return Ok(msg);
                                    case 0:// Exists
                                        return Ok(result.Msg);
                                    default:

                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserGroup function in User Group api controller");
                                        return NotFound("User Policy Already Exists");
                                }
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check UserPolicy GUID");
                        }
                       
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
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

        [HttpDelete("deleteUserPolicy")]
        public async Task<IActionResult> DeleteUserGroup(DeleteUserGroup deleteUserGroup)
        {
            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response)) {
                        await _auditLogService.LogAction("", "deleteUserPolicy", "");
                        foreach (var UserGuid in deleteUserGroup.DeleteDataTable)
                        {
                            var GuidResp = await _guid.GetGUIDBasedOnUserPolicy(UserGuid.UserPolicyGUID);
                            if (GuidResp == UserGuid.UserPolicyGUID) {
                                var dataTable = deleteUserGroup.ConvertToDataTable(deleteUserGroup.DeleteDataTable);
                                var result = await _repo.UserGroupDALRepo.DeleteUserPolicy(userId,deleteUserGroup);
                                _repo.Commit();
                                if (result.deleteuserGroup == true || result.deleteuserGroup == false)
                                {
                                    if (result.deleteResults.Count > 0)
                                    {
                                        return Ok(result.deleteResults);
                                    }
                                }
                                else
                                {
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the DeleteUserPolicy function in User Policy api controller");
                                    return BadRequest();
                                }
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
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
    }
}
