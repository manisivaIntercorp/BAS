using System.Data;

namespace DataAccessLayer.Services
{
    public class MailServer
    {
        IDbTransaction? _transaction;

        private readonly string? connectionString;
        private readonly IDbConnection? _connection;

        IDbConnection? connection = null;
        public MailServer(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Connection string cannot be null or empty.");

            this.connectionString = connectionString;

            // Initialize connection and transaction
            _connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }
        public MailServer() { }
        #region Page Properties
    public string? SSL_Required
        {
            get;
            set;
        }
        public string? SMTP_Port
        {
            get; set;
        }
        public string? ReplyEmail
        {
            get; set;
        }
        public long? UpdatedBy
        {
            get; set;
        }

        public string? SMTP_Address
        {
            get; set;
        }
        public string? CreatedDateTime
        {
            get; set;
        }
        public string? CreatedBy
        {
            get; set;
        }
        public string? CredentialRequired
        {
            get; set;
        }
        public string? ModifiedDateTime
        {
            get; set;

        }
        public string? Password
        {
            get; set;
        }
        public DataTable? MailServerConfigDetail
        {

            get; set;
        }

        public string? Active
        {
            get; set;
        }

        public string? SetupName
        {
            get; set;
        }

        public string? IsDefault
        {
            get; set;
        }

        public string? ModifiedBy
        {
            get; set;
        }
        public long? ID
        {
            get; set;
        }
        public string? TimeZoneID
        {
            get; set;
        }
        public string? UserName
        {
            get; set;
        }
        #endregion Page Properties

    }
}
