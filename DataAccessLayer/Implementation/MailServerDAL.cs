using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Newtonsoft.Json;
using System.Data;


namespace DataAccessLayer.Implementation
{
    public class MailServerDAL: RepositoryBase,IMailServerDAL
    {
        public MailServerDAL(IDbTransaction _transaction) : base(_transaction)
        {

        }
        public async Task<(bool deletemailserver, List<DeleteMailServerResult> deleteResults)> DeleteMailServer(long id, DeleteMailServer deleteMailServer)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(deleteMailServer.MailServerDeleteTable));
            parameters.Add("@UpdatedBy", id);
             parameters.Add("@Mode", Common.PageMode.DELETE);
            var Result = await Connection.QueryMultipleAsync("dbo.sp_MailServerConfig",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            List<DeleteMailServerResult> DeleteMailServer = (await Result.ReadAsync<DeleteMailServerResult>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }

            bool res = DeleteMailServer.Any();
            return (res, DeleteMailServer.ToList());
        }

        public async Task<List<GetMailServerModel>> GetAllMailServer()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@MailServerGUId", string.Empty);
             parameters.Add("@Mode", Common.PageMode.VIEW);
            var multi = await Connection.QueryMultipleAsync("dbo.sp_MailServerConfig",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<GetMailServerModel>().ToList();
        }

        public async Task<GetMailServerModel?> GetMailServerByGUId(string GUId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@MailServerGUId", GUId);
            parameters.Add("@Mode", Common.PageMode.VIEW);
            
            var multi = await Connection.QueryMultipleAsync("dbo.sp_MailServerConfig",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            
            var res = multi.Read<GetMailServerModel>().FirstOrDefault();

            return res;
        }

        public async Task<(bool Insertmailserver, long RetVal, string Msg)> InsertUpdateMailServer(MailServerModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@ReplyEmail", model.ReplyEMail);
            parameters.Add("@SMTP_Address", model.SMTP_Address);
            parameters.Add("@Active", model.Active);
            parameters.Add("@CredentialRequired", model.CredentialRequired);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password", model.Password);
            parameters.Add("@SMTP_Port", model.SMTP_Port);
            parameters.Add("@SSL_Required", model.SSL_Required);
            parameters.Add("@SetupName", model.SetupName);
            
             parameters.Add("@Mode", Common.PageMode.ADD);
            // Declare output parameters explicitly
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            var multi = await Connection.QueryMultipleAsync("dbo.sp_MailServerConfig",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            var mailServer = (await multi.ReadAsync<MailServerModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = mailServer.Any();
            long RetVal = parameters.Get<long>("@RetVal");
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            return (res, RetVal, Msg);
        }

        public async Task<(bool Updatemailserver, long RetVal, string Msg)> UpdateMailServer(UpdateMailServerModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@ReplyEMail", model.ReplyEMail);
            parameters.Add("@SMTP_Address", model.SMTP_Address);
            parameters.Add("@Active", model.Active);
            parameters.Add("@CredentialRequired", model.CredentialRequired);
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Password", model.Password);
            parameters.Add("@SMTP_Port", model.SMTP_Port);
            parameters.Add("@SSL_Required", model.SSL_Required);
            parameters.Add("@SetupName", model.SetupName);
            parameters.Add("@MailServerGUId", model.MailServerGuid);
             parameters.Add("@Mode", Common.PageMode.EDIT);
            // Declare output parameters explicitly
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("dbo.sp_MailServerConfig",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var mailServerModels = (await multi.ReadAsync<UpdateMailServerModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = mailServerModels.Any();
            long RetVal = parameters.Get<long>("@RetVal");
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            return (res, RetVal, Msg);
        }
    }
}
