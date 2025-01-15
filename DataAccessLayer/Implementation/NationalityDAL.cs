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

        public async Task<bool> DeleteNationality(int Id)
        {
            //try {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", Id);
                var Result = await Connection.ExecuteAsync("StProc_Delete_Nationality",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
                return Result > 0 ?true:false;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            
        }

        public async Task<List<NationalityModel>> GetAllNationality()
        {
            //try
            //{
                DynamicParameters parameters = new DynamicParameters();
               // parameters.Add("@Id", Id);
                var multi = await Connection.QueryMultipleAsync("StProc_GetAllNationality",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
                return multi.Read<NationalityModel>().ToList();
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
            
        }

        public async Task<NationalityModel> GetNationalityById(int Id)
        {
            //try
            //{
                DynamicParameters parameters = new DynamicParameters();
                 parameters.Add("@Id", Id);
                var multi = await Connection.QueryMultipleAsync("StProc_GetNationalityById",
                    parameters,
                    transaction:Transaction,
                    commandType: CommandType.StoredProcedure);
               var res = multi.Read<NationalityModel>().First();

                return res;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<bool> InsertUpdateNationality(NationalityModel model)
        {
            //try
            //{
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", model.Id);
                parameters.Add("@NationalityCode", model.NationalityCode);
                parameters.Add("@Nationality", model.Nationality);
                parameters.Add("@Active", model.Active);
                var multi = await Connection.ExecuteAsync("StProc_InsertUpdateNationality",
                    parameters,
                    transaction: Transaction,
                    commandType: CommandType.StoredProcedure);
                var res = multi > 0 ? true:false;

                return res;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

        }
    }
}
