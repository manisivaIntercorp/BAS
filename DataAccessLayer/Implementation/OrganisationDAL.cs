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
                    parameters.Add("@Active", model.Active);
                    parameters.Add("@Mode", "ADD");
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
                            parameters.Add("@Mode", "GET");

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
        public async Task<OrganisationModel> GetOrganisationById(int Id)
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
                            parameters.Add("@UserId", Id);
                            parameters.Add("@Mode", "GET");

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

       
        //public async Task<string> InsertOrganisation(OrganisationModel model)
        //{
        //    try
        //    {
        //        DynamicParameters parameters = new DynamicParameters();
        //        parameters.Add("@OrgID", model.ID);
        //        parameters.Add("@OrgCode", model.OrgCode);
        //        parameters.Add("@OrgName", model.OrgName);
        //        parameters.Add("@OrgAddress", model.Address);
        //        parameters.Add("@Country", model.Country);
        //        parameters.Add("@PriContactNo", model.PriContactNo);
        //        parameters.Add("@SecContactNo", model.SecContactNo);
        //        parameters.Add("@PriEmailAddress", model.PriEmailAddress);
        //        parameters.Add("@SecEmailAddress", model.SecEmailAddress);
        //        parameters.Add("@CcEmailAddress", model.CcEmailAddress);
        //        parameters.Add("@CompanyLogo", model.Logo);
        //        parameters.Add("@UpdatedBy", model.UserID);
        //        parameters.Add("@Active", model.Active);
        //        parameters.Add("@Mode", "ADD");

        //        // Execute stored procedure
        //        var multi = await Connection.ExecuteAsync("sp_OrganisationConfiguration",
        //            parameters,
        //            transaction: Transaction,
        //            commandType: CommandType.StoredProcedure);

        //        // Determine success based on rows affected
        //        return multi > 0 ? "1" : "0";
        //    }
        //    catch (SqlException ex)
        //    {
        //        // Log SQL exception (log framework or custom logger)
        //        Console.WriteLine($"SQL Error: {ex.Message}");
        //        return $"Error: {ex.Message}";
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log general exceptions
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return $"Error: {ex.Message}";
        //    }
        //}

        //public async Task<string> InsertOrganisation(OrganisationModel model)
        //{
        //    try
        //    {
        //        using (IUowOrganisation _repo = new UowOrganisation(ConnectionString))
        //        {
        //            string query = "sp_OrganisationConfiguration";
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@OrgID", model.ID);
        //            parameters.Add("@OrgCode", model.OrgCode);
        //            // Add remaining parameters...

        //            var result = await _connection.ExecuteAsync(query, parameters, commandType: CommandType.StoredProcedure);
        //            return result > 0 ? "1" : "0";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Database error: {ex.Message}");
        //    }
        //}

        ///// <summary>
        ///// workin
        ///// </summary>
        ///// <param name = "model" ></ param >
        ///// < returns ></ returns >
        //public async Task<string> InsertOrganisation(OrganisationModel model)
        //{
        //    try
        //    {
        //        // Prepare the connection string
        //        using (var connection = new SqlConnection(Connection.ConnectionString)) // Reuse the existing connection string
        //        {
        //            // Open the connection
        //            await connection.OpenAsync();

        //            // Prepare parameters for the stored procedure
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@OrgID", model.ID);
        //            parameters.Add("@OrgCode", model.OrgCode);
        //            parameters.Add("@OrgName", model.OrgName);
        //            parameters.Add("@OrgAddress", model.Address);
        //            parameters.Add("@Country", model.Country);
        //            parameters.Add("@PriContactNo", model.PriContactNo);
        //            parameters.Add("@SecContactNo", model.SecContactNo);
        //            parameters.Add("@PriEmailAddress", model.PriEmailAddress);
        //            parameters.Add("@SecEmailAddress", model.SecEmailAddress);
        //            parameters.Add("@CcEmailAddress", model.CcEmailAddress);
        //            parameters.Add("@CompanyLogo", model.Logo);
        //            parameters.Add("@UpdatedBy", model.UserID);
        //            parameters.Add("@Active", model.Active);
        //            parameters.Add("@Mode", "ADD");

        //            // Execute the stored procedure
        //            int rowsAffected = await connection.ExecuteAsync(
        //                "sp_OrganisationConfiguration",
        //                parameters,
        //                commandType: CommandType.StoredProcedure
        //            );

        //            // Return result based on rows affected
        //            return rowsAffected > 0 ? "1" : "0";
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        // Log SQL exception
        //        Console.WriteLine($"SQL Error: {ex.Message}");
        //        return $"SQL Error: {ex.Message}";
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log general exceptions
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return $"Error: {ex.Message}";
        //    }
        //}

        //public async Task<string> InsertOrganisation(OrganisationModel model)
        //{
        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@UserName", model.OrgCode);
        //    parameters.Add("@Password", model.OrgName);

        //    parameters.Add("@Mode", "ADD");
        //    var multi = await Connection.ExecuteAsync("sp_SampleTest",
        //        parameters,
        //        transaction: Transaction,
        //        commandType: CommandType.StoredProcedure);
        //    var res = multi > 0 ? "1" : "0";
        //    return res;
        //}


        //  public async Task<string> InsertOrganisation(OrganisationModel model)
        //{
        //    DynamicParameters parameters = new DynamicParameters();

        //    parameters.Add("@UserName", model.OrgCode);
        //    parameters.Add("@Password", model.OrgName);
        //    parameters.Add("@Mode", "ADD");

        //    var result = await Connection.QueryFirstOrDefaultAsync<string>("sp_SampleTest",
        //        parameters,
        //        transaction: Transaction,
        //        commandType: CommandType.StoredProcedure);

        //    return result == "Success" ? "1" : "0";
        //}

        public async Task<string> UpdateOrganisation(int id, OrganisationModel model)
        {
            
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    //DynamicParameters parameters = new DynamicParameters();
                    //parameters.Add("@OrgID", id);
                    //parameters.Add("@OrgCode", model.OrgCode);
                    //parameters.Add("@OrgName", model.OrgName);
                    //parameters.Add("@OrgAddress", model.Address);
                    //parameters.Add("@Country", model.Country);
                    //parameters.Add("@PriContactNo", model.PriContactNo);
                    //parameters.Add("@SecContactNo", model.SecContactNo);
                    //parameters.Add("@PriEmailAddress", model.PriEmailAddress);
                    //parameters.Add("@SecEmailAddress", model.SecEmailAddress);
                    //parameters.Add("@CcEmailAddress", model.CcEmailAddress);
                    //parameters.Add("@CompanyLogo", model.Logo);
                    //parameters.Add("@UpdatedBy", model.UserID);
                    //parameters.Add("@Active", model.Active);
                    //parameters.Add("@Mode", "EDIT");
                    //var multi = await connection.ExecuteAsync("sp_OrganisationConfiguration",
                    //    parameters,
                    //    transaction: Transaction,
                    //    commandType: CommandType.StoredProcedure);
                    //var res = multi > 0 ? true : false;

                    //return res;


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
                    parameters.Add("@Mode", "EDIT");
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
    }
}
