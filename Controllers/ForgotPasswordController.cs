using Dapper;
using DataAccessLayer.Model;
using DataAccessLayer.Services;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Formats.Asn1;
using System.Net;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ApiBaseController
    {
        private readonly ILogger<ForgotPasswordController> _logger;
        private readonly EmailServices _emailService;
        private readonly SessionService _SessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ForgotPasswordController(EmailServices emailService,SessionService sessionService,
            ILogger<ForgotPasswordController> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(configuration)
        {
            _logger = logger;
            _emailService= emailService;
            _SessionService= sessionService;
            _httpContextAccessor= httpContextAccessor;
        }
        [HttpPost("InsertUpdateForgotPassword")]
        public async Task<IActionResult> InsertUpdateForgotPassword(ForgotPasswordModel objModel)
        {
            try
            {
                if (objModel == null)
                {
                    return BadRequest("Invalid input data.");
                }

                else
                {
                    using (IUowForgotPassword _repo = new UowForgotPassword(_httpContextAccessor))
                    {

                        var result = await _repo.ForgotPasswordDALRepo.InsertUpdateForgotPassword(objModel);
                        _repo.Commit();
                        if(result.forgotPasswordModels != null)
                        {
                            switch (result.RetVal)
                            {
                                case >= 1:// success
                                    
                                    
                                    await _emailService.SendMailMessage(EmailTemplateCode.FORGOT_PASSWORD,
                                                                        Convert.ToInt32(result.RetVal.ToString()),
                                                                        Convert.ToInt64(result.Msg.ToString()),
                                                                        string.Empty);
                                    return Ok();
                                    
                                    
                                case -1:// User Not Exists
                                    return Ok(result.Msg);
                                
                                default:
                                    _logger.LogError(Environment.NewLine);
                                    _logger.LogError("Bad Request occurred while accessing the InsertUpdateRole function in Role API controller");
                                    return BadRequest();
                            }
                        }
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
        //Validating the token from email
        [HttpGet("ValidateTokenForgotPassword/{token}")]
        public async Task<IActionResult> ValidateTokenForgotPassword(string? token)
        {
            try
            {
                
                using (IUowForgotPassword _repo = new UowForgotPassword(_httpContextAccessor))
                {
                    var objvalidatetoken = await _repo.ForgotPasswordDALRepo.ValidateTokenForgotPassword(token);
                    if (objvalidatetoken != null)
                    {
                        _SessionService.SetSession("@UserName", objvalidatetoken.UserName??string.Empty);
                        _SessionService.SetSession("@UserID", Convert.ToString(objvalidatetoken.UserID) ?? "0");
                        _SessionService.SetSession("@RefID",Convert.ToString(objvalidatetoken.ID) ??"0");
                        return Ok(objvalidatetoken);
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
    }
}        