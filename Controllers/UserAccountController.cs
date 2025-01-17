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
    public class UserAccountController : ApiBaseController
    {
        private readonly ILogger<UserAccountController> _logger;
        public UserAccountController(ILogger<UserAccountController> logger, IConfiguration configuration) : base(configuration)
        {
            _logger = logger;
            
        }
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
        [HttpGet("getUserAccountById/{id}")]
        public async Task<IActionResult> GetUserAccountById(int id)
        {
            try
            {
                //int i = 0;
                //var j = 1 / i;
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                {
                    var objuseraccountModel = await _repo.UserAccountDALRepo.GetUserAccountById(id);
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
        [HttpPost("insertUpdateUserAccount")]
        public async Task<IActionResult> InsertUpdateUserAccount(UserAccountModel objModel)
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                {
                    var result = await _repo.UserAccountDALRepo.InsertUpdateUserAccount(objModel);
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        _logger.LogError(Environment.NewLine);
                        _logger.LogError("Bad Request occurred while accessing the InsertUpdateUserAccount function in User Account api controller");
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
        [HttpGet("deleteUserAccount/{id}")]
        public async Task<IActionResult> DeleteUserAccount(int id)
        {
            try
            {
                using (IUowUserAccount _repo = new UowUserAccount(ConnectionString))
                {
                    var result = await _repo.UserAccountDALRepo.DeleteUserAccount(id);
                    _repo.Commit();
                    if (result)
                    {
                        return Ok(result);
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
