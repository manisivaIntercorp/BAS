using Dapper;
using DataAccessLayer.Implementation;
using DataAccessLayer.Model;
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

        
        public ForgotPasswordController(EmailServices emailService,SessionService sessionService,
            ILogger<ForgotPasswordController> logger, IConfiguration configuration) : base(configuration)
        {
            _logger = logger;
            _emailService= emailService;
            _SessionService= sessionService;
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
                    using (IUowForgotPassword _repo = new UowForgotPassword(ConnectionString))
                    {

                        var result = await _repo.ForgotPasswordDALRepo.InsertUpdateForgotPassword(objModel);
                        _repo.Commit();
                        if(result.forgotPasswordModels != null)
                        {
                            switch (result.RetVal)
                            {
                                case >= 1:// success
                                    
                                    var mailserver = _repo.ForgotPasswordDALRepo.mailServerport();

                                    _SessionService.SetSession("TimeZoneID","1");
                                    mailserver.TimeZoneID = _SessionService.GetSession("TimeZoneID");
                                    await _emailService.SendMailMessage(EmailTemplateCode.FORGOT_PASSWORD,
                                                      Convert.ToInt32(result.RetVal.ToString()),
                                    Convert.ToInt32(result.Msg.ToString()),
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
    }
}        