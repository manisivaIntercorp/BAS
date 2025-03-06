using Dapper;
using DataAccessLayer.Interface;
using DataAccessLayer.Model;
using Newtonsoft.Json;
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

        public async Task<(bool deletenationality, List<DeleteNationalityResult> deleteResults)> DeleteNationality(long id, DeleteNationality deleteNationality)
        {
                DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@tblNationality", JsonConvert.SerializeObject(deleteNationality.NationalityDeleteTable));
            parameters.Add("@UpdatedBy", id);
             parameters.Add("@Mode", Common.PageMode.DELETE);
                var Result = await Connection.QueryMultipleAsync("sp_NationalityCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
            List<DeleteNationalityResult> DeleteNationality = (await Result.ReadAsync<DeleteNationalityResult>()).ToList();
            while (!Result.IsConsumed)
            {
                await Result.ReadAsync();
            }

            bool res = DeleteNationality.Any();
            return (res, DeleteNationality.ToList());
        }

        public async Task<List<GetNationalityModel>> GetAllNationality()
        {
            
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@NationalityGUId", string.Empty);
                 parameters.Add("@Mode", Common.PageMode.GET);
                var multi = await Connection.QueryMultipleAsync("sp_NationalityCreation",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
                return multi.Read<GetNationalityModel>().ToList();
            
            
        }

        public async Task<GetNationalityModel?> GetNationalityByGUId(string GUId)
        {
            DynamicParameters parameters = new DynamicParameters();
                 parameters.Add("@NationalityGUId", GUId);
                  parameters.Add("@Mode", Common.PageMode.GET);
                var multi = await Connection.QueryMultipleAsync("sp_NationalityCreation",
                    parameters,
                    transaction:Transaction,
                    commandType: CommandType.StoredProcedure);
               var res = multi.Read<GetNationalityModel>().FirstOrDefault();

                return res;
            
        }

        public async Task<(bool Insertnationality, long RetVal, string Msg)> InsertUpdateNationality(NationalityModel model)
        {
           DynamicParameters parameters = new DynamicParameters();
           parameters.Add("@Id", model.Id);
           parameters.Add("@UpdatedBy", model.CreatedBy);
           parameters.Add("@NationalityCode", model.NationalityCode);
           parameters.Add("@Nationality", model.Nationality);
           parameters.Add("@Active", model.Active);
            parameters.Add("@Mode", Common.PageMode.ADD);
           // Declare output parameters explicitly
           parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
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
            long RetVal = parameters.Get<long>("@RetVal");
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
                return (res, RetVal, Msg);
            

        }

        public async Task<(bool Updatenationality, long RetVal, string Msg)> UpdateNationality(UpdateNationality model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@NationalityCode", model.NationalityCode);
            parameters.Add("@Nationality", model.Nationality);
            parameters.Add("@Active", model.Active);
            parameters.Add("@UpdatedBy", model.CreatedBy);
             parameters.Add("@Mode", Common.PageMode.EDIT);
            // Declare output parameters explicitly
            parameters.Add("@RetVal", dbType: DbType.Int64, direction: ParameterDirection.Output);
            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
            var multi = await Connection.QueryMultipleAsync("sp_NationalityCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var nationalities = (await multi.ReadAsync<UpdateNationality>()).ToList();
            while (!multi.IsConsumed)
            {
                await multi.ReadAsync();
            }

            bool res = nationalities.Any();
            long RetVal = parameters.Get<long>("@RetVal");
            string Msg = parameters.Get<string?>("@Msg") ?? "No Records Found";
            return (res, RetVal, Msg);
        }
    }
}
