using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using DataAccessLayer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Management.XEvent;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data.Common;
using System.Transactions;


namespace DataAccessLayer.Implementation
{
    public class MenuDAL : RepositoryBase, IMenuDAL
    {

        private readonly string _connectionString;

        public MenuDAL(IDbTransaction transaction, string connectionString)
       : base(transaction)
        {
            _connectionString = connectionString;
        }


        public async Task<List<MenuModel>> GetAllMenu()
        {
            List<MenuModel> lstResult = new List<MenuModel>();
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
                            parameters.Add("@Mode", Common.PageMode.MODULES);

                            var multi = await connection.QueryMultipleAsync(
                                "sp_OrganisationConfiguration",
                                parameters,
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure);

                            //transaction.Commit();

                            return multi.Read<MenuModel>().ToList();
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



        public async Task<List<MenusModel>> GetMenu(string UserName, string ClientCode, int OrgID)
        {
            List<MenusModel> lstResult = new List<MenusModel>();
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
                            parameters.Add("@UserName", UserName);
                            parameters.Add("@ClientCode", ClientCode);
                            parameters.Add("@OrgID", OrgID);

                            var multi = await connection.QueryMultipleAsync(
                                "sp_GetMenu",
                                parameters,
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure);

                            //transaction.Commit();

                            return multi.Read<MenusModel>().ToList();
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
