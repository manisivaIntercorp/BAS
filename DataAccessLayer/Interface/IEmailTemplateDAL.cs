using DataAccessLayer.Services;

namespace DataAccessLayer.Interface
{
    public interface IEmailTemplateDAL
    {
        List<GetEmailTemplate?> GetEmailTemplate(EmailTemplate? emailTemplate);
        List<MailServer?> GetMailServerConfig(MailServer? mailServer);
    }

}
