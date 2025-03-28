﻿using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class RoleDAL: RepositoryBase, IRoleDAL
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RoleDAL(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(new SqlConnection(configuration.GetConnectionString("connection")))
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

    public async Task<(bool? DeleteRole, List<DeleteRoleInformation?> deleteRoleInformation)> DeleteRole(RolesDelete? rolesdelete,long UserId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblDelete", JsonConvert.SerializeObject(rolesdelete?.DeleteRoleTable));
            parameters.Add("@UpdatedBy", UserId);
             parameters.Add("@Mode", Common.PageMode.DELETE);
            var Result = await Connection.QueryMultipleAsync("dbo.sp_RoleUserCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            List<DeleteRoleInformation?> DeleteRoleInfo = (await Result.ReadAsync<DeleteRoleInformation?>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }
            bool? res = DeleteRoleInfo.Any();
            return (res, DeleteRoleInfo);
        }

        public async Task<List<GetRoleModel>> GetAllRole(long UpdatedBy)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleGUID", string.Empty);
             parameters.Add("@Mode", Common.PageMode.GET);
            parameters.Add("@UpdatedBy", UpdatedBy);
            var multi = await Connection.QueryMultipleAsync("dbo.sp_RoleUserCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<GetRoleModel>().ToList();
        }

        public async Task<(RoleModel? rolemodel,List<Modules?> ModuleDatatable)> getModulesBasedOnRole(string? RoleGUID,long? updatedBy)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleGUID", RoleGUID);
            parameters.Add("@UpdatedBy", updatedBy);
            parameters.Add("@Mode", Common.PageMode.GET_MODULE_INFORMATION);
            var multi = await Connection.QueryMultipleAsync("dbo.sp_RoleUserCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            var Roles = (await multi.ReadAsync<RoleModel?>()).FirstOrDefault();
            var Moduleinfo = (await multi.ReadAsync<Modules?>()).ToList();
            
            return (Roles, Moduleinfo);
        }

        public async Task<List<Modules?>> getModulesBasedOnInsertRole(long? updatedBy)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleGuid", string.Empty);
            parameters.Add("@UpdatedBy", updatedBy);
            parameters.Add("@Mode", Common.PageMode.GET_MODULE_INFORMATION);
            var multi = await Connection.QueryMultipleAsync("dbo.sp_RoleUserCreation",
                                                             parameters,
                                                             transaction: Transaction,
                                                             commandType: CommandType.StoredProcedure);
            
            var ModuleInfo = (await multi.ReadAsync<Modules?>()).ToList();

            return ModuleInfo;
        }

        public async Task<(List<RoleModel?> roleModels, long? RetVal, string? Msg)> InsertUpdateRole(RoleModel? model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", model?.RoleId);
            parameters.Add("@RoleDesc", model?.RoleName);
            parameters.Add("@IsAdmin", model?.IsAdmin);
            parameters.Add("@IsEntityAdmin", model?.IsEntityAdmin);
            parameters.Add("@DisplayPDPAData", model?.DisplayPDPAData);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@AccessToAllClient", model?.AccessToAllClient);
            parameters.Add("@IsPayrollAccessible", model?.IsPayrollAccessible);
            parameters.Add("@LevelDetailID", model?.LevelDetailsID);
            parameters.Add("@LevelID", model?.LevelID);
            parameters.Add("@tblRARDetail", JsonConvert.SerializeObject(model?.ModuleTable), DbType.String);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
             parameters.Add("@Mode", Common.PageMode.ADD);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            parameters.Add("@RoleGUID", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var result = await Connection.QueryMultipleAsync("dbo.sp_RoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);

            var roles = result.Read<RoleModel?>().ToList();

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            long retVal = parameters.Get<long>("@RetVal");
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            string RoleGuid = parameters.Get<string?>("@RoleGUID") ??string.Empty;


            // Return the roles list along with output parameters
            return (roles, retVal, msg);
        }

        public async Task<string> ModuleTypeCheckBeforeInsert(string? RarMode)
        {
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@ModuleMode", RarMode);
            
            parameters.Add("@Mode", Common.PageMode.CHECK_ModuleType);
            
            var result = await Connection.QueryMultipleAsync("dbo.sp_RoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);


            string output = string.Empty; // Default output

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                var data = result.Read<string>().FirstOrDefault(); // Assume result contains string data
                if (!string.IsNullOrEmpty(data))
                {
                    output = data; // Capture meaningful result
                }
            }
            return output;
        }
        public async Task<(List<GetRoleModel?> roleModels, long? RetVal, string? Msg)> UpdateRole(GetRoleModel? model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleId", model?.RoleId);
            parameters.Add("@RoleDesc", model?.RoleName);
            parameters.Add("@IsAdmin", model?.IsAdmin);
            parameters.Add("@IsEntityAdmin", model?.IsEntityAdmin);
            parameters.Add("@DisplayPDPAData", model?.DisplayPDPAData);
            parameters.Add("@Active", model?.Active);
            parameters.Add("@AccessToAllClient", model?.AccessToAllClient);
            parameters.Add("@IsPayrollAccessible", model?.IsPayrollAccessible);
            parameters.Add("@LevelDetailID", model?.LevelDetailsID);
            parameters.Add("@LevelID", model?.LevelID);
            parameters.Add("@tblRARDetail", JsonConvert.SerializeObject(model?.ModuleTable), DbType.String);
            parameters.Add("@UpdatedBy", model?.CreatedBy);
            parameters.Add("@Mode", Common.PageMode.EDIT);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            parameters.Add("@RoleGUID", model?.RoleGuid);
            var result = await Connection.QueryMultipleAsync("dbo.sp_RoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);

            var roles = result.Read<GetRoleModel?>().ToList();

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            long retVal = parameters.Get<long>("@RetVal");
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            string RoleGuid = parameters.Get<string?>("@RoleGUID") ?? string.Empty;


            // Return the roles list along with output parameters
            return (roles, retVal, msg);
        }

        public async Task<(List<GetRoleModel?> roleModels, long? RetVal, string? Msg)> EditUpdateRoleAsync(GetRoleModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@RoleID", model.RoleId);
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
            parameters.Add("@Mode", Common.PageMode.EDIT);
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var result = await Connection.QueryMultipleAsync("dbo.sp_RoleUserCreation",
                                                              parameters,
                                                              transaction: Transaction,
                                                              commandType: CommandType.StoredProcedure);

            var roles = result.Read<GetRoleModel?>().ToList();

            // Ensure all result sets are consumed to retrieve the output parameters
            while (!result.IsConsumed)
            {
                result.Read(); // Process remaining datasets
            }

            // Access the output parameters after consuming all datasets
            long retVal = parameters.Get<long>("@RetVal");
            string msg = parameters.Get<string?>("@Msg") ?? "No Records Found";

            // Return the roles list along with output parameters
            return (roles, retVal, msg);
        }
    }
}