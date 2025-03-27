using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DataAccessLayer.Implementation
{
    public class ClientOrganisationDAL : RepositoryBase, IClientOrganisationDAL
    {
        private readonly string _connectionString;
        public ClientOrganisationDAL(IDbTransaction transaction, string connectionString): base(transaction)
        {
            _connectionString = connectionString;
        }

        public async Task<List<OrganisationModel>> GetClientOrganisation()
        {
            List<OrganisationModel> lstResult = new List<OrganisationModel>();
            try
            {

                using (var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@Mode", Common.PageMode.GET);

                            var multi = await connection.QueryMultipleAsync(
                                "sp_OrganisationConfiguration",
                                parameters,
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure);

                            //transaction.Commit();

                            return multi.Read<OrganisationModel>().ToList();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return lstResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return lstResult;
            }
        }

        public async Task<string> OrganisationLevelEdit(OrganisationLevelModel organisationLevelModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(); // Ensure connection is open

                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", organisationLevelModel.Mode);
                    parameters.Add("@LevelGuiD", organisationLevelModel.LevelID);
                    parameters.Add("@LevelCode", organisationLevelModel.LevelCode);
                    parameters.Add("@LevelDesc", organisationLevelModel.LevelDesc);
                    parameters.Add("@ParentLevelGuiD", organisationLevelModel.ParentLevelGuid);
                    parameters.Add("@LevelGuiD", organisationLevelModel.LevelGuid);
                    parameters.Add("@IsProject", organisationLevelModel.IsProject);
                    parameters.Add("@ModifiedBy", organisationLevelModel.UserGuid);

                    // Table-Valued Parameter (TVP)
                    var table = new DataTable();
                    table.Columns.Add("LevelID", typeof(long));               // Matches BIGINT
                    table.Columns.Add("LevelInfo", typeof(string));           // Matches VARCHAR(150)
                    table.Columns.Add("Guid", typeof(string));               // Matches NVARCHAR(50)
                    parameters.Add("@Msg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

                    // Execute stored procedure without a transaction
                    await connection.ExecuteAsync(
                        "sp_OrganisationLevel",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return parameters.Get<string>("@Msg");
                }
            }
            catch (SqlException ex)
            {
                // Log the SQL exception properly instead of using Console.WriteLine
                Console.WriteLine($"SQL Error: {ex.Message}");
                return "SQL Error occurred.";
            }
            catch (Exception ex)
            {
                // Log the general exception properly
                Console.WriteLine($"Error: {ex.Message}");
                return "An unexpected error occurred.";
            }
        }


        public async Task<List<LevelInfoModel>> GetOrganisationLevelInfo(OrganisationLevelModel organisationLevelModel)
        {
            List<LevelInfoModel> lstResult = new List<LevelInfoModel>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(); // Ensure connection is open

                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", organisationLevelModel.Mode);
                    //parameters.Add("@LevelID", organisationLevelModel.LevelID);
                    //parameters.Add("@LevelCode", organisationLevelModel.LevelCode);
                    //parameters.Add("@LevelDesc", organisationLevelModel.LevelDesc);
                    //parameters.Add("@ParentLevelID", organisationLevelModel.ParentLevelID);
                    //parameters.Add("@IsProject", organisationLevelModel.IsProject);
                    //parameters.Add("@ModifiedBy", organisationLevelModel.ModifiedBy);

                    // Table-Valued Parameter (TVP)
                    var table = new DataTable();
                    table.Columns.Add("LevelID", typeof(long));               // Matches BIGINT
                    table.Columns.Add("LevelInfo", typeof(string));           // Matches VARCHAR(150)
                    table.Columns.Add("Guid", typeof(string));               // Matches NVARCHAR(50)

                    if (organisationLevelModel.LevelInfo != null)
                    {
                        foreach (var item in organisationLevelModel.LevelInfo)
                        {
                            var levelInfo = item.LevelInfo.Length > 150 ? item.LevelInfo.Substring(0, 150) : item.LevelInfo;
                            table.Rows.Add(item.LevelID, levelInfo, item.Guid);
                        }
                    }
            
                    // Ensure you pass the correct table type name
                    parameters.Add("@dtLevelInfo", table.AsTableValuedParameter("dbo.utt_LevelInfo"));

                    parameters.Add("@dtLevelInfo", table.AsTableValuedParameter("utt_LevelInfo"));
                    parameters.Add("@Msg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

                    // Execute stored procedure without a transaction
                    var multi = await connection.QueryMultipleAsync(
                        "sp_OrganisationLevel",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return multi.Read<LevelInfoModel>().ToList();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return lstResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return lstResult;
            }
        }
        public async Task<List<LevelInfoModel>> SelectOrganisationLevelInfo(OrganisationLevelModel organisationLevelModel)
        {
            List<LevelInfoModel> lstResult = new List<LevelInfoModel>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(); // Ensure connection is open

                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", organisationLevelModel.Mode);
                    parameters.Add("@LevelGuiD", organisationLevelModel.LevelGuid);
                    //parameters.Add("@LevelID", organisationLevelModel.LevelID);
                    //parameters.Add("@LevelCode", organisationLevelModel.LevelCode);
                    //parameters.Add("@LevelDesc", organisationLevelModel.LevelDesc);
                    //parameters.Add("@ParentLevelID", organisationLevelModel.ParentLevelID);
                    //parameters.Add("@IsProject", organisationLevelModel.IsProject);
                    //parameters.Add("@ModifiedBy", organisationLevelModel.ModifiedBy);

                    // Table-Valued Parameter (TVP)
                    var table = new DataTable();
                    table.Columns.Add("LevelID", typeof(long));               // Matches BIGINT
                    table.Columns.Add("LevelInfo", typeof(string));           // Matches VARCHAR(150)
                    table.Columns.Add("Guid", typeof(string));               // Matches NVARCHAR(50)

                    if (organisationLevelModel.LevelInfo != null)
                    {
                        foreach (var item in organisationLevelModel.LevelInfo)
                        {
                            var levelInfo = item.LevelInfo.Length > 150 ? item.LevelInfo.Substring(0, 150) : item.LevelInfo;
                            table.Rows.Add(item.LevelID, levelInfo, item.Guid);
                        }
                    }

                    // Ensure you pass the correct table type name
                    parameters.Add("@dtLevelInfo", table.AsTableValuedParameter("dbo.utt_LevelInfo"));

                    parameters.Add("@dtLevelInfo", table.AsTableValuedParameter("utt_LevelInfo"));
                    parameters.Add("@Msg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

                    // Execute stored procedure without a transaction
                    var multi = await connection.QueryMultipleAsync(
                        "sp_OrganisationLevel",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return multi.Read<LevelInfoModel>().ToList();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return lstResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return lstResult;
            }
        }


         public async Task<List<DeleteResultModel>> DeleteOrganisationLevel(List<LevelInfoDetails> levelInfoDetails, string struserGuid, string strMode)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();

                    // Table-Valued Parameter (TVP)
                    var table = new DataTable();
                    table.Columns.Add("LevelID", typeof(long));               // Matches BIGINT
                    table.Columns.Add("LevelInfo", typeof(string));           // Matches VARCHAR(150)
                    table.Columns.Add("Guid", typeof(string));               // Matches NVARCHAR(50)

                    if (levelInfoDetails != null)  // Ensure list is not null
                    {
                        foreach (var item in levelInfoDetails)  // Directly iterate over the list
                        {
                            var levelInfo = item?.LevelInfo ?? string.Empty;

                            levelInfo = levelInfo.Length > 150 ? levelInfo.Substring(0, 150) : levelInfo;

                            table.Rows.Add(item.LevelID, levelInfo, item.Guid);
                        }
                    }


                    // Ensure you pass the correct table type name
                    parameters.Add("@dtLevelInfo", table.AsTableValuedParameter("dbo.utt_LevelInfo"));
                    parameters.Add("@Mode", "DELETE");
                    parameters.Add("@ModifiedBy", struserGuid);
                    parameters.Add("@Msg", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

                    var result = await connection.QueryAsync<DeleteResultModel>(
                        "sp_OrganisationLevel",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.ToList();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return new List<DeleteResultModel> { new DeleteResultModel { SNo = 0, LevelInfo = "Error", Result = "Failed", Remarks = "SQL Error occurred." } };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<DeleteResultModel> { new DeleteResultModel { SNo = 0, LevelInfo = "Error", Result = "Failed", Remarks = "An unexpected error occurred." } };
            }
        }


    }
}
