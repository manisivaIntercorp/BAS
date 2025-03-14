using DataAccessLayer.Model;
using DataAccessLayer.Services;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Agent;
using Microsoft.SqlServer.Management.XEvent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;
//using WebApi.Resources;
using WebApi.Services;
using WebApi.Services.Implementation;
using WebApi.Services.Interface;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


namespace WebApi.Controllers
{

    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/{region?}/[controller]")] // {region} is optional
    public class LoginController : ApiBaseController
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtService _jwtService;
        //private readonly AppGlobalVariableService _appGlobalVariableService;
        private readonly EncryptedDecrypt _encryptedDecrypt;
        private readonly IAuditLogService _auditLogService;
        //private readonly IStringLocalizer _localizer;
        private readonly TranslationService _translationService;
        string token = string.Empty;
        string userGuid = string.Empty;
       // private readonly IServiceUrlProvider _serviceUrlProvider;

        /// <summary>
        /// private readonly ICommon _common;
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="encryptedDecrypt"></param>
        /// <param name="jwtService"></param>
        /// <param name="configuration"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="auditLogService"></param>
        /// <param name="localizer"></param>
        /// <param name="translationService"></param>

        public LoginController(ILogger<LoginController> logger, EncryptedDecrypt encryptedDecrypt,
        JwtService jwtService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor,
        IAuditLogService auditLogService, TranslationService translationService) //IStringLocalizer<SharedResources> localizer)
        : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;  // Now injected
            //_appGlobalVariableService = appGlobalVariableService;
            _encryptedDecrypt = encryptedDecrypt;
            _auditLogService = auditLogService;
            //_localizer = localizer;
            _translationService = translationService;
            //_serviceUrlProvider = serviceUrlProvider;
            // _common = common;
        }

        [HttpPost("Get UserID")]
        public async Task<IActionResult> GetUserID(string username, string? Language)
        {
            try
            {
                var objLogModel = new LoginModel { UserName = username, LanguageCode = Language };


                using (IUowLogin _repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt))
                {
                    var lstuser = await _repo.LoginDALRepo.GetUserID(objLogModel);

                    if (lstuser == null || lstuser.Count == 0)
                    {
                        return Unauthorized("Invalid User Name.");
                    }
                    await _auditLogService.LogAction("", "LOGIN-UserName", "");
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
                if (HttpContext?.Session?.GetString("Guid") != null)
                {
                    strGuid = Convert.ToString(HttpContext?.Session?.GetString("Guid") ?? "");
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
                            var vGuid = "";
                            if (loginDetail != null)
                            {


                                HttpContext?.Session?.SetString(DataAccessLayer.Model.Common.SessionVariables.UserName, objLogModel.UserName);
                                HttpContext?.Session?.SetString(
                                    DataAccessLayer.Model.Common.SessionVariables.Password,
                                    EncryptShaAlg.Encrypt((objLogModel?.Password ?? "") + strGuid) ?? "");
                                //HttpContext?.Session?.SetString(DataAccessLayer.Model.Common.SessionVariables.Password, EncryptShaAlg.Encrypt(objLogModel?.Password + strGuid));
                                GetUserData(lstLoginUser, "GET");
                                vGuid = loginDetail?.Guid;
                            }
                            var token = _jwtService.GenerateToken(objLogModel?.UserName ?? "", Convert.ToString(vGuid) ?? "", objLogModel?.Password ?? "");
                            var result = LogLoginDetails("LOGIN", vGuid?.ToString() ?? "", token);

                            HttpContext?.Session?.SetString(Common.SessionVariables.Token, token);
                            await _auditLogService.LogAction(strGuid ?? "", "LOGIN", token);

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
                        string strRegionCode = lstUserDetails[0]?.RegionCode?.Trim() ?? string.Empty;
                        

                        HttpContext.Session.SetString(Common.SessionVariables.UserID, strUserID);
                        HttpContext.Session.SetString(Common.SessionVariables.UserName, strUserName);
                        HttpContext.Session.SetString(Common.SessionVariables.UserDisplayName, strUserDisplayName);
                        HttpContext.Session.SetString(Common.SessionVariables.UserRoleID, strUserRoleID);
                        HttpContext.Session.SetString(Common.SessionVariables.UserRoleName, strUserRoleName);
                        HttpContext.Session.SetString(Common.SessionVariables.UserRoleType, strUserRoleType);
                        HttpContext.Session.SetString(Common.SessionVariables.DisplayPDPA, strDisplayPDPA);
                        HttpContext.Session.SetString(Common.SessionVariables.PayrollAccessible, strPayrollAccessible);
                        HttpContext.Session.SetString(Common.SessionVariables.UserAccessRightsCount, strUserAccessRightsCount);
                        HttpContext.Session.SetString(Common.SessionVariables.SetPassword, strSetPassword);
                        HttpContext.Session.SetString(Common.SessionVariables.PasswordExpiry, strPasswordExpiry);
                        HttpContext.Session.SetString(Common.SessionVariables.ValidateOTP, strValidateOTP);
                        HttpContext.Session.SetString(Common.SessionVariables.IsProfileUser, strIsProfileUser);
                        HttpContext.Session.SetString(Common.SessionVariables.IsSystemUser, strIsSystemUser);
                        HttpContext.Session.SetString(Common.SessionVariables.UserImgPath, strUserImgPath);
                        HttpContext.Session.SetString(Common.SessionVariables.Guid, strGuid);
                        HttpContext.Session.SetString(Common.SessionVariables.RegionCode, strRegionCode);
                    }

                    if (string.IsNullOrWhiteSpace(strDBName))
                    {
                        HttpContext.Session.SetString(Common.SessionVariables.DBName, strDBName);
                        HttpContext.Session.SetString(Common.SessionVariables.DBChange, "N");
                        HttpContext.Session.SetString(Common.SessionVariables.GlobalUser, "Y");
                        HttpContext.Session.SetString(Common.SessionVariables.InstanceChange, "N");
                        HttpContext.Session.SetString(Common.SessionVariables.InstanceName, string.Empty);
                        HttpContext.Session.SetString(Common.SessionVariables.DataBaseUserName, string.Empty);
                        HttpContext.Session.SetString(Common.SessionVariables.DataBasePassword, string.Empty);


                    }
                    else
                    {
                        HttpContext.Session.SetString(Common.SessionVariables.DBName, strDBName);
                        HttpContext.Session.SetString(Common.SessionVariables.DBChange, "Y");
                        HttpContext.Session.SetString(Common.SessionVariables.GlobalUser, "N");
                        if (lstUserDetails != null && lstUserDetails.Count > 0)
                        {
                            HttpContext.Session.SetString(Common.SessionVariables.TimeOffset, Convert.ToString(lstUserDetails[0]?.TimeOffset?.Trim() ?? string.Empty));
                            HttpContext.Session.SetString(Common.SessionVariables.TimeZoneID, Convert.ToString(lstUserDetails[0]?.TimeZoneID?.Trim() ?? string.Empty));
                            HttpContext.Session.SetString(Common.SessionVariables.IsAppuserTimeZone, Convert.ToString(lstUserDetails[0]?.IsAppuserTimeZone?.Trim() ?? string.Empty));
                        }


                    }

                    if (strMode == "GET" && lstUserDetails != null)
                    {
                        if (!String.IsNullOrWhiteSpace(Convert.ToString(lstUserDetails[0]?.IsProfileUser?.Trim() ?? string.Empty)))
                        {
                            HttpContext.Session.SetString(Common.SessionVariables.IsProfileUser, Convert.ToString(lstUserDetails[0]?.IsProfileUser?.Trim() ?? string.Empty));
                        }
                        else if (!String.IsNullOrWhiteSpace(Convert.ToString(lstUserDetails[0]?.IsSystemUser?.Trim() ?? string.Empty)))
                        {

                            HttpContext.Session.SetString(Common.SessionVariables.IsSystemUser, Convert.ToString(lstUserDetails[0]?.IsSystemUser?.Trim() ?? string.Empty));
                        }
                    }
                    else if (strMode == "" && lstUserDetails != null)
                    {
                        if (!String.IsNullOrWhiteSpace(Convert.ToString(lstUserDetails[0]?.LanguageCode?.Trim() ?? string.Empty)))
                        {
                            HttpContext.Session.SetString(Common.SessionVariables.LanguageCode, Convert.ToString(lstUserDetails[0]?.LanguageCode?.Trim() ?? string.Empty));
                        }
                    }
                }
            }
        }

        [HttpPost("GetOrganisationWithDBDetails")]
        public async Task<IActionResult> GetOrganisationWithDBDetails(string Guid)
        {
            var objLogModel = new LoginModel { Guid = Guid };
            try
            {
               

                using (IUowLogin _repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt))
                {
                    var lstOrgDetails = await _repo.LoginDALRepo.GetOrganisationWithDBDetails(objLogModel);
                    await _auditLogService.LogAction(userGuid, "GetOrganisationWithDBDetails", token);
                    if (lstOrgDetails != null)
                    {
                        // Serialize the list to a JSON string
                        var orgDetailsJson = System.Text.Json.JsonSerializer.Serialize(lstOrgDetails);
                        HttpContext?.Session.SetString(Common.SessionVariables.OrgDetails, orgDetailsJson);
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
        public async Task<IActionResult> SelectedOrganisation(string? Guid)
        {
            if (String.IsNullOrEmpty(Guid))
                return BadRequest("Invalid Organisation ID.");
            try
            {

                if (HttpContext?.Session != null)
                {
                    HttpContext.Session.TryGetValue(Common.SessionVariables.Token, out var tokenBytes);
                    HttpContext.Session.TryGetValue(Common.SessionVariables.Guid, out var guidBytes);

                    string token = tokenBytes != null ? System.Text.Encoding.UTF8.GetString(tokenBytes) : string.Empty;
                    string userGuid = guidBytes != null ? System.Text.Encoding.UTF8.GetString(guidBytes) : string.Empty;

                    await _auditLogService.LogAction(userGuid, "SelectedOrganisation", token);
                }

                var dbDetailsJson = HttpContext?.Session.GetString(Common.SessionVariables.OrgDetails);
                if (string.IsNullOrWhiteSpace(dbDetailsJson))
                    return NotFound("Organisation details not found.");

                var dbDetails = JsonConvert.DeserializeObject<DataTable>(dbDetailsJson);
                var dbRow = dbDetails?.AsEnumerable()
                    .FirstOrDefault(row => row.Field<string>("Guid") == Guid);

                if (dbRow == null)
                {
                    return NotFound($"No organisation found with ID: {Guid}.");
                }
                else
                {
                    HttpContext?.Session.SetString(Common.SessionVariables.DBChange, "Y");
                    HttpContext?.Session.SetString(Common.SessionVariables.InstanceChange, "Y");
                    if (dbRow != null)
                    {
                        HttpContext?.Session.SetString(Common.SessionVariables.DBName, dbRow?.Field<string>(TableVariables.Organisation.DBName) ?? "");
                        HttpContext?.Session.SetString(Common.SessionVariables.InstanceName, dbRow?.Field<string>(TableVariables.Organisation.InstanceName) ?? "");
                        HttpContext?.Session.SetString(Common.SessionVariables.DataBaseUserName, dbRow?.Field<string>(TableVariables.Organisation.conUserName) ?? "");
                        HttpContext?.Session.SetString(Common.SessionVariables.DataBasePassword, dbRow?.Field<string>(TableVariables.Organisation.conPassword) ?? "");
                    }
                }



                var loginModel = new LoginModel
                {
                    UserName = HttpContext?.Session.GetString(Common.SessionVariables.UserName),
                    Password = HttpContext?.Session.GetString(Common.SessionVariables.Password),
                    GlobalUser = HttpContext?.Session.GetString(Common.SessionVariables.GlobalUser)
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
                    var token = _jwtService.GenerateToken(loginModel?.UserName ?? "", Convert.ToString(vGuid) ?? "", loginModel?.Password ?? "");

                    return Ok(token);
                }

                return Unauthorized("Invalid login.");
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogError($"{ex.Message} {ex.StackTrace}");
                return StatusCode(500, "A data reader was accessed after disposal.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message} {ex.StackTrace}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //}
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Token is required for logout.");
                }

                _jwtService.InvalidateToken(token);

                if (HttpContext?.Session != null)
                {
                    var vGuid = HttpContext.Session.GetString(Common.SessionVariables.Guid);
                    var vtoken = HttpContext.Session.GetString(Common.SessionVariables.Token);
                    var result = LogLoginDetails("LOGOUT", vGuid?.ToString() ?? "", vtoken?.ToString() ?? "");
                }

                await _auditLogService.LogAction(userGuid ?? "", "SelectedOrganisation", token ?? "");

                HttpContext?.Session.Clear();
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
                    var lstuser = await _repo.LoginDALRepo.GetDDlLanguage(Mode, RefID1 ?? "", RefID2 ?? "", RefID3 ?? "");
                    await _auditLogService.LogAction(userGuid, "GetDDlLanguage", token);

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
                _logger.LogError($"{ex.Message} {ex.StackTrace}");
                return BadRequest("Bad Request");
            }
        }

        [HttpPost("DropDownModules")]
        public async Task<IActionResult> GetDDlModules(string? RefID1, string? RefID2, string? RefID3)
        {
            try
            {
                string Mode = "MODULEID";
                using (IUowLogin _repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt))
                {
                    var lstuser = await _repo.LoginDALRepo.GetDDlLanguage(Mode, RefID1 ?? "", RefID2 ?? "", RefID3 ?? "");
                    await _auditLogService.LogAction(userGuid, "GetAllOrganisaion", token);
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
                _logger.LogError($"{ex.Message} {ex.StackTrace}");
                return BadRequest("Bad Request");
            }
        }

        private async Task LogLoginDetails(string Mode, string userGuid, string token)
        {
            try
            {
                var loginDetails = new LoginDetails
                {
                    //UserId = userId,
                    Mode = Mode,
                    UserGuid = userGuid,
                    Token = token,
                    LoginAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // Example expiration time
                                                             //LogOut = false,
                    IPAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                    DeviceInfo = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString(),
                    CreatedBy = userGuid,
                    CreatedDateTime = DateTime.UtcNow,
                    UTCCreatedDateTime = DateTime.UtcNow
                };

                using (IUowLogin _repo = new UowLogin(_httpContextAccessor, _configuration, _encryptedDecrypt))
                {
                    var reslt = await _repo.LoginDALRepo.InsertLoginDetails(loginDetails);
                }
           
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
}

    }
}
