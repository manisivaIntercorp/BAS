using DataAccessLayer.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IEmailTemplate
    {
        List<GetEmailTemplate> GetEmailTemplate(EmailTemplate emailTemplate);
        List<MailServer> GetMailServerConfig(MailServer mailServer);
    }

}
