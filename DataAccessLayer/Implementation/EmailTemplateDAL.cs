using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Services;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Implementation
{
    public class EmailTemplateDAL:RepositoryBase, IEmailTemplateDAL
    {
        private readonly IDbTransaction? _transaction;
        private readonly string? _connectionString;
        public EmailTemplateDAL(IDbTransaction? transaction, string? connectionString) : base(transaction)
        {
            _connectionString=connectionString;
            _transaction= transaction;
        }

        public List<GetEmailTemplate?> GetEmailTemplate(EmailTemplate? _emailrepository)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@TemplateCode", _emailrepository?.TemplateCode);
                parameters.Add("@UserID", _emailrepository?.UserID);
                parameters.Add("@RefID", _emailrepository?.RefID1);
                parameters.Add("@Month", _emailrepository?.Month);
                parameters.Add("@Year", _emailrepository?.Year);
                parameters.Add("@EmployeeID", _emailrepository?.EmployeeID);
                parameters.Add("@Generalpassphrase", new Cryptology().Generalpassphrase.ToString());
                var result = connection.QueryMultiple(
                              "sp_EmailTemplate",
                              parameters,
                              commandType: CommandType.StoredProcedure
                          );
                return result.Read<GetEmailTemplate?>().ToList();
            }
        }
        public List<MailServer?> GetMailServerConfig(MailServer? objMailServerConfig)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                DynamicParameters parameters = new DynamicParameters();

                parameters.Add("@Mode", "GET");
                parameters.Add("@ID", objMailServerConfig?.ID);
                parameters.Add("@TimeZoneID", objMailServerConfig?.TimeZoneID);
                var result = connection.QueryMultiple(
                          "sp_MailServerConfig",
                          parameters,
                          commandType: CommandType.StoredProcedure
                      );
                return result.Read<MailServer?>().ToList();
            }

        }
    }
}
