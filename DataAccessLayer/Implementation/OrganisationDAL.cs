using Azure;
using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;



namespace DataAccessLayer.Implementation
{
    internal class OrganisationDAL : RepositoryBase, IOrganisationDAL
    {

        private readonly string _connectionString;

        public OrganisationDAL(IDbTransaction transaction, string connectionString)
       : base(transaction)
        {
            _connectionString = connectionString;
        }
        public async Task<string> InsertOrganisation(OrganisationModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@OrgID", model.ID);
                    parameters.Add("@OrgCode", model.OrgCode);
                    parameters.Add("@OrgName", model.OrgName);
                    parameters.Add("@OrgAddress", model.Address);
                    parameters.Add("@Country", model.Country);
                    parameters.Add("@PriContactNo", model.PriContactNo);
                    parameters.Add("@SecContactNo", model.SecContactNo);
                    parameters.Add("@PriEmailAddress", model.PriEmailAddress);
                    parameters.Add("@SecEmailAddress", model.SecEmailAddress);
                    parameters.Add("@CcEmailAddress", model.CcEmailAddress);
                    parameters.Add("@CompanyLogo", model.Logo);
                    parameters.Add("@UpdatedBy", model.UserID);
                    parameters.Add("@PinCode", model.PinCode);
                    parameters.Add("@Active", model.Active);
                    parameters.Add("@Mode", Common.PageMode.ADD);
                    parameters.Add("@Msg", dbType: DbType.String, size: 2000, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "sp_OrganisationConfiguration",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return parameters.Get<string>("@Msg");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        public async Task<List<OrganisationModel>> GetAllOrganisation()
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


        public async Task<OrganisationModel> GetOrganisationById(string strGuid)
        {
            OrganisationModel Rst = new OrganisationModel();
            try
            {
                using (var connection = new  SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@Guid", strGuid);
                            parameters.Add("@Mode", Common.PageMode.GET);

                            var multi = await connection.QueryMultipleAsync(
                                "sp_OrganisationConfiguration",
                                parameters,
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure);

                            var res = multi.Read<OrganisationModel>().First();

                            // Commit the transaction if everything is successful
                            transaction.Commit();
                            return res;
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
                return Rst;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Rst;
            }
        }

        public async Task<string> UpdateOrganisation(OrganisationModel model)
        {
            
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new DynamicParameters();
                    parameters.Add("@OrgID", model.ID);
                    parameters.Add("@OrgCode", model.OrgCode);
                    parameters.Add("@OrgName", model.OrgName);
                    parameters.Add("@OrgAddress", model.Address);
                    parameters.Add("@Country", model.Country);
                    parameters.Add("@PriContactNo", model.PriContactNo);
                    parameters.Add("@SecContactNo", model.SecContactNo);
                    parameters.Add("@PriEmailAddress", model.PriEmailAddress);
                    parameters.Add("@SecEmailAddress", model.SecEmailAddress);
                    parameters.Add("@CcEmailAddress", model.CcEmailAddress);
                    parameters.Add("@CompanyLogo", model.Logo);
                    parameters.Add("@UpdatedBy", model.UserID);
                    parameters.Add("@Active", model.Active);
                    parameters.Add("@Guid", model.CustomerGuid);
                    parameters.Add("@Mode", Common.PageMode.EDIT);
                    parameters.Add("@PinCode", model.PinCode);
                    parameters.Add("@Msg", dbType: DbType.String, size: 2000, direction: ParameterDirection.Output);

                    await connection.ExecuteAsync(
                        "sp_OrganisationConfiguration",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return parameters.Get<string>("@Msg");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }

        }


        public async Task<List<OrganisationDeleteRecord>> DeleteOrganisation(List<DeleteRecord> dltOrg)
        {
            var res = new List<OrganisationDeleteRecord>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var table = new DataTable();
                    table.Columns.Add("Guid", typeof(string));
                    table.Columns.Add("FldInfo", typeof(string));

                    foreach (var record in dltOrg)
                    {
                        // Ensure null values don't cause issues
                        table.Rows.Add(record.Guid ?? DBNull.Value.ToString(), record.FldInfo ?? DBNull.Value.ToString());
                    }

                    var parameters = new DynamicParameters();
                    parameters.Add("@dtOrganisation", table.AsTableValuedParameter("utt_DeleteRecords"));
                     parameters.Add("@Mode", Common.PageMode.DELETE);

                    var multi = await connection.QueryMultipleAsync(
                        "sp_OrganisationConfiguration",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    res = multi.Read<OrganisationDeleteRecord>().ToList();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return res;
        }
        public async Task<List<DataLocationDropdown>> DataLocationInDropdown()
        {
            List<DataLocationDropdown> lstResult = new List<DataLocationDropdown>();
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
                            parameters.Add("@Mode", Common.PageMode.GET_DATALOCATION_DROPDOWN);

                            var multi = await connection.QueryMultipleAsync(
                                "sp_ListData",
                                parameters,
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure);

                            //transaction.Commit();

                            return multi.Read<DataLocationDropdown>().ToList();
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



    }
}
