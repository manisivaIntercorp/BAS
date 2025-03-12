using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Services;
using WebApi.Services.Interface;


namespace WebApi.Controllers
{
    [Route("api/{region?}/[controller]")]
    [ApiController]
    public class NationalityController : ApiBaseController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;
        private SessionService _sessionService;
        private GUID _guid;
        private readonly ILogger<NationalityController> _logger;
        
        public NationalityController(ILogger<NationalityController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService,GUID gUID,SessionService sessionService) : base(configuration)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _auditLogService = auditLogService;
            _guid = gUID;
            _sessionService = sessionService;
        }
        

        [HttpGet("getAllNationality")]
        public async Task<IActionResult> GetAllNationality()
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if(!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getAllNationality", "");
                        var lstNationalityModel = await _repo.NationalityDALRepo.GetAllNationality();
                        if (lstNationalityModel != null && lstNationalityModel.Count > 0)
                        {
                            return Ok(lstNationalityModel);
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
        [HttpGet("getNationalityByGUId/{GUId}")]
        public async Task<IActionResult> getNationalityByGUId(string GUId)
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "getNationalityByGUId", "");
                        string GuidNationality = await _guid.GetGUIDBasedOnNationality(GUId);
                        if (GuidNationality == GUId)
                        {
                            var objNationalityModel = await _repo.NationalityDALRepo.GetNationalityByGUId(GUId);
                            if (objNationalityModel != null)
                            {
                                return Ok(objNationalityModel);
                            }
                            else
                            {
                                return BadRequest(Common.Messages.NoRecordsFound);
                            }
                        }
                        else
                        {
                            return BadRequest("Please Check Nationality GUID");
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
        [HttpPost("insertNationality")]
        public async Task<IActionResult> InsertNationality(NationalityModel objModel)
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "insertNationality", "");
                        objModel.CreatedBy = userId;
                        var result = await _repo.NationalityDALRepo.InsertUpdateNationality(objModel);
                        _repo.Commit();
                        if (result.Insertnationality == true || result.Insertnationality == false)
                        {
                            switch (result.RetVal)
                            {
                                case 1://Success
                                    return Ok(result.Msg);
                                case -2:// Failure in Nationality Name
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
            catch (Exception ex )
            {
                _logger.LogError(ex.Message + "  " + ex.StackTrace);
                throw;
            }
        }

        [HttpPut("UpdateNationality")]
        public async Task<IActionResult> UpdateNationality(UpdateNationality objModel)
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        objModel.CreatedBy = userId;
                        await _auditLogService.LogAction("", "UpdateNationality", "");
                        var result = await _repo.NationalityDALRepo.UpdateNationality(objModel);
                        _repo.Commit();
                        if (result.Updatenationality == true || result.Updatenationality == false)
                        {
                            switch (result.RetVal)
                            {
                                case 1://Success
                                    return Ok(result.Msg);
                                case -2:// Failure in Nationality Name
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
        [HttpDelete("deleteNationality")]
        public async Task<IActionResult> DeleteNationality(DeleteNationality deleteNationality)
        {
            try
            {
                using (IUowNationality _repo = new UowNationality(_httpContextAccessor))
                {
                    string userIdStr = _sessionService.GetSession(Common.SessionVariables.UserID);
                    long userId = !string.IsNullOrEmpty(userIdStr) ? Convert.ToInt64(userIdStr) : 0;
                    string response = _sessionService.GetSession(Common.SessionVariables.Guid);
                    if (!string.IsNullOrEmpty(response))
                    {
                        await _auditLogService.LogAction("", "deleteNationality", "");
                        foreach (var UserGuid in deleteNationality.DeleteDataTable)
                        {
                            var GuidResp = await _guid.GetGUIDBasedOnNationality(UserGuid.NationalityGuid);
                            if (GuidResp == UserGuid.NationalityGuid)
                            {
                                DataTable dataTable = deleteNationality.ConvertToDataTable(deleteNationality.DeleteDataTable);
                                var result = await _repo.NationalityDALRepo.DeleteNationality(userId, deleteNationality);
                                _repo.Commit();
                                if (result.deletenationality==true || result.deletenationality==false)
                                {
                                    return Ok(result.deleteResults);
                                }
                                else
                                {
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the DeleteNationality function in Nationality api controller");
                                    return BadRequest();
                                }
                            }
                            else
                            {
                                return BadRequest("Please Check Nationality GUID");
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
