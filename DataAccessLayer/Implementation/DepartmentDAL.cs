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

namespace DataAccessLayer.Implementation
{
    public class DepartmentDAL : RepositoryBase,IDepartmentDAL
    {
        private readonly string _connectionString;
        public DepartmentDAL(IDbTransaction transaction, string connectionString) : base(transaction)
        {
            _connectionString = connectionString;
        }

        public async Task<DepartmentModel> GetDepartment(DepartmentInput departmentInput)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", departmentInput.Mode);
                    parameters.Add("@UpdatedBy", departmentInput.UpdatedGuidBy);
                    parameters.Add("@Level_Detail_GUID", departmentInput.LevelDetailGUID);
                    parameters.Add("@DeptGUID", departmentInput.DeptGUID);
                    parameters.Add("@Function", departmentInput.Function);

                    using (var multi = await connection.QueryMultipleAsync(
                        "sp_Department", parameters, commandType: CommandType.StoredProcedure))
                    {
                        var departmentModel = new DepartmentModel
                        {
                            lstDeptDetails = (await multi.ReadAsync<DeptDetails>()).ToList(),
                            lstDivision = (await multi.ReadAsync<Division>()).ToList(),
                            lstCategory = (await multi.ReadAsync<Category>()).ToList(),
                            lstDivisionSelection = (await multi.ReadAsync<DivisionSelection>()).ToList(),
                            lstCategorySelection = (await multi.ReadAsync<CategorySelection>()).ToList()
                        };

                        return departmentModel;
                    }
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException("An error occurred while fetching department data", ex);
            }
        }

        public async Task<DepartmentModel> ViewDepartment(DepartmentInput departmentInput)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@Mode", departmentInput.Mode);
                    parameters.Add("@UpdatedBy", departmentInput.UpdatedGuidBy);
                    parameters.Add("@Level_Detail_GUID", departmentInput.LevelDetailGUID);
                    parameters.Add("@DeptGUID", departmentInput.DeptGUID);
                    parameters.Add("@Function", departmentInput.Function);

                    using (var multi = await connection.QueryMultipleAsync(
                        "sp_Department", parameters, commandType: CommandType.StoredProcedure))
                    {
                        var departmentModel = new DepartmentModel
                        {
                            lstDeptDetails = (await multi.ReadAsync<DeptDetails>()).ToList(),
                            lstDivisionSelection = (await multi.ReadAsync<DivisionSelection>()).ToList(),
                            lstCategorySelection = (await multi.ReadAsync<CategorySelection>()).ToList()
                        };

                        return departmentModel;
                    }
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException("An error occurred while fetching department data", ex);
            }
        }


        public async Task<string> InsertDepartmentDetails(DepartmentInput organisationLevelModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var table = new DataTable();
                    table.Columns.Add("DeptGUID", typeof(string));
                    table.Columns.Add("DivisionGUID", typeof(string));
                    table.Columns.Add("CategoryGUID", typeof(string));
                    table.Columns.Add("Department", typeof(string));

                    foreach (var item in organisationLevelModel.lstDepart)
                    {
                        table.Rows.Add(item.DeptGUID, item.DivisionGUID, item.CategoryGUID, item.Department);
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@tblDepartmentDetail", table.AsTableValuedParameter("dbo.utt_DepartmentDetail")); // Ensure this matches the table type
                    parameters.Add("@Mode", "ADD");
                    parameters.Add("@LevelGUID", organisationLevelModel.LevelGUID);
                    parameters.Add("@Level_Detail_GUID", organisationLevelModel.LevelDetailGUID);
                    parameters.Add("@DeptCode", organisationLevelModel.DeptCode);
                    parameters.Add("@DeptDesc", organisationLevelModel.DeptDesc);
                    parameters.Add("@Reference_ID", organisationLevelModel.ReferenceID);
                    parameters.Add("@ColourCode", organisationLevelModel.ColourCode);
                    parameters.Add("@TimeZoneID", organisationLevelModel.TimeZoneID);
                    parameters.Add("@UpdatedBy", organisationLevelModel.UpdatedGuidBy);
                    parameters.Add("@Msg", dbType: DbType.String, direction: ParameterDirection.Output, size: 2000);

                    var result = await connection.ExecuteAsync(
                        "sp_Department",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    string message = parameters.Get<string>("@Msg");

                    return message; // Ensure this matches the controller's expectation
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "0";
            }
        }

        public async Task<string> UpdateDepartmentDetails(DepartmentInput organisationLevelModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var table = new DataTable();
                    table.Columns.Add("DeptGUID", typeof(string));
                    table.Columns.Add("DivisionGUID", typeof(string));
                    table.Columns.Add("CategoryGUID", typeof(string));
                    table.Columns.Add("Department", typeof(string));

                    foreach (var item in organisationLevelModel.lstDepart)
                    {
                        table.Rows.Add(item.DeptGUID, item.DivisionGUID, item.CategoryGUID, item.Department);
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@tblDepartmentDetail", table.AsTableValuedParameter("dbo.utt_DepartmentDetail")); // Ensure this matches the table type
                    parameters.Add("@Mode", organisationLevelModel.Mode);
                    parameters.Add("@LevelGUID", organisationLevelModel.LevelGUID);
                    parameters.Add("@Level_Detail_GUID", organisationLevelModel.LevelDetailGUID);
                    parameters.Add("@DeptCode", organisationLevelModel.DeptCode);
                    parameters.Add("@DeptDesc", organisationLevelModel.DeptDesc);
                    parameters.Add("@Reference_ID", organisationLevelModel.ReferenceID);
                    parameters.Add("@ColourCode", organisationLevelModel.ColourCode);
                    parameters.Add("@TimeZoneID", organisationLevelModel.TimeZoneID);
                    parameters.Add("@UpdatedBy", organisationLevelModel.UpdatedGuidBy);
                    parameters.Add("@DeptGUID", organisationLevelModel.DeptGUID);
                    parameters.Add("@Msg", dbType: DbType.String, direction: ParameterDirection.Output, size: 2000);

                    var result = await connection.ExecuteAsync(
                        "sp_Department",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    string message = parameters.Get<string>("@Msg");

                    return message;// Ensure this matches the controller's expectation
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return "0";
            }
        }


        public async Task<List<DeptDeleteResult>> DeleteDepartmentDetails(DepartmentInput organisationLevelModel)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var table = new DataTable();
                    table.Columns.Add("DeptGUID", typeof(string));
                    table.Columns.Add("DivisionGUID", typeof(string));
                    table.Columns.Add("CategoryGUID", typeof(string));
                    table.Columns.Add("Department", typeof(string));

                    foreach (var item in organisationLevelModel.lstDepart)
                    {
                        table.Rows.Add(item.DeptGUID, item.DivisionGUID, item.CategoryGUID, item.Department);
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@tblDepartmentDetail", table.AsTableValuedParameter("dbo.utt_DepartmentDetail"));
                    parameters.Add("@Mode", "DELETE");
                    parameters.Add("@Msg", dbType: DbType.String, direction: ParameterDirection.Output, size: 2000);

                    var result = await connection.QueryAsync<DeptDeleteResult>(
                        "sp_Department",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    string message = parameters.Get<string>("@Msg");

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<DeptDeleteResult>(); // Return empty list on failure instead of string
            }
        }


    }
}
