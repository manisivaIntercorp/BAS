using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Xml.Linq;
using WebApi.Services;
using WebApi.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ApiBaseController
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtService _jwtService;
        private readonly AppGlobalVariableService _appGlobalVariableService;


        public LoginController(ILogger<LoginController> logger, AppGlobalVariableService appGlobalVariableService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            // Retrieve JWT key from configuration
            var jwtKey = configuration["JwtSettings:Key"];
            _jwtService = new JwtService(jwtKey);
            _appGlobalVariableService = appGlobalVariableService;
        }

        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLogin(LoginModel objLogModel)
        {
            try
            {
                using (IUowLogin _repo = new UowLogin(_httpContextAccessor))
                {
                    var lstLoginUser = await _repo.LoginDALRepo.UserLogin(objLogModel);

                    if (lstLoginUser != null && lstLoginUser.Any())
                    {
                        var loginResult = lstLoginUser.First();
                        var loginDetail = loginResult.lstLoginDetails?.FirstOrDefault();

                        if (loginDetail != null)
                        {
                            HttpContext.Session.SetString("DBName", loginDetail.DBName);
                            // You can store other values as needed
                        }

                        var token = _jwtService.GenerateToken(objLogModel?.UserName ?? "");
                        return Ok(token);
                    }
                    else
                    {
                        return Unauthorized("Invalid login.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}  {ex.StackTrace}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }
        private void GetUserData(List<ResultModel> objLoginModel, string strMode)
        {
            if (objLoginModel != null && objLoginModel.Count > 0)
            {
                var RetVal = objLoginModel[0]?.RetVal ?? 0;
                var RetMsg = objLoginModel[0]?.RetMsg?.Trim() ?? string.Empty;
                string strDBName = string.Empty;
                if (RetVal == 1)
                {
                    var lstUserDetails = objLoginModel[0].lstLoginDetails;
                    if (lstUserDetails != null && lstUserDetails.Count > 0)
                    {
                        strDBName = lstUserDetails[0]?.DBName?.Trim() ?? string.Empty;
                        string strUserID = Convert.ToString(lstUserDetails[0]?.UserID ?? 0);
                        string strUserName = lstUserDetails[0]?.UserName?.Trim() ?? string.Empty;
                        string strUserDisplayName = lstUserDetails[0]?.DisplayName?.Trim() ?? string.Empty;
                        string strUserRoleID = Convert.ToString(lstUserDetails[0]?.RoleID ?? 0);
                        string strUserRoleName = lstUserDetails[0]?.RoleDesc?.Trim() ?? string.Empty;
                        string strUserRoleType = lstUserDetails[0]?.RoleName?.Trim() ?? string.Empty;
                        string strDisplayPDPA = lstUserDetails[0]?.DisplayPDPA?.Trim() ?? string.Empty;
                        string strPayrollAccessible = lstUserDetails[0]?.PayrollAccessible?.Trim() ?? string.Empty;
                        string strUserAccessRightsCount = lstUserDetails[0]?.UserAccessRightsCount?.Trim() ?? string.Empty;
                        string strSetPassword = lstUserDetails[0]?.SetPassword?.Trim() ?? string.Empty;
                        string strPasswordExpiry = lstUserDetails[0]?.PasswordExpiry?.Trim() ?? string.Empty;
                        string strValidateOTP = lstUserDetails[0]?.ValidateOTP?.Trim() ?? string.Empty;
                        string strIsProfileUser = "N";
                        string strIsSystemUser = "N";
                        string strUserImgPath = lstUserDetails[0]?.ImagePath?.Trim() ?? string.Empty;

                        HttpContext.Session.SetString("DBName", strDBName);
                        HttpContext.Session.SetString("strUserID", strUserID);
                        HttpContext.Session.SetString("strUserName", strUserName);
                        HttpContext.Session.SetString("strUserDisplayName", strUserDisplayName);
                        HttpContext.Session.SetString("strUserRoleID", strUserRoleID);
                        HttpContext.Session.SetString("strUserRoleName", strUserRoleName);
                        HttpContext.Session.SetString("strUserRoleType", strUserRoleType);
                        HttpContext.Session.SetString("strDisplayPDPA", strDisplayPDPA);
                        HttpContext.Session.SetString("strPayrollAccessible", strPayrollAccessible);
                        HttpContext.Session.SetString("strUserAccessRightsCount", strUserAccessRightsCount);
                        HttpContext.Session.SetString("strSetPassword", strSetPassword);
                        HttpContext.Session.SetString("strPasswordExpiry", strPasswordExpiry);
                        HttpContext.Session.SetString("strValidateOTP", strValidateOTP);
                        HttpContext.Session.SetString("strIsProfileUser", strIsProfileUser);
                        HttpContext.Session.SetString("strIsSystemUser", strIsSystemUser);
                        HttpContext.Session.SetString("strUserImgPath", strUserImgPath);
                    }

                    if (string.IsNullOrWhiteSpace(strDBName))
                    {
                        HttpContext.Session.SetString("DBName", strDBName);
                        HttpContext.Session.SetString("DBChange", "N");
                        HttpContext.Session.SetString("GlobalUser", "Y");
                        HttpContext.Session.SetString("InstanceChange", "N");
                        HttpContext.Session.SetString("InstanceName", "Y");
                        HttpContext.Session.SetString("DataBaseUserName", string.Empty);
                        HttpContext.Session.SetString("DataBasePassword", string.Empty);
                    }
                    else
                    {
                        HttpContext.Session.SetString("DBName", strDBName);
                        HttpContext.Session.SetString("DBChange", "Y");
                        HttpContext.Session.SetString("GlobalUser", "N");
                        if (lstUserDetails != null && lstUserDetails.Count > 0)
                        {
                            HttpContext.Session.SetString("TimeOffset", Convert.ToString(lstUserDetails[0]?.TimeOffset?.Trim() ?? string.Empty));
                            HttpContext.Session.SetString("TimeZoneID", Convert.ToString(lstUserDetails[0]?.TimeZoneID?.Trim() ?? string.Empty));
                            HttpContext.Session.SetString("IsAppuserTimeZone", Convert.ToString(lstUserDetails[0]?.IsAppuserTimeZone?.Trim() ?? string.Empty));
                        }
                    }

                    if (strMode == "GET" && lstUserDetails != null)
                    {
                        if (!String.IsNullOrWhiteSpace(Convert.ToString(lstUserDetails[0]?.IsProfileUser?.Trim() ?? string.Empty)))
                        {
                            HttpContext.Session.SetString("IsProfileUser", Convert.ToString(lstUserDetails[0]?.IsProfileUser?.Trim() ?? string.Empty));
                        }
                        else if (!String.IsNullOrWhiteSpace(Convert.ToString(lstUserDetails[0]?.IsSystemUser?.Trim() ?? string.Empty)))
                        {
                            HttpContext.Session.SetString("IsSystemUser", Convert.ToString(lstUserDetails[0]?.IsSystemUser?.Trim() ?? string.Empty));
                        }
                    }
                    else if (strMode == "" && lstUserDetails != null)
                    {
                        if (!String.IsNullOrWhiteSpace(Convert.ToString(lstUserDetails[0]?.LanguageCode?.Trim() ?? string.Empty)))
                        {
                            HttpContext.Session.SetString("LanguageCode", Convert.ToString(lstUserDetails[0]?.LanguageCode?.Trim() ?? string.Empty));
                        }
                    }
                }
            }
        }
        

        [HttpPost("Get UserID")]
        public async Task<IActionResult> GetUserID(LoginModel objModel)
        {
            try
            {
                using (IUowLogin _repo = new UowLogin(_httpContextAccessor))
                {
                    var lstuser = await _repo.LoginDALRepo.GetUserID(objModel);
                    if(lstuser != null)
                    {
                        return Ok(lstuser);
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


    }
}
