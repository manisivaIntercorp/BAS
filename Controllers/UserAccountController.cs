using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services;
using DataAccessLayer.Services;
using System;
using static WebApi.Services.Implementation.Common;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ApiBaseController
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly EmailServices _emailService;
        private SessionService _sessionService;
        private GUID _guid;


        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAccountController(EmailServices emailServices, ILogger<UserAccountController> logger, IHttpContextAccessor httpContextAccessor,IConfiguration configuration, SessionService sessionService, GUID guid) : base(configuration)
        {
            _emailService = emailServices;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _guid = guid;
        }
        //List Page for User Creation
        [HttpGet("getAllUserAccount")]
        public async Task<IActionResult> GetAllUserAccount()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    
                    string response = _sessionService.GetSession("Guid");
                    if (!string.IsNullOrEmpty(response))
                    {
                        var lstUserAccountModel = await _repo.UserAccountDALRepo.GetAllUserAccount();
                        if (lstUserAccountModel != null)
                        {
                            return Ok(lstUserAccountModel);
                        }
                        else
                        {
                            return BadRequest("No Records Found");
                        }
                    }

                    else
                    {
                        return BadRequest("Try to Login");
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
                    string response = _sessionService.GetSession("Guid");
                    if (!string.IsNullOrEmpty(response))
                    {
                        var lstUserPolicyModel = await _repo.UserAccountDALRepo.getAllUserPolicyInDropdown();
                        if (lstUserPolicyModel != null)
                        {
                            return Ok(lstUserPolicyModel);
                        }
                        else
                        {
                            return BadRequest("No Records Found");
                        }

                    }
                    else
                    {
                        return BadRequest("Try to Login");
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
                    string response =  _sessionService.GetSession("Guid");
                    if (!string.IsNullOrEmpty(response))
                    {
                        var lstUserPolicyModel = await _repo.UserAccountDALRepo.getAllUserRoleInDropdown();
                        if (lstUserPolicyModel != null)
                        {
                            return Ok(lstUserPolicyModel);
                        }
                        else
                        {
                            return BadRequest("No Records Found");
                        }
                    }

                    else { return BadRequest("Try to Login"); }
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
                    string responseGUId = _sessionService.GetSession("Guid");

                    if (!string.IsNullOrEmpty(responseGUId))
                    {
                        string guidresp = await _guid.GetGUIDBasedOnUserGuid(guid);
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
                        return BadRequest("Try to Login");
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
        [HttpGet("getallOrgDetails")]
        public async Task<IActionResult> getOrgDetailsByUserGUId()
        {
            try
            {

                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession("strUserID");
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession("Guid");
                    if (!string.IsNullOrEmpty(response))
                    {
                        var objuseraccountModel = await _repo.UserAccountDALRepo.GetOrgDetailsByUserGUId();
                        if (objuseraccountModel != null)
                        {
                            return Ok(objuseraccountModel);
                        }
                        else
                        {
                            return BadRequest();
                        }
                    }
                    else
                    {
                        return BadRequest("Try to Login");
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
        [HttpPost("insertUpdateUserAccount")]
        public async Task<IActionResult> InsertUpdateUserAccount(UserAccountUpdateRequest objModel)
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
                    string userIdStr = _sessionService.GetSession("strUserID");
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession("Guid");
                    if (!string.IsNullOrEmpty(response))
                    {

                        DataTable dataTable = objModel.UserAccount.ConvertToDataTable(objModel.OrgDataTable, 0);
                        DataTable dataTableRole = objModel.UserAccount.ConvertToDataTable(objModel.RoleNameList, userId, 0);
                        var result = await _repo.UserAccountDALRepo.InsertUpdateUserAccount(objModel.UserAccount);
                        _repo.Commit();
                        if (result.InsertedUsers != null || result.OrgDetails == null || result.OrgDetails != null)
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
                    }
                    else
                    {
                        return BadRequest("Try to Login");
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
        [HttpPut("EditUpdateUserAccount")]
        public async Task<IActionResult> EditUpdateUserAccount([FromBody] UserAccountModel userAccount)
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

                        string userIdStr = _sessionService.GetSession("strUserID");
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession("Guid");
                        if (!string.IsNullOrEmpty(response))
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
                        }
                        else
                        {
                            return BadRequest("Try to Login");
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
                    string responseMsg = string.Empty;
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {
                        string userIdStr = _sessionService.GetSession("strUserID");
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                        string response = _sessionService.GetSession("Guid");
                        if (!string.IsNullOrEmpty(response))
                        {
                            var result = await _repo.UserAccountDALRepo.UpdateUserAccountAsync(userAccount?.UserAccount);
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
                                        _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
                                        return BadRequest();
                                }

                            }
                        }
                        else
                        {
                            return BadRequest("Try To Login");
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
        [HttpDelete("deleteUserAccount")]
        public async Task<IActionResult> DeleteUserAccount(DeleteUserAccount deleteUserAccount)
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    
                    string response = _sessionService.GetSession("Guid");
                    if (!string.IsNullOrEmpty(response))
                    {
                        var datatable = deleteUserAccount.ConvertToDataTable(deleteUserAccount.DeleteDataTable);
                        var result = await _repo.UserAccountDALRepo.DeleteUserAccount(Convert.ToInt64(_sessionService.GetSession("strUserID")), deleteUserAccount);

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
                        return BadRequest("Try to Login");
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
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt32(userIdStr) : 0;
                        string response = _sessionService.GetSession("Guid");
                        if(!string.IsNullOrEmpty(response))
                        {
                            objModel.CreatedBy= userId;
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
                        string? userIdStr = _sessionService.GetSession("strUserID");
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt32(userIdStr) : 0;
                        objModel.CreatedBy = userId;
                        string response = _sessionService.GetSession("Guid");
                        if (!string.IsNullOrEmpty(response))
                        {
                            foreach (var guid in objModel.DeleteRoleNames)
                            {
                                string? guidresp = await _guid.GetGUIDBasedOnUserAccountRoleGuid(guid.UserAccountRoleGuid);
                                if (guidresp == guid.UserAccountRoleGuid)
                                {
                                    DataTable? deletetable = objModel?.ConvertToDataTable(objModel.DeleteRoleNames ?? new List<DeleteRoleNameInList>());
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
                            return BadRequest("Try to Login");
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
                        long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt32(userIdStr) : 0;
                        string response = _sessionService.GetSession("Guid");
                        if (!string.IsNullOrEmpty(response))
                        {
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
                    Convert.ToInt64(_sessionService.GetSession("strUserID")),
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
                            return BadRequest("Try to Login");
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
        public async Task<IActionResult> UnlockUser([FromBody] UnlockUser listofunlock)
        {
            if (listofunlock == null)
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
                        string response = _sessionService.GetSession("Guid");
                        if (!string.IsNullOrEmpty(response))
                        {
                            DataTable? datatable = listofunlock?.ConvertToDataTable(listofunlock.Users ?? new List<UnlockUserList>()) ?? new DataTable();
                            listofunlock.UpdatedBy = userId;
                            var result = await _repo.UserAccountDALRepo.UnlockUserAsync(listofunlock);
                            var msg = "Unlock User successfully.";
                            _repo.Commit();
                            if (result.unlockuser != null && result.RetVal == 1)
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
                        else
                        {
                            return BadRequest("Try to Login");
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