using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services;
using DataAccessLayer.Services;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ApiBaseController
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly EmailServices _emailService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAccountController(EmailServices emailServices, ILogger<UserAccountController> logger, IHttpContextAccessor httpContextAccessor,IConfiguration configuration) : base(configuration)
        {
            _emailService = emailServices;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        //List Page for User Creation
        [HttpGet("getAllUserAccount")]
        public async Task<IActionResult> GetAllUserAccount()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    var lstUserAccountModel = await _repo.UserAccountDALRepo.GetAllUserAccount();
                    if (lstUserAccountModel != null)
                    {
                        //string plaintext = "Server=103.224.167.139,1437;Database=MasterData;User ID=sa;Password=DEV@dmin;MultipleActiveResultSets=True;TrustServerCertificate=True;";
                        //byte[] ciphertext = _iCS.Encrypt(plaintext, key, iv);
                        return Ok(lstUserAccountModel);

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

        //Get User Policy in DropDown
        [HttpGet("getAllUserPolicyinDropdown")]
        public async Task<IActionResult> getAllUserPolicyinDropdown()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    var lstUserPolicyModel = await _repo.UserAccountDALRepo.getAllUserPolicyinDropdown();
                    if (lstUserPolicyModel != null)
                    {
                        return Ok(lstUserPolicyModel);
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
        // Get User Role in Dropdown
        [HttpGet("getAllUserRoleinDropdown")]
        public async Task<IActionResult> getAllUserRoleinDropdown()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    var lstUserPolicyModel = await _repo.UserAccountDALRepo.getAllUserRoleinDropdown();
                    if (lstUserPolicyModel != null)
                    {
                        return Ok(lstUserPolicyModel);
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

        //Getting Specific User ID from User Account Creation
        [HttpGet("getUserAccountById/{Userid}")]

        public async Task<IActionResult> GetUserAccountById(int Userid)
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    var objuseraccountModel = await _repo.UserAccountDALRepo.GetUserAccountById(Userid);
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

        // Displaying Organization Details Based on User ID
        [HttpGet("getallOrgDetails")]
        public async Task<IActionResult> getOrgDetailsByUserId()
        {
            try
            {

                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    var objuseraccountModel = await _repo.UserAccountDALRepo.GetOrgDetailsByUserId();
                    if (objuseraccountModel != null)
                    {
                        return Ok(objuseraccountModel);
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

        //Insert New User Account Creation
        [HttpPost("insertUpdateUserAccount")]
        public async Task<IActionResult> InsertUpdateUserAccount(UserAccountUpdateRequest objModel)
        {
            try
            {
                string responseMsg = string.Empty;
                if (objModel == null || objModel.UserAccount == null || objModel.OrgDatatable == null || objModel.RoleNameList == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {
                        DataTable dataTable = objModel.UserAccount.ConvertToDataTable(objModel.OrgDatatable, 0);
                        DataTable dataTableRole = objModel.UserAccount.ConvertToDataTable(objModel.RoleNameList, 0);
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
                                    await _emailService.SendMailMessage(EmailTemplateCode.USER_ACCOUNT_CREATED, -1, result.RetVal, objModel.UserAccount.UserPassword);
                                    //_httpContextAccessor?.HttpContext?.Session.Remove("DBName");
                                    //_httpContextAccessor?.HttpContext?.Session.Remove("DBChange");
                                    //_httpContextAccessor?.HttpContext?.Session.Remove("InstanceChange");
                                    //_httpContextAccessor?.HttpContext?.Session.Remove("InstanceName");
                                    //_httpContextAccessor?.HttpContext?.Session.Remove("DataBaseUserName");
                                    //_httpContextAccessor?.HttpContext?.Session.Remove("DataBasePassword");
                                    break;

                                default:
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
                                    return NotFound("User Account Already Exists" + BadRequest());
                            }

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
                }
                return Ok(responseMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }


        [HttpPost("ResetPasswordinUserAccount")]
        public async Task<IActionResult> ResetPasswordinUserAccount(ResetPassword objModel)
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
                        var result = await _repo.UserAccountDALRepo.ResetPasswordinUserAccount(objModel);

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
                                                                            -1, Convert.ToInt32(_httpContextAccessor?.HttpContext?.Session.GetString("strUserID")), objModel.Password);
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
                }
                return Ok(responseMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }


        // Update User Based on UserID
        [HttpPut("updateUserAccount/")]
        public async Task<IActionResult> UpdateUserAccount([FromBody] UserAccountModel userAccount)
        {
            if (userAccount == null)
            {
                return BadRequest("Invalid input data.");
            }

            else
            {
                try
                {
                    string responseMsg = string.Empty;
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {

                        var result = await _repo.UserAccountDALRepo.UpdateUserAccountAsync(userAccount);
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
                    return Ok(responseMsg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }

        // Unlock User
        [HttpPut("UnlockUser")]
        public async Task<IActionResult> UnlockUser([FromBody] ListofUnlock listofunlock)
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
                        DataTable? datatable = listofunlock.User?.ConvertToDataTable(listofunlock.Users ?? new List<UnlockUserList>()) ?? new DataTable();
                        var result = await _repo.UserAccountDALRepo.UnlockUserAsync(listofunlock.User);
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
                    return Ok(responseMsg);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message + "  " + ex.StackTrace);
                    throw;
                }
            }
        }
        // Delete the User Account
        [HttpDelete("DeleteRoleinUserAccount")]
        public async Task<IActionResult> DeleteRoleinUserAccount(DeleteRoleinUserAccount objModel)
        {
            try
            {
                string responseMsg = string.Empty;
                if (objModel == null || objModel.DeleteRoleName == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                    {
                        DataTable? deletetable = objModel.DeleteRoleNamesingle?.ConvertToDataTable(objModel.DeleteRoleName ?? new List<DeleteRoleNameinList>());
                        var result = await _repo.UserAccountDALRepo.DeleteRoleinUserAccount(objModel.DeleteRoleNamesingle ?? new DeleteRoleName());

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
                }
                return Ok(responseMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpDelete("deleteUserAccount")]
        public async Task<IActionResult> DeleteUserAccount(DeleteUserAccount deleteUserAccount)
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(_httpContextAccessor))
                {
                    var datatable = deleteUserAccount.ConvertToDataTable(deleteUserAccount.DeleteDatatable);
                    var result = await _repo.UserAccountDALRepo.DeleteUserAccount(Convert.ToInt32(_httpContextAccessor?.HttpContext?.Session.GetString("strUserID")),deleteUserAccount);
                    
                    _repo.Commit();
                    if (result.deleteuseraccount ==true || result.deleteuseraccount ==false)
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