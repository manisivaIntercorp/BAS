using DataAccessLayer.Services;

namespace DataAccessLayer.Interface
{
    public interface IEmailTemplate
    {
        List<GetEmailTemplate?> GetEmailTemplate(EmailTemplate? emailTemplate);
        List<MailServer?> GetMailServerConfig(MailServer? mailServer);
    }

}
