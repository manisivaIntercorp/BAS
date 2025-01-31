using DataAccessLayer.Implementation;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using System.Data;
using WebApi.Controllers;
using MailKit.Net.Smtp;
using MimeKit;

namespace WebApi.Services
{
    public class EmailServices : ApiBaseController
    {
        private readonly ILogger<EmailServices> _logger;
        public EmailServices(IConfiguration configuration) : base(configuration)
        {
        }
        public EmailServices(ILogger<EmailServices> logger, IConfiguration configuration) : base(configuration)
        {
            _logger = logger;
        }
        public async Task SendMailMessage(string TemplateCode, int RefID, int UserID, string Password, ForgotPasswordRequest forgotPasswordRequest=null)
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
                    List<GetEmailTemplate> lstEmailTemplate = _repo.EmailTemplateDALRepo.GetEmailTemplate(forgotPasswordRequest._emailrepository);
                    _repo.Commit();
                    // Ensure this async method is awaited
                    DataTable dtEmailTemplate = forgotPasswordRequest.ForgotPasswordModel.ConvertToDataTableAsync(lstEmailTemplate);

                    // Iterate over the data table
                    for (int i = 0; i < dtEmailTemplate.Rows.Count; i++)
                    {
                        try
                        {
                            string EmailID = dtEmailTemplate.Rows[i]["EmailID"].ToString().Trim();  // Fetch EmailID from the DataTable

                            if (!string.IsNullOrEmpty(EmailID))  // Ensure EmailID is not empty
                            {
                                string Template = dtEmailTemplate.Rows[i]["Template"].ToString();
                                string Subject = dtEmailTemplate.Rows[i]["Subject"].ToString();
                                string MobileContent = dtEmailTemplate.Rows[i]["MobileContent"].ToString();
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

        public async Task SendEmail(string EmailID, string Subject, string Template, string MobileContent, string ApproverID, string Templatecode, ForgotPasswordModel forgotPasswordModel)
        {
            
            MailServer objMailServerConfig = new MailServer();

            objMailServerConfig.ID = 1;
            using (IUowEmailTemplate _repo = new UowEmailTemplate(ConnectionString))
            {
                List<MailServer> lstMailServerConfig = _repo.EmailTemplateDALRepo.GetMailServerConfig(objMailServerConfig);
                _repo.Commit();
                DataTable dtMailServerConfig = forgotPasswordModel.ConvertToDataTableAsync(lstMailServerConfig);
                if (dtMailServerConfig.Rows.Count > 0)
                {
                    string FromEmail = Convert.ToString(dtMailServerConfig.Rows[0]["ReplyEmail"]);
                    string SmtpServer = Convert.ToString(dtMailServerConfig.Rows[0]["SMTP_Address"]);
                    int SmtpPort = Convert.ToInt32(Convert.ToString(dtMailServerConfig.Rows[0]["SMTP_Port"]));
                    bool SmtpSSL = Convert.ToBoolean(Convert.ToString(dtMailServerConfig.Rows[0]["SSL_Required"]).Equals("Y") ? true : false);
                    bool SmtpAuthentication = Convert.ToBoolean(Convert.ToString(dtMailServerConfig.Rows[0]["CredentialRequired"]).Equals("Y") ? true : false);
                    string SmtpAuthenticationID = Convert.ToString(dtMailServerConfig.Rows[0]["UserName"]);
                    string SmtpAuthenticationPwd = Convert.ToString(dtMailServerConfig.Rows[0]["Password"]);

                    
                    // Using MimeKit to create the email message
                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("My App", FromEmail));
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
