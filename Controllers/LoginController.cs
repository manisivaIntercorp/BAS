using DataAccessLayer.Model;
using DataAccessLayer.Services;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;
using WebApi.Services;


namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ApiBaseController
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtService _jwtService;
        //private readonly AppGlobalVariableService _appGlobalVariableService;
        private readonly EncryptedDecrypt  _encryptedDecrypt;
     

        public LoginController(ILogger<LoginController> logger, EncryptedDecrypt encryptedDecrypt,
        JwtService jwtService,IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;  // Now injected
            //_appGlobalVariableService = appGlobalVariableService;
            _encryptedDecrypt = encryptedDecrypt;
        }

        [HttpPost("Get UserID")]
        public async Task<IActionResult> GetUserID(string username, string? Language)
        {
            try
            {
                var objLogModel = new LoginModel { UserName = username, LanguageCode = Language };

                using (IUowLogin _repo = new UowLogin(_httpContextAccessor,_configuration,_encryptedDecrypt))
                {
                    var lstuser = await _repo.LoginDALRepo.GetUserID(objLogModel);

                    if (lstuser == null || lstuser.Count == 0)
                    {
                        return Unauthorized("Invalid User Name.");
                    }

                    switch (lstuser[0].RetVal)
                    {
                        case -1:
                            return Unauthorized("Invalid User Name.");

                        case 1:
                            GetUserData(lstuser, "GET");
                            return Ok(lstuser);

                        default:
                            return Unauthorized("Invalid User Name.");
                    }
                   
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }


        [HttpGet("UserLogin")]
        public async Task<IActionResult> UserLogin(string username, string password)
        {

          
            try
            {
                string strGuid = string.Empty;
                if(HttpContext?.Session?.GetString("Guid") != null)
                {
                    strGuid = Convert.ToString(HttpContext?.Session?.GetString("Guid"));
                }
                var objLogModel = new LoginModel { UserName = username, Password = EncryptShaAlg.Encrypt(password + strGuid) };

                using (IUowLogin _repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt))

                {
                    var lstLoginUser = await _repo.LoginDALRepo.UserLogin(objLogModel);

                    if (lstLoginUser == null || !lstLoginUser.Any())
                    {
                        return Unauthorized("Invalid Password");
                    }

                    switch (lstLoginUser[0].RetVal)
                    {
                        case -1:
                            return Unauthorized("Invalid Password");

                        case 1:
                            var loginDetail = lstLoginUser.First()?.lstLoginDetails?.FirstOrDefault();
                            var vGuid ="";
                            if (loginDetail != null)
                            {
                                HttpContext.Session.SetString("UserName", objLogModel.UserName);
                                HttpContext.Session.SetString("Password", EncryptShaAlg.Encrypt(objLogModel.Password + strGuid));
                                GetUserData(lstLoginUser, "GET");
                                vGuid = loginDetail?.Guid;
                            }

                            var token = _jwtService.GenerateToken(objLogModel?.UserName ?? "",Convert.ToString(vGuid), objLogModel?.Password ?? "");
                            return Ok(token);

                        default:
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
                        //strDBName = _encryptedDecrypt.Encrypt(lstUserDetails[0]?.DBName)?.Trim() ?? string.Empty;
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
                        string strGuid = lstUserDetails[0]?.Guid?.Trim() ?? string.Empty;



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
                        HttpContext.Session.SetString("Guid", strGuid);
                    }

                    if (string.IsNullOrWhiteSpace(strDBName))
                    {
                        HttpContext.Session.SetString("DBName", strDBName);
                        HttpContext.Session.SetString("DBChange", "N");
                        HttpContext.Session.SetString("GlobalUser", "Y");
                        HttpContext.Session.SetString("InstanceChange", "N");
                        HttpContext.Session.SetString("InstanceName", string.Empty);
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

        [HttpPost("GetOrganisationWithDBDetails")]
        public async Task<IActionResult> GetOrganisationWithDBDetails(string username)
        {
            var objLogModel = new LoginModel { UserName = username };
            try
            {
                using (IUowLogin _repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt))
                {
                    var lstOrgDetails = await _repo.LoginDALRepo.GetOrganisationWithDBDetails(objLogModel);
                    if (lstOrgDetails != null)
                    {
                        // Serialize the list to a JSON string
                        var orgDetailsJson = System.Text.Json.JsonSerializer.Serialize(lstOrgDetails);
                        HttpContext.Session.SetString("OrgDetails", orgDetailsJson);
                        return Ok(lstOrgDetails);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                throw;
            }
        }

        [HttpGet("SelectOrganisation")]
        public async Task<IActionResult> SelectedOrganisation(int orgID, string? orgCode)
        {
            if (orgID <= 0)
                return BadRequest("Invalid Organisation ID.");

            try
            {
                var dbDetailsJson = HttpContext.Session.GetString("OrgDetails");
                if (string.IsNullOrWhiteSpace(dbDetailsJson))
                    return NotFound("Organisation details not found.");

                var dbDetails = JsonConvert.DeserializeObject<DataTable>(dbDetailsJson);
                var dbRow = dbDetails?.AsEnumerable()
                    .FirstOrDefault(row => row.Field<long>("ID") == orgID);

                if (dbRow == null)
                {
                    return NotFound($"No organisation found with ID: {orgID}.");
                }
                else
                {
                    HttpContext.Session.SetString("DBChange","Y");
                    HttpContext.Session.SetString("InstanceChange", "Y");
                    HttpContext.Session.SetString("DBName", dbRow.ItemArray[4].ToString());
                    HttpContext.Session.SetString("InstanceName", dbRow.ItemArray[6].ToString());
                    HttpContext.Session.SetString("DataBaseUserName", dbRow.ItemArray[7].ToString());
                    HttpContext.Session.SetString("DataBasePassword", dbRow.ItemArray[8].ToString());
                }

                

                var loginModel = new LoginModel
                {
                    UserName = HttpContext.Session.GetString("strUserName"),
                    Password = HttpContext.Session.GetString("Password"),
                    GlobalUser = HttpContext.Session.GetString("GlobalUser")
                };
                using var repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt);
                var users = await repo.LoginDALRepo.ClientUserLogin(loginModel);

                if (users != null && users.Any())
                {
                    var loginDetail = users.First()?.lstLoginDetails?.FirstOrDefault();
                    var vGuid = "";
                    if (loginDetail != null)
                    {
                        GetUserData(users, "GET");
                        vGuid = loginDetail?.Guid;
                    }
                    var token = _jwtService.GenerateToken(loginModel?.UserName ?? "", Convert.ToString(vGuid), loginModel?.Password ?? "");

                    return Ok(token);
                }

                return Unauthorized("Invalid login.");
            }
            catch (ObjectDisposedException)
            {
                return StatusCode(500, "A data reader was accessed after disposal.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //}
        [HttpPost("Logout")]
        public IActionResult Logout(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Token is required for logout.");
                }

                _jwtService.InvalidateToken(token);
                // HttpContext.Session.Remove();
                HttpContext.Session.Clear();
                return Ok("Logout successful. Token revoked.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Logout error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred during logout.");
            }
        }

        [HttpPost("DropDownLanguage")]
        public async Task<IActionResult> GetDDlLanguage(string? RefID1, string? RefID2, string? RefID3)
        {
            try
            {
                // var objDDlModel = new GetDropDownDataModel { Mode = Mode, RefID1 = RefID1, RefID2 = RefID2, RefID3 = RefID3, RefID4 = RefID4 };
                string Mode = "LANGUAGE";
                using (IUowLogin _repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt))
                {
                    var lstuser = await _repo.LoginDALRepo.GetDDlLanguage(Mode,RefID1,RefID2,RefID3);
                    if (lstuser != null)
                    {
                        return Ok(lstuser);
                    }
                    else
                    {
                        return Unauthorized("Invalid User Name.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request");
            }
        }

        [HttpPost("DropDownModules")]
        public async Task<IActionResult> GetDDlModules( string? RefID1, string? RefID2, string? RefID3)
        {
            try
            {
                // var objDDlModel = new GetDropDownDataModel { Mode = Mode, RefID1 = RefID1, RefID2 = RefID2, RefID3 = RefID3, RefID4 = RefID4 };
                string Mode = "MODULEID";
                using (IUowLogin _repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt))
                {
                    var lstuser = await _repo.LoginDALRepo.GetDDlLanguage(Mode, RefID1, RefID2, RefID3);
                    if (lstuser != null)
                    {
                        return Ok(lstuser);
                    }
                    else
                    {
                        return Unauthorized("Invalid User Name.");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Bad Request");
            }
        }
    }
}
