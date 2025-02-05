using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using System.Data;
using WebApi.Controllers;
using MailKit.Net.Smtp;
using MimeKit;
using DataAccessLayer.Services;

namespace WebApi.Services
{
    public class EmailServices : ApiBaseController
    {
        private readonly ILogger<EmailServices> _logger;
        private SessionService _SessionService;
        public EmailServices(ILogger<EmailServices> logger, SessionService sessionService, IConfiguration configuration) : base(configuration)
        {
            _logger = logger;
            _SessionService = sessionService;
        }
        public async Task SendMailMessage(string? TemplateCode, int? RefID, int? UserID, string? Password, ForgotPasswordRequest? forgotPasswordRequest = null)
        {
            
            try
            {
                forgotPasswordRequest ??= new ForgotPasswordRequest
                {
                    ForgotPasswordModel = new ForgotPasswordModel(), // Prevents null reference
                    _emailrepository = new EmailTemplate() // Ensures _emailrepository is not null
                };
                string StaffID = string.Empty;
                string StrURL = string.Empty;
                
                // Ensure forgotPasswordRequest._emailrepository is populated before usage
                forgotPasswordRequest._emailrepository.TemplateCode = TemplateCode;
                forgotPasswordRequest._emailrepository.UserID = UserID;
                forgotPasswordRequest._emailrepository.RefID1 = RefID;
                forgotPasswordRequest._emailrepository.EmployeeID = Password;

                using (IUowEmailTemplate _repo = new UowEmailTemplate(ConnectionString))
                {
                    List<GetEmailTemplate?> lstEmailTemplate = _repo.EmailTemplateDALRepo.GetEmailTemplate(forgotPasswordRequest._emailrepository);
                    _repo.Commit();
                    // Ensure this async method is awaited
                    DataTable dtEmailTemplate = forgotPasswordRequest.ForgotPasswordModel.ConvertToDataTableAsync(lstEmailTemplate??new List<GetEmailTemplate?>());
                    // Iterate over the data table
                    foreach (DataRow row in dtEmailTemplate.Rows)
                    {
                        try
                        {
                            string EmailID = row["EmailID"] as string ?? string.Empty;  // Fetch EmailID from the DataTable

                            if (!string.IsNullOrEmpty(EmailID))  // Ensure EmailID is not empty
                            {
                                string Template = row["Template"] as string ?? string.Empty;
                                string Subject = row["Subject"] as string ?? string.Empty;
                                string MobileContent = row["MobileContent"] as string ?? string.Empty;
                                string ApproverID = string.Empty; // Ensure this is populated if needed
                                string EmployeeID = string.Empty;

                                // Replace placeholders in the template
                                Template = Template.Replace("[@password]", Password);

                                // Send email with the updated template
                                ForgotPasswordModel objModel = new ForgotPasswordModel();
                                await SendEmail(EmailID, Subject, Template, MobileContent,
                                                string.IsNullOrEmpty(ApproverID) ? Convert.ToString(UserID) : ApproverID,
                                                TemplateCode, objModel);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error processing email template: {ex.Message} - {ex.StackTrace}");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _logger.LogError($"Error in SendMailMessageAsync: {ex.Message} - {ex.StackTrace}");
            }
            
        }

        public async Task SendEmail(string? EmailID, string? Subject, string? Template, string? MobileContent, string? ApproverID, string? Templatecode, ForgotPasswordModel forgotPasswordModel)
        {
            
            MailServer? objMailServerConfig = new MailServer();

            objMailServerConfig.ID = 1;
            objMailServerConfig.TimeZoneID = _SessionService.GetSession("@TimeZoneID");
            using (IUowEmailTemplate _repo = new UowEmailTemplate(ConnectionString))
            {
                List<MailServer?> lstMailServerConfig = _repo.EmailTemplateDALRepo.GetMailServerConfig(objMailServerConfig ?? new MailServer());
                _repo.Commit();
                DataTable dtMailServerConfig = forgotPasswordModel.ConvertToDataTableAsync(lstMailServerConfig?? new List<MailServer?>());
                if (dtMailServerConfig.Rows.Count > 0)
                {
                    string FromEmail = dtMailServerConfig.Rows[0]["ReplyEmail"] as string?? string.Empty;
                    string SmtpServer = dtMailServerConfig.Rows[0]["SMTP_Address"] as string ?? string.Empty;
                    int SmtpPort = dtMailServerConfig.Rows[0]["SMTP_Port"] is not DBNull ? Convert.ToInt32(dtMailServerConfig.Rows[0]["SMTP_Port"]):0 ;
                    bool SmtpSSL = Convert.ToBoolean(dtMailServerConfig.Rows[0]["SSL_Required"].Equals("Y") ? true : false);
                    bool SmtpAuthentication = string.Equals(dtMailServerConfig.Rows[0]["CredentialRequired"]?.ToString(), "Y", StringComparison.OrdinalIgnoreCase);
                    string SmtpAuthenticationID = dtMailServerConfig.Rows[0]["UserName"] as string ?? string.Empty;
                    string SmtpAuthenticationPwd = dtMailServerConfig.Rows[0]["Password"] as string ?? string.Empty;

                    
                    // Using MimeKit to create the email message
                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("", FromEmail));
                    emailMessage.To.Add(new MailboxAddress("", EmailID));
                    emailMessage.Subject = Subject;
                    var bodyBuilder = new BodyBuilder
                    {
                        HtmlBody = Template // Use HTML for email body
                    };
                    emailMessage.Body = bodyBuilder.ToMessageBody();
                    try
                    {
                        using (var smtp = new SmtpClient())
                        {
                            await smtp.ConnectAsync(SmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls); // Use async
                            if (SmtpAuthentication)
                            {
                                await smtp.AuthenticateAsync(SmtpAuthenticationID, SmtpAuthenticationPwd); // Use async
                            }
                            await smtp.SendAsync(emailMessage); // Use async
                            await smtp.DisconnectAsync(true); // Disconnect async
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error sending email: {ex.Message}");
                    }
                }
            }
            
        }
    }
}
