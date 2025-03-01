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
    public class LanguageDAL : RepositoryBase, ILanguageDAL
    {
        public LanguageDAL(IDbTransaction _transaction) : base(_transaction)
        {

        }
        public async Task<bool> DeleteLanguage(int Id)
        {
            //try {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LanguageId", Id);
             parameters.Add("@Mode", Common.PageMode.DELETE);
            var Result = await Connection.ExecuteAsync("sp_LanguageCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return Result > 0 ? true : false;
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

        }

        public async Task<List<LanguageModel>> GetAllLanguage()
        {
            DynamicParameters parameters = new DynamicParameters();
             parameters.Add("@LanguageId", 0);
             parameters.Add("@Mode", Common.PageMode.GET);
            var multi = await Connection.QueryMultipleAsync("sp_LanguageCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<LanguageModel>().ToList();
        }
        public async Task<List<LanguageNameEnum>> GetAllLanguageinDropdown()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LanguageId", 0);
            parameters.Add("@Mode", "GET_LANGUAGE_IN_DROPDOWN");
            var multi = await Connection.QueryMultipleAsync("sp_LanguageCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            return multi.Read<LanguageNameEnum>().ToList();
        }
               

        public async Task<LanguageModel> GetLanguageById(int Id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LanguageId", Id);
             parameters.Add("@Mode", Common.PageMode.GET);
            var multi = await Connection.QueryMultipleAsync("sp_LanguageCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi.Read<LanguageModel>().First();

            return res;
        }
        
        public async Task<bool> InsertUpdateLanguage(LanguageModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LanguageId", model.LanguageID);
            parameters.Add("@LanguageCode", model.LanguageCode);
            parameters.Add("@LanguageName", model.LanguageName);
            parameters.Add("@Active", model.Active);
            parameters.Add("@UpdatedBy", model.CreatedBy);
             parameters.Add("@Mode", Common.PageMode.ADD);
            var multi = await Connection.ExecuteAsync("sp_LanguageCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi > 0 ? true : false;

            return res;
        }
        public async Task<bool> UpdateLanguageAsync(int id,LanguageModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@LanguageId", model.LanguageID);
            parameters.Add("@LanguageCode", model.LanguageCode);
            parameters.Add("@LanguageName", model.LanguageName);
            parameters.Add("@Active", model.Active);
            parameters.Add("@UpdatedBy", model.CreatedBy);
             parameters.Add("@Mode", Common.PageMode.EDIT);
            var multi = await Connection.ExecuteAsync("sp_LanguageCreation",
                parameters,
                transaction: Transaction,
                commandType: CommandType.StoredProcedure);
            var res = multi > 0 ? true : false;

            return res;
        }
    }
}
