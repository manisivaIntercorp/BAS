using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Newtonsoft.Json;
using System.Data;

namespace DataAccessLayer.Implementation
{
    public class RoleDAL: RepositoryBase, IRoleDAL
    {
        
        public RoleDAL(IDbTransaction _transaction) : base(_transaction)
        {

        }
        public async Task<bool> DeleteRole(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", Id);
            parameters.Add("@Mode", "DELETE");
            var Result = await Connection.ExecuteAsync("sp_RoleUserCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return Result > 0 ? true : false;
        }

        public async Task<List<RoleModel>> GetAllRole()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", 0);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_RoleUserCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<RoleModel>().ToList();
        }

        public async Task<(RoleModel rolemodel,List<Modules> ModuleDatatable)> getModulesBasedonRole(long RoleId, string IsPayrollAccessible, long updatedby)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", RoleId);
            parameters.Add("@IsPayrollAccessible", IsPayrollAccessible);
            parameters.Add("@UpdatedBy", updatedby);
            parameters.Add("@Mode", "GET_MODULE_INFORMATION");
            var multi = await Connection.QueryMultipleAsync("sp_RoleUserCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            var Roles = (await multi.ReadAsync<RoleModel>()).FirstOrDefault();
            var Moduleinfo = (await multi.ReadAsync<Modules>()).ToList();
            
            return (Roles, Moduleinfo);
        }

        public async Task<(List<RoleModel> roleModels, int RetVal, string Msg)> InsertUpdateRole(RoleModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", model.RoleId);
            parameters.Add("@RoleDesc", model.RoleName);
            parameters.Add("@IsAdmin", model.IsAdmin);
            parameters.Add("@IsEntityAdmin", model.IsEntityAdmin);
            parameters.Add("@DisplayPDPAData", model.DisplayPDPAData);
            parameters.Add("@Active", model.Active);
            parameters.Add("@AccessToAllClient", model.AccessToAllClient);
            parameters.Add("@IsPayrollAccessible", model.IsPayrollAccessible);
            parameters.Add("@LevelDetailID", model.LevelDetailsID);
            parameters.Add("@LevelID", model.LevelID);
            parameters.Add("@tblRARDetail", JsonConvert.SerializeObject(model.ModuleTable), DbType.String);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@Mode", "ADD");
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var result = await Connection.QueryMultipleAsync("sp_RoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);

            var roles = result.Read<RoleModel>().ToList();

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            int retVal = parameters.Get<int?>("@RetVal") ?? -4;
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            // Return the roles list along with output parameters
            return (roles, retVal, msg);
        }

        public async Task<(List<RoleModel> roleModels, int RetVal, string Msg)> UpdateRoleAsync(int id,RoleModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", model.RoleId);
            parameters.Add("@RoleDesc", model.RoleName);
            parameters.Add("@IsAdmin", model.IsAdmin);
            parameters.Add("@IsEntityAdmin", model.IsEntityAdmin);
            parameters.Add("@DisplayPDPAData", model.DisplayPDPAData);
            parameters.Add("@Active", model.Active);
            parameters.Add("@AccessToAllClient", model.AccessToAllClient);
            parameters.Add("@IsPayrollAccessible", model.IsPayrollAccessible);
            parameters.Add("@LevelDetailID", model.LevelDetailsID);
            parameters.Add("@LevelID", model.LevelID);
            parameters.Add("@tblRARDetail", JsonConvert.SerializeObject(model.ModuleTable), DbType.String);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@Mode", "EDIT");
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var result = await Connection.QueryMultipleAsync("sp_RoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);

            var roles = result.Read<RoleModel>().ToList();

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            int retVal = parameters.Get<int?>("@RetVal") ?? -4;
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            // Return the roles list along with output parameters
            return (roles, retVal, msg);
        }
    }
}