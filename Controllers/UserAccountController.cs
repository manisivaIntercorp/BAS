using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services;
using DataAccessLayer.Services;
using WebApi.Services.Interface;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ApiBaseController
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly EmailServices _emailService;
        private SessionService _sessionService;
        private readonly IAuditLogService _auditLogService;
        private GUID _guid;


        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAccountController(EmailServices emailServices, ILogger<UserAccountController> logger, IHttpContextAccessor httpContextAccessor,IConfiguration configuration, SessionService sessionService, GUID guid, IAuditLogService auditLogService) : base(configuration)
        {
            _emailService = emailServices;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _guid = guid;
            _auditLogService = auditLogService;
        }
        //List Page for User Creation
        [HttpGet("getAllUserAccount")]
        public async Task<IActionResult> GetAllUserAccount()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllUserAccount", "");
                        var lstUserAccountModel = await _repo.UserAccountDALRepo.GetAllUserAccount();
                        if (lstUserAccountModel != null && lstUserAccountModel.Count > 0)
                        {
                            return Ok(lstUserAccountModel);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
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

        //Get User Policy in DropDown
        [HttpGet("getAllUserPolicyInDropdown")]
        public async Task<IActionResult> getAllUserPolicyInDropdown()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllUserPolicyInDropdown", "");
                        var lstUserPolicyModel = await _repo.UserAccountDALRepo.getAllUserPolicyInDropdown();
                        if (lstUserPolicyModel != null && lstUserPolicyModel.Count>0)
                        {
                            return Ok(lstUserPolicyModel);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
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

        //Get User Language in Dropdown
        [HttpGet("getAllUserLanguageInDropdown")]
        public async Task<IActionResult> getAllUserLanguageInDropdown()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllUserLanguageInDropdown", "");
                        var lstUserLanguageModel = await _repo.UserAccountDALRepo.getAllUserLanguageInDropdown();
                        if (lstUserLanguageModel != null && lstUserLanguageModel.Count > 0)
                        {
                            return Ok(lstUserLanguageModel);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
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
        //Get User Language in Dropdown
        [HttpGet("getAllUserTimeZoneInDropdown")]
        public async Task<IActionResult> getAllUserTimeZoneInDropdown()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllUserTimeZoneInDropdown", "");
                        var lstUserLanguageModel = await _repo.UserAccountDALRepo.getAllUserTimeZoneInDropdown();
                        if (lstUserLanguageModel != null && lstUserLanguageModel.Count > 0)
                        {
                            return Ok(lstUserLanguageModel);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
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
        // Get User Role in Dropdown
        [HttpGet("getAllUserRoleInDropdown")]
        public async Task<IActionResult> getAllUserRoleInDropdown()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    string response =  _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllUserRoleInDropdown", "");
                        var lstUserPolicyModel = await _repo.UserAccountDALRepo.getAllUserRoleInDropdown();
                        if (lstUserPolicyModel != null && lstUserPolicyModel.Count>0)
                        {
                            return Ok(lstUserPolicyModel);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
                        }
                    }

                    else { return BadRequest(Common.Messages.Login); }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        //Getting Specific User ID from User Account Creation
        [HttpGet("GetUserAccountByGUId/{guid}")]

        public async Task<IActionResult> GetUserAccountByGUId(string guid)
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    string responseGUId = _sessionService.GetSession(Common.SessionVariables.Guid);

                    if (!string.IsNullOrEmpty(responseGUId))
                    {
                        await _auditLogService.LogAction("", "GetUserAccountByGUId", "");
                        string? guidresp = await _guid.GetGUIDBasedOnUserGuid(guid);
                        if (guid.Equals(guidresp))
                        {
                            var objuseraccountModel = await _repo.UserAccountDALRepo.GetUserAccountByGUId(guid);
                            var response = new UserAccountResponse();
                            if (objuseraccountModel.userAccounts != null || objuseraccountModel.UserRoles != null || objuseraccountModel.Org != null)
                            {
                                if (objuseraccountModel.userAccounts?.UserID is not null and 0)
                                {
                                    // Return 204 No Content
                                    return NoContent();
                                }
                                if (objuseraccountModel.userAccounts?.UserID != null && objuseraccountModel.userAccounts.UserID != 0)
                                {
                                    response = new UserAccountResponse
                                    {
                                        User = objuseraccountModel.userAccounts,
                                        Roles = objuseraccountModel.UserRoles,
                                        Organizations = objuseraccountModel.Org
                                    };

                                }
                                return Ok(response);

                            }

                        }
                        else
                        {
                            return BadRequest("Please Check GUID");
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        // Displaying Organization Details Based on User ID
        [HttpGet("getAllOrgDetails")]
        public async Task<IActionResult> getOrgDetailsByUserGUId()
        {
            try
            {

                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllOrgDetails", "");
                        var objuseraccountModel = await _repo.UserAccountDALRepo.GetOrgDetailsByUserGUId();
                        if (objuseraccountModel != null && objuseraccountModel.Count > 0)
                        {
                            return Ok(objuseraccountModel);
                        }
                        else
                        {
                            return BadRequest(Common.Messages.NoRecordsFound);
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

        //Insert New User Account Creation
        [HttpPost("insertUserAccount")]
        public async Task<IActionResult> InsertUserAccount(UserAccountInsertRequest objModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                string responseMsg = string.Empty;
                if (objModel == null || objModel.UserAccount == null || objModel.OrgDataTable == null || objModel.RoleNameList == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else if (objModel?.UserAccount?.Active != "Y" && objModel?.UserAccount?.Active != "N")
                {
                    return BadRequest("UserAccount Active  invalid. Only 'Y' or 'N' are allowed.");
                }
                foreach (var org in objModel.OrgDataTable)
                {
                    // Check if ActiveOrg is neither "Y" nor "N" and is not null
                    var activeOrg = org?.ActiveOrg;
                    if (!string.IsNullOrEmpty(activeOrg) && activeOrg != "Y" && activeOrg != "N")
                    {
                        return BadRequest("OrgDataTable ActiveOrg is invalid. Only 'Y' or 'N' are allowed.");
                    }
                }

                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "InsertUserAccount", "");
                        foreach (var GuidOrg in objModel.OrgDataTable)
                        {
                            if (GuidOrg?.OrgName != "string" || GuidOrg?.OrgName == "string")
                            {
                                string OrgGuid = await _guid.GetGUIDBasedOnOrgName(GuidOrg?.OrgName);
                                if (OrgGuid == GuidOrg?.OrgName)
                                {
                                    DataTable dataTable = objModel.UserAccount.ConvertToDataTable(objModel.OrgDataTable, 0);
                                    DataTable dataTableRole = objModel.UserAccount.ConvertToDataTable(objModel.RoleNameList, userId, 0);

                                    var insertCheckResult= await _repo.UserAccountDALRepo.InsertCheckUserAccount(objModel.UserAccount);
                                    
                                    if(insertCheckResult.InsertedUsers != null)
                                    {
                                        switch (insertCheckResult.RetVal) {
                                            case 0:// Success
                                                var result = await _repo.UserAccountDALRepo.InsertUpdateUserAccount(objModel.UserAccount);
                                                _repo.Commit();

                                                if (result.InsertedUsers != null || result.OrgDetails == null || result.OrgDetails != null)
                                                {
                                                    if (!string.IsNullOrEmpty(GuidOrg?.OrgName) && GuidOrg?.OrgName != "string")
                                                    {
                                                        if ((result.InsertedUsers != null && result.InsertedUsers.Count > 0) &&
                                                             (result.OrgDetails != null && result.OrgDetails.Count > 0))
                                                        {
                                                            switch (result.RetVal)
                                                            {
                                                                case 0:
                                                                case -1://Already Exists
                                                                    responseMsg = result.Msg ?? string.Empty;

                                                                    break;

                                                                case >= 1:
                                                                    responseMsg = result.Msg ?? string.Empty;
                                                                    await _emailService.SendMailMessage(EmailTemplateCode.USER_ACCOUNT_CREATED, -1,
                                                                                                        result.RetVal,
                                                                                                        objModel.UserAccount.UserPassword);

                                                                    break;

                                                                default:
                                                                    _logger.LogError(Environment.NewLine);
                                                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
                                                                    return NotFound("User Account Already Exists" + BadRequest());
                                                            }

                                                        }
                                                        else
                                                        {
                                                            _logger.LogError("Organization not found: " + (result.OrgDetails?.FirstOrDefault()?.OrgName ?? "Unknown Org"));
                                                            return BadRequest("Please Check Organization Name");
                                                        }
                                                    }
                                                    else if ((!string.IsNullOrEmpty(GuidOrg?.OrgName) && GuidOrg?.OrgName == "string") || (string.IsNullOrEmpty(GuidOrg?.OrgName) && GuidOrg?.OrgName == "string"))
                                                    {
                                                        if ((result.InsertedUsers != null && result.InsertedUsers.Count > 0) ||
                                                       (result.OrgDetails != null && result.OrgDetails.Count == 0))
                                                        {
                                                            switch (result.RetVal)
                                                            {
                                                                case 0:
                                                                case -1://Already Exists
                                                                    responseMsg = result.Msg ?? string.Empty;

                                                                    break;

                                                                case >= 1:
                                                                    responseMsg = result.Msg ?? string.Empty;
                                                                    await _emailService.SendMailMessage(EmailTemplateCode.USER_ACCOUNT_CREATED, -1,
                                                                                                        result.RetVal,
                                                                                                        objModel.UserAccount.UserPassword);

                                                                    break;

                                                                default:
                                                                    _logger.LogError(Environment.NewLine);
                                                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
                                                                    return NotFound("User Account Already Exists" + BadRequest());
                                                            }

                                                        }
                                                        else
                                                        {
                                                            _logger.LogError("Organization not found: " + (result.OrgDetails?.FirstOrDefault()?.OrgName ?? "Unknown Org"));
                                                            return BadRequest("Please Check Organization Name");
                                                        }
                                                    }



                                                }
                                                break;
                                            case 1:// Already Exists
                                                _logger.LogError("UserName Already exists: " + (objModel.UserAccount.UserName ?? "Already Exists"));
                                                return BadRequest("UserName " + (objModel.UserAccount.UserName ?? "Already Exists") +" Already Exists");
                                                
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    return BadRequest($"Please Check Org Name {GuidOrg?.OrgName}");
                                }
                            }
                            if(GuidOrg?.OrgName == "")
                            {
                                return BadRequest("OrgName should not empty");
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }
                    return Ok(responseMsg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        // Edit Update User Account
        [HttpPut("EditUpdateUserAccount")]
        public async Task<IActionResult> EditUpdateUserAccount([FromBody] UpdateUserAccountModel userAccount)
        {
            if (userAccount == null)
            {
                return BadRequest("Invalid input data.");
            }
            else if (userAccount?.Active != "Y" && userAccount?.Active != "N")
            {
                return BadRequest("Active  invalid. Only 'Y' or 'N' are allowed.");
            }
            else
            {
                try
                {
                    string responseMsg = string.Empty;
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        userAccount.CreatedBy = userId;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);

                        if (!string.IsNullOrEmpty(response))
                        {
                            await _auditLogService.LogAction("", "EditUpdateUserAccount", "");
                            string? guidresp = await _guid.GetGUIDBasedOnUserGuid(userAccount.MasterGuid);
                            if (userAccount.MasterGuid == guidresp)
                            {
                                var result = await _repo.UserAccountDALRepo.EditUpdateUserAccountAsync(userAccount);
                                _repo.Commit();
                                if (result.updateuseraccount != null)
                                {
                                    switch (result.RetVal)
                                    {
                                        case >= 1:
                                            responseMsg = result.Msg ?? string.Empty;
                                            break;
                                        case -1:
                                            return Ok(result.Msg);
                                        default:
                                            _logger.LogError(Environment.NewLine);
                                            _logger.LogError("Bad Request occurred while accessing the EditupdateUserAccount function in User Account api controller");
                                            return BadRequest();
                                    }

                                }
                                else
                                {
                                    return BadRequest("Please Check the Master Guid");
                                }
                            }
                            else
                            {
                                return BadRequest(Common.Messages.Login);
                            }
                        }
                        return Ok(responseMsg);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }
        // Update User Account
        [HttpPut("updateUserAccount")]
        public async Task<IActionResult> UpdateUserAccount([FromBody] UserAccountUpdateRequest userAccount)
        {
            if (userAccount == null)
            {
                return BadRequest("Invalid input data.");
            }
            else if (userAccount?.UserAccount?.Active != "Y" && userAccount?.UserAccount?.Active != "N")
            {
                return BadRequest("Active  invalid. Only 'Y' or 'N' are allowed.");
            }

            else
            {
                try
                {
                    foreach (var org in userAccount.OrgDataTable)
                    {
                        // Check if ActiveOrg is neither "Y" nor "N" and is not null
                        var activeOrg = org?.ActiveOrg;
                        if (!string.IsNullOrEmpty(activeOrg) && activeOrg != "Y" && activeOrg != "N")
                        {
                            return BadRequest("OrgDataTable ActiveOrg is invalid. Only 'Y' or 'N' are allowed.");
                        }
                    }
                    string responseMsg = string.Empty;
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {
                        string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        userAccount.UserAccount.CreatedBy = userId;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            await _auditLogService.LogAction("", "updateUserAccount", "");
                            string? guidresp = await _guid.GetGUIDBasedOnUserGuid(userAccount.UserAccount.MasterGuid);
                            if (userAccount.UserAccount.MasterGuid == guidresp)
                            {
                                foreach (var org in userAccount.OrgDataTable)
                                {
                                    if(org?.OrgName!="string" || org?.OrgName=="string")
                                    {
                                        DataTable dataTable = userAccount.UserAccount.ConvertToDataTable(userAccount.OrgDataTable, userAccount.UserAccount.MasterGuid);
                                        DataTable dataTableRole = userAccount.UserAccount.ConvertToDataTable(userAccount.RoleNameList, userId, userAccount.UserAccount.MasterGuid);
                                        var result = await _repo.UserAccountDALRepo.UpdateUserAccountAsync(userAccount?.UserAccount);
                                        _repo.Commit();
                                        if (result.updateuseraccount != null || result.OrgDetails == null || result.OrgDetails != null)
                                        {
                                            if (!string.IsNullOrEmpty(org?.OrgName) && org?.OrgName != "string")
                                            {
                                                if ((result.updateuseraccount != null && result.updateuseraccount.Count > 0) &&
                                                    (result.OrgDetails != null && result.OrgDetails.Count > 0))
                                                {
                                                    switch (result.RetVal)
                                                    {
                                                        case 0:
                                                        case -1://Already Exists
                                                            responseMsg = result.Msg ?? string.Empty;
                                                            break;

                                                        case >= 1:
                                                            responseMsg = result.Msg ?? string.Empty;

                                                            break;

                                                        default:
                                                            _logger.LogError(Environment.NewLine);
                                                            _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
                                                            return NotFound("User Account Already Exists" + BadRequest());
                                                    }
                                                }
                                                else
                                                {
                                                    _logger.LogError("Organization not found: " + (result.OrgDetails?.FirstOrDefault()?.OrgName ?? "Unknown Org"));
                                                    return BadRequest("Please Check Organization Name");
                                                }

                                            }
                                            else if (!string.IsNullOrEmpty(org?.OrgName) && org?.OrgName == "string")
                                            {
                                                if ((result.updateuseraccount != null && result.updateuseraccount.Count > 0) ||
                                               (result.OrgDetails != null && result.OrgDetails.Count == 0))
                                                {
                                                    switch (result.RetVal)
                                                    {
                                                        case 0:
                                                        case -1://Already Exists
                                                            responseMsg = result.Msg ?? string.Empty;

                                                            break;

                                                        case >= 1:
                                                            responseMsg = result.Msg ?? string.Empty;
                                                            break;

                                                        default:
                                                            _logger.LogError(Environment.NewLine);
                                                            _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
                                                            return NotFound("User Account Already Exists" + BadRequest());
                                                    }
                                                }
                                                else
                                                {
                                                    _logger.LogError("Organization not found: " + (result.OrgDetails?.FirstOrDefault()?.OrgName ?? "Unknown Org"));
                                                    return BadRequest("Please Check Organization Name");
                                                }
                                            }
                                        }

                                    }
                                    if(org?.OrgName == "")
                                   
                                    {
                                        return BadRequest("OrgName should not be empty");
                                        
                                    }
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check the Master GUID");
                            }
                                
                        }
                        else
                        {
                            return BadRequest(Common.Messages.Login);
                        }
                    }
                    return Ok(responseMsg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }
        // Delete User Account
        [HttpDelete("deleteUserAccount")]
        public async Task<IActionResult> DeleteUserAccount(DeleteUserAccount deleteUserAccount)
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "deleteUserAccount", "");
                        foreach (var UserGuid in deleteUserAccount.DeleteDataTable)
                        {
                            var GuidResp = await _guid.GetGUIDBasedOnUserGuid(UserGuid.UserGUID);
                            if (GuidResp == UserGuid.UserGUID)
                            {
                                var dataTable = deleteUserAccount.ConvertToDataTable(deleteUserAccount.DeleteDataTable);
                                var result = await _repo.UserAccountDALRepo.DeleteUserAccount(Convert.ToInt64(userId), deleteUserAccount);

                                _repo.Commit();
                                if (result.deleteuseraccount == true || result.deleteuseraccount == false)
                                {
                                    if (result.deleteResults.Count > 0)
                                    {
                                        return Ok(result.deleteResults);
                                    }
                                }
                                else
                                {
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the DeleteUserAccount function in User Account api controller");
                                    return BadRequest();
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check User Guid");
                            }
                        }
                    }


                    else
                    {
                        return BadRequest(Common.Messages.Login);
                    }

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpPost("AddRoleInUserAccount")]
        public async Task<IActionResult> AddRoleInUserAccount(RoleName objModel)
        {
            try
            {
                string responseMsg = string.Empty;
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {
                        string? userIdStr = _httpContextAccessor?.HttpContext?.Session?.GetString("strUserID");
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        objModel.CreatedBy = userId;
                        
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if(!string.IsNullOrEmpty(response))
                        {
                            await _auditLogService.LogAction("", "AddRoleInUserAccount", "");
                            string? GuidResp = await _guid.GetGUIDBasedOnUserRoleGuid(objModel.RoleGUID);
                            string? GuidUser = await _guid.GetGUIDBasedOnUserGuid(objModel.UserGUID);
                            if (GuidResp == objModel.RoleGUID && GuidUser == objModel.UserGUID)
                            {
                                var result = await _repo.UserAccountDALRepo.AddRoleName(objModel);
                                _repo.Commit();
                                if (result.Roles != null)
                                {
                                    switch (result.RetVal)
                                    {
                                        case 0:// Already Exists
                                            responseMsg = result.Msg ?? string.Empty;
                                            break;

                                        case -4:// Null Value Return
                                            responseMsg = result.Msg ?? string.Empty;
                                            break;
                                        case 1://Success
                                            responseMsg = result.Msg ?? string.Empty;

                                            break;

                                        case -1:// Effective Date Should not be greater than current date
                                            responseMsg = result.Msg ?? string.Empty;
                                            break;

                                        case -2:
                                            responseMsg = result.Msg ?? string.Empty;
                                            break;
                                        case -3: // There cannot be more than one role for the high level user!
                                            responseMsg = result.Msg ?? string.Empty;
                                            break;

                                        default:
                                            _logger.LogError(Environment.NewLine);
                                            _logger.LogError("Bad Request occurred while accessing the AddRoleInUserAccount function in User Account api controller");
                                            return BadRequest();

                                    }
                                }

                            }
                            else if (GuidResp == objModel.RoleGUID && GuidUser != objModel.UserGUID)
                            {
                                return BadRequest("Please Check User GUID");
                            }
                            else
                            {
                                return BadRequest("Please Check Role GUID");
                            }
                        }
                        else { return  BadRequest("Try to Login"); }
                        
                    }
                }
                return Ok(responseMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpDelete("DeleteRoleInUserAccount")]
        public async Task<IActionResult> DeleteRoleInUserAccount(DeleteRoleName objModel)
        {
            try
            {
                string responseMsg = string.Empty;
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {

                        string? userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        objModel.CreatedBy = userId;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            await _auditLogService.LogAction("", "DeleteRoleInUserAccount", "");
                            foreach (var guid in objModel.DeleteRoleNames)
                            {
                                string? GuidResp = await _guid.GetGUIDBasedOnUserAccountRoleGuid(guid.UserAccountRoleGuid);
                                if (GuidResp == guid.UserAccountRoleGuid)
                                {
                                    DataTable? deleteTable = objModel?.ConvertToDataTable(objModel.DeleteRoleNames ?? new List<DeleteRoleNameInList>());
                                    var result = await _repo.UserAccountDALRepo.DeleteRoleInUserAccount(objModel ?? new DeleteRoleName());

                                    var msg = "Role Deleted Successfully";
                                    _repo.Commit();
                                    if (result.deleteroles != null)
                                    {
                                        switch (result.RetVal)
                                        {
                                            case 1://Success
                                                responseMsg = msg;
                                                break;
                                            case -1:// Failure
                                                responseMsg = result.Msg ?? string.Empty;
                                                break;
                                            case -4:// Null Value Return
                                                responseMsg = result.Msg ?? string.Empty;
                                                break;
                                            default:
                                                _logger.LogError(Environment.NewLine);
                                                _logger.LogError("Bad Request occurred while accessing the DeleteRoleinUserAccount function in User Account api controller");
                                                return NotFound(BadRequest());
                                        }

                                    }
                                }
                                else
                                {
                                    return BadRequest("Please Check Guid");
                                }
                            }
                        }
                        else
                        {
                            return BadRequest(Common.Messages.Login);
                        }

                    }
                }
                return Ok(responseMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        // Reset Password in User Account
        [HttpPost("ResetPasswordInUserAccount")]
        public async Task<IActionResult> ResetPasswordInUserAccount(ResetPassword objModel)
        {
            try
            {
                string responseMsg = string.Empty;
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {
                        string? userIdStr = _httpContextAccessor?.HttpContext?.Session?.GetString("strUserID");
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            objModel.CreatedBy = userId;
                            await _auditLogService.LogAction("", "ResetPasswordInUserAccount", "");
                            var result = await _repo.UserAccountDALRepo.ResetPasswordInUserAccount(objModel);
                            _repo.Commit();
                            if (result.PasswordReset != null)
                            {
                                switch (result.RetVal)
                                {
                                    case -4:// Null Value Return
                                        responseMsg = result.Msg ?? string.Empty;
                                        break;
                                    case 1:// Success
                                        responseMsg = result.Msg ?? string.Empty;

                                        await _emailService.SendMailMessage(EmailTemplateCode.RESET_PASSWORD,
                                            -1,
                                            userId,
                                            objModel.Password);
                                        break;

                                    case -1://
                                        responseMsg = result.Msg ?? string.Empty;
                                        break;
                                    default:
                                        _logger.LogError(Environment.NewLine);
                                        _logger.LogError("Bad Request occurred while accessing the AddRoleInUserAccount function in User Account api controller");
                                        return BadRequest();
                                }
                            }
                        }
                        else
                        {
                            return BadRequest(Common.Messages.Login);
                        }
                    }
                }
                return Ok(responseMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        // Unlock User
        [HttpPut("UnlockUser")]
        public async Task<IActionResult> UnlockUser([FromBody] UnlockUser? listOfUnlock)
        {
            if (listOfUnlock == null)
            {
                return BadRequest("Invalid input data.");
            }

            else
            {
                try
                {
                    var responseMsg = string.Empty;
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {
                        string? userIdStr = _httpContextAccessor?.HttpContext?.Session?.GetString("strUserID");
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                        if (!string.IsNullOrEmpty(response))
                        {
                            await _auditLogService.LogAction("", "UnlockUser", "");
                            foreach (var UserGuid in listOfUnlock.Users ?? new List<UnlockUserList?>())
                            {
                                string? guidUser = await _guid.GetGUIDBasedOnUserGuid(UserGuid?.UserGuId);
                                if(guidUser == UserGuid?.UserGuId)
                                {
                                    DataTable? dataTable = listOfUnlock?.ConvertToDataTable(listOfUnlock.Users ?? new List<UnlockUserList?>()) ?? new DataTable();
                                    listOfUnlock.UpdatedBy = userId;
                                    var result = await _repo.UserAccountDALRepo.UnlockUserAsync(listOfUnlock);
                                    var msg = "Unlock User successfully.";
                                    _repo.Commit();
                                    if (result.unlockuser != null)
                                    {
                                        switch (result.RetVal)
                                        {
                                            case 1:// Success
                                                responseMsg = msg;
                                                break;

                                            case -1:// Failure or Not exists
                                                responseMsg = result.Msg;
                                                break;
                                            default:
                                                _logger.LogError(Environment.NewLine);
                                                _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
                                                return BadRequest();
                                        }

                                    }

                                }

                            }
                        }
                        else
                        {
                            return BadRequest(Common.Messages.Login);
                        }
                        return Ok(responseMsg);
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
}