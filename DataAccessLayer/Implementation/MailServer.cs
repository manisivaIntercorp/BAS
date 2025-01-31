using Microsoft.Extensions.Logging;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class MailServer
    {
        #region Page Variables 
        private readonly string _connectionString;
        private readonly ILogger<MailServer> _logger;
        #endregion


        #region Page Properties
        
        public MailServer() { }
        public string SSL_Required
        {
            get;
            set;
        }
        public string SMTP_Port
        {
            get;set;
        }
        public string ReplyEmail
        {
            get;set;
        }
        public Int64 UpdatedBy
        {
            get;set;
        }

        public string SMTP_Address
        {
            get;set;
        }
        public string CreatedDateTime
        {
            get;set;
        }
        public string CreatedBy
        {
            get;set;
        }
        public string CredentialRequired
        {
            get;set;
        }
        public string ModifiedDateTime
        {
            get;set;

        }
        public string Password
        {
            get;set;
        }
        public DataTable MailServerConfigDetail
        {

            get;set;
        }

        public string Active
        {
            get;set;
        }

        public string SetupName
        {
            get;set;
        }

        public string IsDefault
        {
            get;set;
        }

        public string ModifiedBy
        {
            get;set;
        }
        public Int64 ID
        {
            get;set;
        }
        public string TimeZoneID
        {
            get; set;
        }
        public string UserName
        {
            get;set;
        }
        #endregion Page Properties

    }
}
