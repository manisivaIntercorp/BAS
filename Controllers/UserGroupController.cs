using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Eventing.Reader;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ApiBaseController
    {
        private readonly ILogger<UserGroupController> _logger;
        public UserGroupController(ILogger<UserGroupController> logger, IConfiguration configuration) : base(configuration)
        {
            _logger = logger;

        }
        [HttpGet("getAllUserPolicy")]
        public async Task<IActionResult> GetAllUserPolicy()
        {
            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(ConnectionString))
                {
                    var lstUserGroupModel = await _repo.UserGroupDALRepo.GetAllUserPolicy();
                    if (lstUserGroupModel != null)
                    {
                        return Ok(lstUserGroupModel);
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
        [HttpGet("getUserPolicyById/{id}")]
        public async Task<IActionResult> GetUserGroupById(int id)
        {
            try
            {
                //int i = 0;
                //var j = 1 / i;
                using (IUowUserGroup _repo = new UowUserGroup(ConnectionString))
                {
                    var objUserGroupModel = await _repo.UserGroupDALRepo.GetUserPolicyById(id);
                    if (objUserGroupModel != null)
                    {
                        return Ok(objUserGroupModel);
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
        [HttpPost("insertUpdateUserPolicy")]
        public async Task<IActionResult> InsertUpdateUserGroup(UserGroupModel objModel)
        {
            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(ConnectionString))
                {
                    var result = await _repo.UserGroupDALRepo.InsertUpdateUserPolicy(objModel);
                    var msg = "User Policy Inserted Successfully";
                    _repo.Commit();
                    if (result)
                    {
                        Ok(result);
                        Ok(msg);

                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserGroup function in User Group api controller");
                        return NotFound("User Policy Already Exists");
                    }
                    return Ok(msg + ' ' + result);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        [HttpPut("updateUserPolicy/{id}")]
        public async Task<IActionResult> UpdateUserGroup(int id, [FromBody] UserGroupModel UserGroup)
        {

            if (UserGroup == null || id != UserGroup.UserGroupID)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(ConnectionString))
                {
                    var result = await _repo.UserGroupDALRepo.UpdateUserPolicyAsync(id, UserGroup);
                    var msg = "User Group updated successfully.";
                    _repo.Commit();
                    if (result)
                    {
                        Ok(result);
                        Ok(msg);

                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the updateUserPolicy function in User Policy api controller");
                        return BadRequest();
                    }
                    return Ok(msg + result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;

            }
        }

        [HttpGet("deleteUserPolicy/{id}")]
        public async Task<IActionResult> DeleteUserGroup(int id)
        {
            try
            {
                using (IUowUserGroup _repo = new UowUserGroup(ConnectionString))
                {
                    var result = await _repo.UserGroupDALRepo.DeleteUserPolicy(id);
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the DeleteUserPolicy function in User Policy api controller");
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
