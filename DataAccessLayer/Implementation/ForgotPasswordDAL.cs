using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Services;
using System.Data;
using System.Reflection;

namespace DataAccessLayer.Implementation
{
    public class ForgotPasswordDAL: RepositoryBase, IForgotPasswordDAL
    {
        
        public ForgotPasswordDAL(IDbTransaction transaction) :base(transaction)
        {
            
        }
        
        public async Task<(List<ForgotPasswordModel> forgotPasswordModels, int RetVal, string Msg)> InsertUpdateForgotPassword(ForgotPasswordModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", model.UserName);
            parameters.Add("@Token", model.Token);
             parameters.Add("@Mode", Common.PageMode.ADD);
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@RetMsg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            using (var result = await Connection.QueryMultipleAsync("sp_ForgotPassword",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
            {
                // Process the first result set (roles)
                var forgotPasswordModels = result.Read<ForgotPasswordModel>().ToList();
                // Ensure all result sets are consumed to retrieve the output parameters
                while (!result.IsConsumed)
                {
                    result.Read(); // Process remaining datasets
                }

                // Access the output parameters after consuming all datasets
                int retVal = parameters.Get<int?>("@RetVal") ?? -4;
                string msg = parameters.Get<string?>("@RetMsg") ?? "No Records Found";

                // Return the roles list along with output parameters
                return (forgotPasswordModels, retVal, msg);
            }
        }
        public MailServer mailServerPort()
        {
            return new MailServer();
        }
        public async Task<GetForgotPasswordModel?> ValidateTokenForgotPassword(string? token)
        {
            DynamicParameters parameters = new DynamicParameters();
            
            parameters.Add("@Token", token);
            parameters.Add("@Mode", "VALIDATE_TOKEN");
            
            using (var result = await Connection.QueryMultipleAsync("sp_ForgotPassword",
                                                                        parameters,
                                                                        transaction: Transaction,
                                                                        commandType: CommandType.StoredProcedure))
            {
                return result.Read<GetForgotPasswordModel?>().FirstOrDefault();
            }
        }
    }
}
