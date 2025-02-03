using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class LoginDAL : RepositoryBase, ILoginDAL
    {
        public LoginDAL(IDbTransaction transaction) : base(transaction)
        {

        }

        public async Task<List<ResultModel>> UserLogin(LoginModel objLoginModel)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@Mode", "Get");
            dynamicParameters.Add("@UserName", objLoginModel.UserName);
            dynamicParameters.Add("@Password", objLoginModel.Password);
            dynamicParameters.Add("@UserType", objLoginModel.UserType);
            dynamicParameters.Add("@IPAddress", objLoginModel.IPAddress);
            dynamicParameters.Add("@DeviceName", objLoginModel.DeviceName);
            dynamicParameters.Add("@BrowserName", objLoginModel.BrowserName);

            if (Connection == null)
                throw new ArgumentNullException(nameof(Connection), "The database connection cannot be null.");

            using var multi = await Connection.QueryMultipleAsync(
               "sp_Authentication",
               dynamicParameters,
               transaction: Transaction,
               commandType: CommandType.StoredProcedure);

            var res = multi.Read<ResultModel>().ToList();
            var details = multi.Read<LoginDetailModel>().ToList();

            if (res.Any())
            {
                res[0].lstLoginDetails = details;
            }
            return res;
        }


        public async Task<List<LoginModel>> GetUserID(LoginModel objloginModel)
        {
            DynamicParameters dyParameter = new DynamicParameters();
            dyParameter.Add("@Mode", objloginModel.Mode);
            dyParameter.Add("@UserName", objloginModel.UserName);
            dyParameter.Add("@Password", objloginModel.Password);
            dyParameter.Add("@UserType", objloginModel.UserType);
            dyParameter.Add("@IPAddress", objloginModel.IPAddress);
            dyParameter.Add("@DeviceName", objloginModel.DeviceName);
            dyParameter.Add("@BrowserName", objloginModel.BrowserName);
            if (Connection == null)
                throw new ArgumentNullException(nameof(Connection), "The database connection cannot be null.");

            var mulit = await Connection.QueryMultipleAsync("sp_Authentication",
                dyParameter,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = mulit.Read<LoginModel>().ToList();

            return res;
        }


    }
}
