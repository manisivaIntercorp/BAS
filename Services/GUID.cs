using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;

using WebApi.Controllers;

namespace WebApi.Services
{
    public class GUID : ApiBaseController
    {
        private readonly ILogger<GUID> _logger;
        private readonly IConfiguration _configuration;
        IHttpContextAccessor _httpContextAccessor;

        public GUID(ILogger<GUID> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetGUIDBasedOnUserGuid(string? UpdatedGuidBy)
        {
            string response = string.Empty;
            try
            {
                
                using (IUowGUID _repo = new UowGUID(_httpContextAccessor))
                {
                    var result = await _repo.GUIDDALRepo.GetGUID(UpdatedGuidBy);
                    
                    _repo.Commit();
                    if (result.GetGuid==true || result.GetGuid==false)
                    {
                        switch (result.RetVal) 
                        {
                            case 1:// Success
                                response =result.Msg;
                                break;
                            case 0:// Failure
                                response = result.Msg;
                            break;

                            default:
                                response = "Invalid User";
                                break;
                                
                        }
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError($"Error processing GUID: {ex.Message} - {ex.StackTrace}");
            }
            return response;
        }

        public async Task<string> GetGUIDBasedOnUserAccountRoleGuid(string? UpdatedGuidBy)
        {
            string response = string.Empty;
            try
            {

                using (IUowGUID _repo = new UowGUID(_httpContextAccessor))
                {
                    var result = await _repo.GUIDDALRepo.GetGUIDBasedOnUserAccountRoleGuid(UpdatedGuidBy);

                    _repo.Commit();
                    if (result.GetGuid == true || result.GetGuid == false)
                    {
                        switch (result.RetVal)
                        {
                            case 1:// Success
                                response = result.Msg;
                                break;
                            case 0:// Failure
                                response = result.Msg;
                                break;

                            default:
                                response = "Invalid User";
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing GUID: {ex.Message} - {ex.StackTrace}");
            }
            return response;
        }

        public async Task<string> GetGUIDBasedOnUserRoleGuid(string? UpdatedGuidBy)
        {
            string response = string.Empty;
            try
            {

                using (IUowGUID _repo = new UowGUID(_httpContextAccessor))
                {
                    var result = await _repo.GUIDDALRepo.GetGUIDBasedOnRoleGuid(UpdatedGuidBy);

                    _repo.Commit();
                    if (result.GetGuid == true || result.GetGuid == false)
                    {
                        switch (result.RetVal)
                        {
                            case 1:// Success
                                response = result.Msg;
                                break;
                            case 0:// Failure
                                response = result.Msg;
                                break;

                            default:
                                response = "Invalid User";
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing GUID: {ex.Message} - {ex.StackTrace}");
            }
            return response;
        }

        public async Task<string> GetGUIDBasedOnOrgName(string? UpdatedGuidBy)
        {
            string response = string.Empty;
            try
            {

                using (IUowGUID _repo = new UowGUID(_httpContextAccessor))
                {
                    var result = await _repo.GUIDDALRepo.GetGUIDBasedOnOrgName(UpdatedGuidBy);

                    _repo.Commit();
                    if (result.GetGuid == true || result.GetGuid == false)
                    {
                        switch (result.RetVal)
                        {
                            case 1:// Success
                                response = result.Msg;
                                break;
                            case 0:// Failure
                                response = result.Msg;
                                break;

                            default:
                                response = "Invalid User";
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing GUID: {ex.Message} - {ex.StackTrace}");
            }
            return response;
        }

        public async Task<string> GetGUIDBasedOnUserPolicy(string? UpdatedGuidBy)
        {
            string response = string.Empty;
            try
            {

                using (IUowGUID _repo = new UowGUID(_httpContextAccessor))
                {
                    var result = await _repo.GUIDDALRepo.GetGUIDBasedOnUserPolicy(UpdatedGuidBy);

                    _repo.Commit();
                    if (result.GetGuid == true || result.GetGuid == false)
                    {
                        switch (result.RetVal)
                        {
                            case 1:// Success
                                response = result.Msg;
                                break;
                            case 0:// Failure
                                response = result.Msg;
                                break;

                            default:
                                response = "Invalid User";
                                break;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing GUID: {ex.Message} - {ex.StackTrace}");
            }
            return response;
        }

    }
}
    

