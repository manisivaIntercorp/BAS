using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class NationalityDAL: RepositoryBase,INationalityDAL
    {
        public NationalityDAL(IDbTransaction _transaction) : base(_transaction) {
        
        }

        public async Task<bool> DeleteNationality(int id)
        {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", id);
            parameters.Add("@Mode", "DELETE");
                var Result = await Connection.ExecuteAsync("sp_NationalityCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
                return Result > 0 ?true:false;
            
        }

        public async Task<List<NationalityModel>> GetAllNationality()
        {
            
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", 0);
                parameters.Add("@Mode", "GET");
                var multi = await Connection.QueryMultipleAsync("sp_NationalityCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
                return multi.Read<NationalityModel>().ToList();
            
            
        }

        public async Task<NationalityModel> GetNationalityById(int Id)
        {
            
                DynamicParameters parameters = new DynamicParameters();
                 parameters.Add("@Id", Id);
            parameters.Add("@Mode", "GET");
            var multi = await Connection.QueryMultipleAsync("sp_NationalityCreation",
                    parameters,
                    transaction:Transaction,
                    commandType: CommandType.StoredProcedure);
               var res = multi.Read<NationalityModel>().First();

                return res;
            
        }

        public async Task<(bool Insertnationality, int RetVal, string Msg)> InsertUpdateNationality(NationalityModel model)
        {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", model.Id);
                parameters.Add("@NationalityCode", model.NationalityCode);
                parameters.Add("@Nationality", model.Nationality);
                parameters.Add("@Active", model.Active);
                parameters.Add("@Mode", "ADD");
            // Declare output parameters explicitly
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            var multi = await Connection.QueryMultipleAsync("sp_NationalityCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            var nationalities = (await multi.ReadAsync<NationalityModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = nationalities.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
                return (res, RetVal, Msg);
            

        }

        public async Task<(bool Updatenationality, int RetVal, string Msg)> UpdateNationality(NationalityModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@NationalityCode", model.NationalityCode);
            parameters.Add("@Nationality", model.Nationality);
            parameters.Add("@Active", model.Active);
            parameters.Add("@UpdatedBy", model.CreatedBy);
            parameters.Add("@Mode", "EDIT");
            // Declare output parameters explicitly
            parameters.Add("@RetVal", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_NationalityCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var nationalities = (await multi.ReadAsync<NationalityModel>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = nationalities.Any();
            int RetVal = parameters.Get<int?>("@RetVal") ?? -4;
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            return (res, RetVal, Msg);
        }
    }
}
