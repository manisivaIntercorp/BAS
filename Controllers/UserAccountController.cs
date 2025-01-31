using Azure;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ApiBaseController
    {
        private readonly ILogger<UserAccountController> _logger;
        public UserAccountController(ILogger<UserAccountController> logger, IConfiguration configuration) : base(configuration)
        {
            _logger = logger;
            
        }
        //List Page for User Creation
        [HttpGet("getAllUserAccount")]
        public async Task<IActionResult> GetAllUserAccount()
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                {
                    var lstUserAccountModel = await _repo.UserAccountDALRepo.GetAllUserAccount();
                    if (lstUserAccountModel != null)
                    {
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
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
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
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
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
            try {
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                {
                    var objuseraccountModel = await _repo.UserAccountDALRepo.GetUserAccountById(Userid);
                    var response = new UserAccountResponse();
                    if (objuseraccountModel.userAccounts != null || objuseraccountModel.UserRoles!=null || objuseraccountModel.Org!=null)
                    {
                        if (objuseraccountModel.userAccounts.UserID != null && objuseraccountModel.userAccounts.UserID == 0)
                        {
                            // Return 204 No Content
                            return NoContent();
                        }
                        if (objuseraccountModel.userAccounts.UserID != null && objuseraccountModel.userAccounts.UserID!=0)
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
                    else {
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
                
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
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
                if (objModel == null || objModel.UserAccount == null || objModel.OrgDatatable == null ||  objModel.RoleNameList == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                    {
                        DataTable dataTable = objModel.UserAccount.ConvertToDataTable(objModel.OrgDatatable, 0);
                        DataTable dataTableRole = objModel.UserAccount.ConvertToDataTable(objModel.RoleNameList, 0);
                        var result = await _repo.UserAccountDALRepo.InsertUpdateUserAccount(objModel.UserAccount);
                        var msg = "User Account Inserted Successfully";
                        _repo.Commit();
                        if ((result.insertroles != null && result.RetVal==0) || (result.insertroles != null && result.RetVal == -1)) // Already Exists
                        {
                            return Ok(result.Msg);
                        }
                        if (result.insertroles != null && result.RetVal >=1)// Success
                        {
                            return Ok(result.Msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
                            return NotFound("User Account Already Exists" + BadRequest());
                        }
                    }
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
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                    {
                        var result = await _repo.UserAccountDALRepo.AddRoleName(objModel);
                        var msg = "Role Inserted Successfully";
                        _repo.Commit();
                        if (result.Roles != null && result.RetVal == 0)// Already Exists
                        {
                            return Ok(result.Msg);
                        }
                        if (result.Roles != null && result.RetVal == -4)// Null Value Return
                        {
                            return Ok(result.Msg);
                        }
                        if (result.Roles != null && result.RetVal == 1)// Success
                        {
                            return Ok(result.Msg);
                        }
                        if (result.Roles != null && result.RetVal == -1)// Effective Date Should not be greater than current date
                        {
                            return Ok(result.Msg);
                        }
                        if (result.Roles != null && result.RetVal == -2)// User Does Not Exist
                        {
                            return Ok(result.Msg);
                        }
                        if (result.Roles != null && result.RetVal == -3)// There cannot be more than one role for the high level user!
                        {
                            return Ok(result.Msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the AddRoleInUserAccount function in User Account api controller");
                            return BadRequest();
                        }
                    }
                }
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
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                    {
                        var result = await _repo.UserAccountDALRepo.ResetPasswordinUserAccount(objModel);
                        var msg = "Password Reset Successfully";
                        _repo.Commit();
                        if (result.PasswordReset != null && result.RetVal == 0)// Already Exists
                        {
                            return Ok(result.Msg);
                        }
                        if (result.PasswordReset != null && result.RetVal == -4)// Null Value Return
                        {
                            return Ok(result.Msg);
                        }
                        if (result.PasswordReset != null && result.RetVal == 1)// Success
                        {
                            return Ok(result.Msg);
                        }
                        if (result.PasswordReset != null && result.RetVal == -1)// Effective Date Should not be greater than current date
                        {
                            return Ok(result.Msg);
                        }
                        if (result.PasswordReset != null && result.RetVal == -2)// User Does Not Exist
                        {
                            return Ok(result.Msg);
                        }
                        if (result.PasswordReset != null && result.RetVal == -3)// There cannot be more than one role for the high level user!
                        {
                            return Ok(result.Msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the AddRoleInUserAccount function in User Account api controller");
                            return BadRequest();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }


        // Update User Based on UserID
        [HttpPut("updateUserAccount/{id}")]
        public async Task<IActionResult> UpdateUserAccount(int id, [FromBody] UserAccountModel userAccount)
        {
            if (userAccount == null ||id != userAccount.UserId)
            {
                return BadRequest("Invalid input data.");
            }
            
            else
            {
                try
                {
                    using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                    {
                        
                        var result = await _repo.UserAccountDALRepo.UpdateUserAccountAsync(id, userAccount);
                        var msg = "User account updated successfully.";
                        _repo.Commit();
                        if (result.updateuseraccount!=null && result.RetVal>=1)
                        {
                            return Ok(msg);
                        }
                        if(result.updateuseraccount != null && result.RetVal == -1)
                        {
                            return Ok(result.Msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
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
                    using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                    {
                         DataTable datatable = listofunlock.User.ConvertToDataTable(listofunlock.Users);
                        var result = await _repo.UserAccountDALRepo.UnlockUserAsync(listofunlock.User);
                        var msg = "Unlock User successfully.";
                        _repo.Commit();
                        if (result.unlockuser != null && result.RetVal == 1)// Success
                        {
                            return Ok(msg);
                        }
                        if (result.unlockuser != null && result.RetVal == -1)// Failure or Not exists
                        {
                            return Ok(result.Msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the updateUserAccount function in User Account api controller");
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
        // Delete the User Account
        [HttpDelete("DeleteRoleinUserAccount")]
        public async Task<IActionResult> DeleteRoleinUserAccount(DeleteRoleinUserAccount objModel)
        {
            try
            {
                if (objModel == null  ||objModel.DeleteRoleName == null)
                {
                    return BadRequest("Invalid input data.");
                }
                else
                {
                    using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                    {
                        var deletetable = objModel.DeleteRoleNamesingle.ConvertToDataTable(objModel.DeleteRoleName);
                        var result = await _repo.UserAccountDALRepo.DeleteRoleinUserAccount(objModel.DeleteRoleNamesingle);
                        var msg = "Role Deleted Successfully";
                        _repo.Commit();
                        if (result.deleteroles != null && result.RetVal==1)//Success
                        {
                            return Ok(msg);
                        }
                        if (result.deleteroles != null && result.RetVal == -1)//Failure
                        {
                            return Ok(result.Msg);
                        }
                        if (result.deleteroles != null && result.RetVal == -4)// Null Value Return
                        {
                            return Ok(result.Msg);
                        }
                        else
                        {
                            _logger.LogError(Environment.NewLine);
                            _logger.LogError("Bad Request occurred while accessing the DeleteRoleinUserAccount function in User Account api controller");
                            return NotFound(BadRequest());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }
        [HttpGet("deleteUserAccount/{id}")]
        public async Task<IActionResult> DeleteUserAccount(int id)
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                {
                    var result = await _repo.UserAccountDALRepo.DeleteUserAccount(id);
                    var msg = "User Account Deleted Successfully";
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(msg);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the DeleteUserAccount function in User Account api controller");
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