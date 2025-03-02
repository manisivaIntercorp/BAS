using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services;
using WebApi.Services.Interface;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailServerController : ApiBaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;
        private SessionService _sessionService;
        private GUID _guid;
        private readonly ILogger<MailServerController> _logger;
        public MailServerController(ILogger<MailServerController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService, GUID gUID, SessionService sessionService) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _auditLogService = auditLogService;
            _guid = gUID;
            _sessionService = sessionService;
        }
        [HttpGet("getAllMailServer")]
        public async Task<IActionResult> getAllMailServer()
        {
            try
            {
                using (IUowMailServer _repo = new UowMailServer(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllMailServer", "");
                        var lstMailServerModel = await _repo.MailServerDALRepo.GetAllMailServer();
                        if (lstMailServerModel != null && lstMailServerModel.Count > 0)
                        {
                            return Ok(lstMailServerModel);
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
        [HttpGet("getMailServerByGUId/{GUId}")]
        public async Task<IActionResult> getMailServerByGUId(string GUId)
        {
            try
            {
                using (IUowMailServer _repo = new UowMailServer(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getMailServerByGUId", "");
                        string GuidMailServer = await _guid.GetGUIDBasedOnMailServer(GUId);
                        if (GuidMailServer == GUId)
                        {
                            var objMailServerModel = await _repo.MailServerDALRepo.GetMailServerByGUId(GUId);
                            if (objMailServerModel != null)
                            {
                                return Ok(objMailServerModel);
                            }
                            else
                            {
                                return BadRequest(Common.Messages.NoRecordsFound);
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Mail Server GUID");
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
        [HttpPost("insertMailServer")]
        public async Task<IActionResult> InsertMailServer(MailServerModel objModel)
        {
            try
            {
                using (IUowMailServer _repo = new UowMailServer(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "insertMailServer", "");
                        objModel.CreatedBy = userId;
                        var result = await _repo.MailServerDALRepo.InsertUpdateMailServer(objModel);
                        _repo.Commit();
                        if (result.Insertmailserver == true || result.Insertmailserver == false)
                        {
                            switch (result.RetVal)
                            {
                                case 1://Success
                                    return Ok(result.Msg);
                                
                                case -1:// Failure in Nationality Name
                                    return Ok(result.Msg);
                                default:
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateNationality function in Nationality api controller");
                                    return BadRequest();
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

        [HttpPut("UpdateMailServer")]
        public async Task<IActionResult> UpdateMailServer(UpdateMailServerModel objModel)
        {
            try
            {
                using (IUowMailServer _repo = new UowMailServer(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        objModel.CreatedBy = userId;
                        await _auditLogService.LogAction("", "UpdateMailServer", "");
                        var result = await _repo.MailServerDALRepo.UpdateMailServer(objModel);
                        _repo.Commit();
                        if (result.Updatemailserver == true || result.Updatemailserver == false)
                        {
                            switch (result.RetVal)
                            {
                                case 1://Success
                                    return Ok(result.Msg);
                                
                                case -1:// Failure in Nationality Name
                                    return Ok(result.Msg);
                                default:
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateNationality function in Nationality api controller");
                                    return BadRequest();
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
        [HttpDelete("deleteMailServer")]
        public async Task<IActionResult> DeleteMailServer(DeleteMailServer deleteMailServer)
        {
            try
            {
                using (IUowMailServer _repo = new UowMailServer(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "deleteMailServer", "");
                        foreach (var UserGuid in deleteMailServer.DeleteDataTable)
                        {
                            var GuidResp = await _guid.GetGUIDBasedOnMailServer(UserGuid.MailServerGuid);
                            if (GuidResp == UserGuid.MailServerGuid)
                            {
                                DataTable dataTable = deleteMailServer.ConvertToDataTable(deleteMailServer.DeleteDataTable);
                                var result = await _repo.MailServerDALRepo.DeleteMailServer(userId, deleteMailServer);
                                _repo.Commit();
                                if (result.deletemailserver == true || result.deletemailserver == false)
                                {
                                    return Ok(result.deleteResults);
                                }
                                else
                                {
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the Delete MailServer function in Nationality api controller");
                                    return BadRequest();
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check Mail Server GUID");
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
